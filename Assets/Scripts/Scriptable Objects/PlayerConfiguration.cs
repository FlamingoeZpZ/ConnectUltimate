using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Player Configuration", menuName = "Connect4/Player Configuration", order = 0)]
    public class PlayerConfiguration : ScriptableObject
    {
        [field: SerializeField, Range(1, 4)] public int numPlayers { get; private set; } = 2;
        [field: SerializeField, Range(0, 3)] public int numAI { get; private set; } = 4;

        [field: SerializeField, Range(1, 4)] public int startingPlayer { get; private set; }
        [field: SerializeField] public bool randomStartingPlayer { get; private set; } = true;
    }
}
