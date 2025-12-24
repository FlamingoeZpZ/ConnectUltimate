using UnityEngine;

namespace ScriptableObjects
{
    public interface IConfiguration
    {
        public void InitializeConfig(ScriptableObject configData);
    }
}