using Cysharp.Threading.Tasks;
using Game.Core;

namespace Game.Players
{
    public class RandomAI : IPlayer
    {
        public async UniTask<IPiece> PlacePiece()
        {
            return null;
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