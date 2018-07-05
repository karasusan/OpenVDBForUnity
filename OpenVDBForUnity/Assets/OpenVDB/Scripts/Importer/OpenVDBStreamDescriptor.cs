using UnityEngine;

namespace OpenVDB
{
    public class OpenVDBStreamDescriptor : ScriptableObject
    {
        [SerializeField] public string pathToVDB;
        [SerializeField] public OpenVDBStreamSettings settings = new OpenVDBStreamSettings();
    }
}
