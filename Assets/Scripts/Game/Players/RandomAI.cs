using Cysharp.Threading.Tasks;
using Game.Pieces;

namespace Game.Players
{
    public class RandomAI : IPlayer
    {
        public async UniTask<PieceData> PlacePiece()
        {
            return new PieceData();
        }
        public void SetPlayerInformation(PlayerInformation info)
        {
            
        }
        public PlayerInformation GetPlayerInformation()
        {
            return new PlayerInformation();
        }
    }
}