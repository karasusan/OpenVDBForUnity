#include "pch.h"
#include "oiInternal.h"
#include "oiContext.h"
#include "oiVolume.h"

#include "OpenVDBImporter.h"

#include <tbb/enumerable_thread_specific.h>
#include <tbb/parallel_for.h>

#include <openvdb/tools/Interpolation.h>

template <typename RealType>
struct ValueRange {
public:
    ValueRange()
            : m_min(std::numeric_limits<RealType>::max())
            , m_max(std::numeric_limits<RealType>::min())
    {}
    ValueRange(RealType min_, RealType max_)
            : m_min(min_)
            , m_max(max_)
    {}

    RealType getMin() const { return m_min; }
    RealType getMax() const { return m_max; }

    void addValue(RealType value)
    {
        m_min = std::min(m_min, value);
        m_max = std::max(m_max, value);
    }

private:
    RealType m_min, m_max;
};

typedef ValueRange<float> FloatRange;

enum class FilterMode { BOX, MULTIRES, AUTO };

inline openvdb::CoordBBox getIndexSpaceBoundingBox(const openvdb::GridBase& grid)
{
    try
    {
        const auto file_bbox_min = openvdb::Coord(grid.metaValue<openvdb::Vec3i>("file_bbox_min"));
        if (file_bbox_min.x() == std::numeric_limits<int>::max() ||
            file_bbox_min.y() == std::numeric_limits<int>::max() ||
            file_bbox_min.z() == std::numeric_limits<int>::max()) {
            return {};
        }
        const auto file_bbox_max = openvdb::Coord(grid.metaValue<openvdb::Vec3i>("file_bbox_max"));

        if (file_bbox_max.x() == std::numeric_limits<int>::min() ||
            file_bbox_max.y() == std::numeric_limits<int>::min() ||
            file_bbox_max.z() == std::numeric_limits<int>::min()) {
            return {};
        }

        return { file_bbox_min, file_bbox_max };
    }
    catch (openvdb::Exception e)
    {
        return {};
    }
}

template <typename SamplingFunc, typename RealType>
bool sampleVolume( const openvdb::Coord& extents, SamplingFunc sampling_func, RealType* out_samples)
{
    const auto domain = openvdb::CoordBBox(openvdb::Coord(0, 0, 0), extents - openvdb::Coord(1, 1, 1));
    if (domain.empty())
    {
        return false;
    }

    // Sample on a lattice.
    typedef tbb::enumerable_thread_specific<FloatRange> PerThreadRange;
    PerThreadRange ranges;
    const openvdb::Vec3i stride = {1, extents.x(), extents.x() * extents.y()};
    tbb::atomic<bool> cancelled;
    cancelled = false;
    tbb::parallel_for(
            domain,
            [&sampling_func, &stride, &ranges, out_samples, &cancelled] (const openvdb::CoordBBox& bbox)
            {
                const auto local_extents = bbox.extents();

                // Loop through local bbox.
                PerThreadRange::reference this_thread_range = ranges.local();
                for (auto z = bbox.min().z(); z <= bbox.max().z(); ++z)
                {
                    for (auto y = bbox.min().y(); y <= bbox.max().y(); ++y)
                    {
                        for (auto x = bbox.min().x(); x <= bbox.max().x(); ++x)
                        {
                            const auto domain_index = openvdb::Vec3i(x, y, z);
                            const auto linear_index = domain_index.dot(stride) * 4;
                            const auto sample_value = sampling_func(domain_index);

                            // fixme
                            out_samples[linear_index + 0] = sample_value;
                            out_samples[linear_index + 1] = sample_value;
                            out_samples[linear_index + 2] = sample_value;
                            out_samples[linear_index + 3] = sample_value;
                            this_thread_range.addValue(sample_value);
                        }
                    }
                }
            });
}

template <typename RealType>
bool sampleGrid(
        const openvdb::FloatGrid& grid,
        const openvdb::Coord& sampling_extents,
        RealType* out_data)
{
    assert(out_data);

    const auto grid_bbox_is = getIndexSpaceBoundingBox(grid);
    const auto bbox_world = grid.transform().indexToWorld(grid_bbox_is);

    // Return if the grid bbox is empty.
    if (grid_bbox_is.empty())
    {
        return false;
    }

    const auto domain_extents = sampling_extents.asVec3d();
    openvdb::tools::GridSampler<openvdb::FloatGrid, openvdb::tools::BoxSampler> sampler(grid);

    auto sampling_func = [&sampler, &bbox_world, &domain_extents] (const openvdb::Vec3d& domain_index) -> RealType
    {
        const auto sample_pos_ws = bbox_world.min() + (domain_index + 0.5) / domain_extents * bbox_world.extents();
        return sampler.wsSample(sample_pos_ws);
    };

    sampleVolume( sampling_extents, sampling_func, out_data);
}

oiVolume::oiVolume(const openvdb::FloatGrid& grid, const openvdb::Coord& extents)
    : m_grid(grid), m_extents(extents)
{
    grid.print();

    int voxel_count = extents.x() * extents.y() * extents.z();
    int texture_format = 5;
    m_summary = new oiVolumeSummary(voxel_count, extents.x(), extents.y(), extents.z(), texture_format);
}

oiVolume::~oiVolume()
{
}

void oiVolume::reset()
{
}

void oiVolume::fillTextureBuffer(oiVolumeData& data) const
{
    DebugLog("oiVolume::fillTextureBuffer start");

    if(!data.voxels)
    {
        DebugLog("oiVolume::fillTextureBuffer voxels pointer is null");
        return;
    }

    openvdb::Coord extents{m_summary->width, m_summary->height, m_summary->depth};
    sampleGrid(m_grid, extents, (char*)data.voxels);
}

const oiVolumeSummary& oiVolume::getSummary() const
{
    return *m_summary;
}