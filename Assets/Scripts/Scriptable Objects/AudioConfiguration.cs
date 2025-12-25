using System;
using UnityEngine;

namespace ScriptableObjects
{

    [CreateAssetMenu(fileName = "Audio Configuration", menuName = "Connect4/Audio Configuration", order = 0)]
    public class AudioConfiguration : ScriptableObject, IConfigObject
    {
        public event Action OnUpdated;
    }
}
