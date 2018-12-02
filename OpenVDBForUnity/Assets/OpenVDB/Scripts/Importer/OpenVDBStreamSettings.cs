using UnityEngine;

namespace OpenVDB
{
    [System.Serializable]
    public class OpenVDBStreamSettings
    {
        // Meshes
        [SerializeField] public float scaleFactor = 0.01f;
        
        // Material
        [SerializeField] public bool importMaterials = true;
        
        [SerializeField] public bool extractTextures = false;
        
        [SerializeField] public bool extractMaterials = false;
        
        [SerializeField] public Texture[] textures = new Texture[0];
        
        [SerializeField] public Material[] materials = new Material[0];

        [SerializeField] public int textureMaxSize = 256;
    }
}