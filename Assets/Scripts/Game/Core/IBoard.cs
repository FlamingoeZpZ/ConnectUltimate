using Cysharp.Threading.Tasks;

namespace Game.Core
{
    public interface IBoard
    {
        public UniTask PlacePiece(PieceData data);
    }
}