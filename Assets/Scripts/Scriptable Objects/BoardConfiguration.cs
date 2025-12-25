using System;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Board Configuration", menuName = "Connect4/Board Configuration", order = 0)]
    public class BoardConfiguration : ScriptableObject, IConfigObject
    {
        [field: SerializeField, Range(3, 12)] public int numCols { get; private set; } = 7;
        [field: SerializeField, Range(3, 12)] public int numRows { get; private set; } = 6;
        [field: SerializeField, Range(3, 12)] public int numConnect { get; private set; } = 4;

        [NonSerialized] public float RunTimeTileSize;
        public event Action OnUpdated;
    }
}
