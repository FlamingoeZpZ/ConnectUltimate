using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Core
{
    public interface IPlayer
    {
        public UniTask<IPiece> PlacePiece();
        public void SetPlayerInformation(PlayerInformation info);
        public PlayerInformation GetPlayerInformation();
    }

    public struct PlayerInformation
    {
        public readonly int TeamNumber;
        public readonly Material SharedPlayerMaterial;

        public PlayerInformation(int teamNumber, Material sharedPlayerMaterial)
        {
            TeamNumber = teamNumber;
            SharedPlayerMaterial = sharedPlayerMaterial;
        }
    }

}
    