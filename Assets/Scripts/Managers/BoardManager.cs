using Cysharp.Threading.Tasks;
using Game.Core;
using UnityEngine;

namespace Managers
{
    public class BoardManager
    {
        private readonly BoardGenerator _boardGenerator;
        public BoardManager(BoardGenerator generator)
        {
            _boardGenerator = generator;
        }
        
        public bool IsFull()
        {
            foreach (ITile t in _boardGenerator.tiles)
            {
                if (t.GetCurrentPiece() == null) return false;
            }
            return true;
        }

        public async UniTask PlacePiece(PieceData data)
        {
            Vector3 loc = data.Piece.GetCurrentPosition();
            var tile = _boardGenerator.GetTile((int)loc.x, (int)loc.y);
            await tile.SetCurrentPiece(data.Piece);
        }


        //Check if the game is over, returns true if it is, and creates an array of all the winning players.
        public bool IsGameOver(ref IPlayer[] winners)
        {
            return false;
        }
    }
}