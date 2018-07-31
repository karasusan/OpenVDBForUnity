#pragma once

class oiContext;
struct oiVolumeData;
struct oiVolumeSummary;

class oiVolume
{
public:
    oiVolume(const openvdb::FloatGrid& grid, const openvdb::Coord& extents);
    virtual ~oiVolume();

    void reset();
    void setScaleFactor(float scaleFactor);
    void fillTextureBuffer(oiVolumeData& data) const;
    const oiVolumeSummary& getSummary() const;

    //IArray<openvdbV4> m_values_ref;
    //RawVector<openvdbV4 > m_values;
private:
    oiVolumeSummary* m_summary;
    float m_scaleFactor;
    const openvdb::FloatGrid& m_grid;
    const openvdb::Coord& m_extents;
};