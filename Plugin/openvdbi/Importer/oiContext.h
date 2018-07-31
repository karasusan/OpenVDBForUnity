#pragma once

class oiVolume;
class oiConfig;

class oiContext
{
public:
    explicit oiContext(int uid=-1);
    ~oiContext();

    bool load(const char *path);
    void setConfig(const oiConfig& config);
    const std::string& getPath() const;

    oiVolume* getVolume() const;
    int getUid() const;

private:
    int m_uid = 0;

    void reset();

    std::string m_path;
    openvdb::io::Archive* m_archive;
    openvdb::FloatGrid::ConstPtr m_grid;
    openvdb::Coord m_extents = {256, 256, 256};

    oiVolume* m_volume;
    oiConfig m_config;
};

class oiContextManager
{
public:
    static oiContext* getContext(int uid);
    static void destroyContext(int uid);

private:
    ~oiContextManager();

    using ContextPtr = std::unique_ptr<oiContext>;
    std::map<int, ContextPtr> m_contexts;
    static oiContextManager s_instance;
};