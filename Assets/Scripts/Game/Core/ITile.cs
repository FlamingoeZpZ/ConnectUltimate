using Cysharp.Threading.Tasks;

namespace Game.Core
{
    public interface ITile
    {
        //Pending Implementation
        public UniTask SetCurrentPiece(IPiece newPiece);
        public IPiece GetCurrentPiece();
    }
}