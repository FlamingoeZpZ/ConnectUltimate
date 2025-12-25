using System;
using Game.Core;
using UnityEngine;

namespace ScriptableObjects
{

    [CreateAssetMenu(fileName = "Effects Configuration", menuName = "Connect4/Effects Configuration", order = 0)]
    public class EffectsConfiguration : ScriptableObject,IConfigObject
    {
        [field: SerializeField, Range(0, 1)] public float tileAnimationTime { get; private set; } = 0.2f;
        
        public event Action OnUpdated;
    }
}
