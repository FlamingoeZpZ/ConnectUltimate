using Cysharp.Threading.Tasks;
using Game.Core;
using UnityEngine;

namespace Game.Tiles
{
    public class Tile : MonoBehaviour, ITile
    {
        private IPiece _currentPiece;
        
        public async UniTaskVoid SetCurrentPiece(IPiece newPiece)
        {
            _currentPiece = newPiece;
            
            Debug.LogWarning("Implement functionality.", gameObject);
        }

        public IPiece GetCurrentPiece() => _currentPiece;
    }
}