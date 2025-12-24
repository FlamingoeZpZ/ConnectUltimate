using UnityEngine;

namespace ScriptableObjects
{

    [CreateAssetMenu(fileName = "Board Configuration", menuName = "Connect4/Board Configuration", order = 0)]
    public class BoardConfiguration : ScriptableObject
    {
        [field: SerializeField, Range(3, 12)] public int numCols { get; private set; } = 7;
        [field: SerializeField, Range(3, 12)] public int numRows { get; private set; } = 6;
        [field: SerializeField, Range(3, 12)] public int numConnect { get; private set; } = 4;

        [Header("Weirdo stuff")]
        [field: SerializeField]
        public bool useGravity { get; private set; }

        [field: SerializeField] public bool useSpinning { get; private set; }

        [Header("Plinko Mode")]
        [field: SerializeField]
        public bool usePlinko { get; private set; }

        [field: SerializeField] public bool skipTurnIfFailedToSettle { get; private set; } = true;

        [field: SerializeField, Range(10, 120)]
        public float coinSettleTime { get; private set; } = 30;
    }
}
