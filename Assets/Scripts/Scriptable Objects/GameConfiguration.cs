using System;
using Game.Core;
using UnityEngine;

namespace ScriptableObjects
{

    [CreateAssetMenu(fileName = "Game Configuration", menuName = "Connect4/Game Configuration", order = 0)]
    public class GameConfiguration : ScriptableObject,IConfigObject
    {
        [Header("Weirdo stuff")] 
        [field: SerializeField] public bool placeAnywhere { get; private set; }
        [field: SerializeField] public bool useSpinning { get; private set; }

        [Header("Plinko Mode")]
        [field: SerializeField] public bool usePlinko { get; private set; }
        [field: SerializeField] public bool bouncyWalls { get; private set; }

        [field: SerializeField] public float gravityScale { get; private set; } = 3;
        
        [field: SerializeField] public EPlacementType moveToNextTurnWhen { get; private set; } = EPlacementType.Success;
        [field: SerializeField, Range(10, 120)] public float coinSettleTime { get; private set; } = 30;
        public event Action OnUpdated;
    }
}
