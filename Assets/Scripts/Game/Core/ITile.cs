using Cysharp.Threading.Tasks;

namespace Game.Core
{
    public interface ITile
    {
        //Pending Implementation
        public UniTaskVoid SetCurrentPiece(IPiece newPiece);
        public IPiece GetCurrentPiece();
    }
}