using Cysharp.Threading.Tasks;
using Game.Core;
using Game.Pieces;
using UnityEngine;

namespace Game.Players
{
    public class Human : MonoBehaviour, IPlayer
    {
        private PiecePlacer _piecePlacer;
        private PlayerInformation _playerInformation;
        
        private void Awake()
        {
            _piecePlacer = GetComponent<PiecePlacer>();
            _piecePlacer.SetOwner(this);
        }
        #region Controls
        public void TryDropPiecePlayer() => _piecePlacer.ReleasePiece();
        public void HandlePiecePlayer(Vector2 loc) => _piecePlacer.HandlePiece(loc);
        public void PlacePiecePlayer(Vector2 loc) => _piecePlacer.CreateChip(loc);

        #endregion
        
        public async UniTask<IPiece> PlacePiece()
        {
            
            Debug.Log("Waiting for player to start placing");
            
            await UniTask.WaitUntil(_piecePlacer.HasActivePiece); //First wait until a piece actually spawns
            
            Debug.Log("Waiting for player to drop the piece");

            
            IPiece currentCachedPiece =  _piecePlacer.currentPiece;
            await UniTask.WaitWhile(_piecePlacer.HasActivePiece); //Then wait until the piece is dropped...
            
            Debug.Log("Waiting for piece to fall asleep");
            return currentCachedPiece;
        }

        public void SetPlayerInformation(PlayerInformation info)
        {
            _playerInformation = info;
        }


        public PlayerInformation GetPlayerInformation() => _playerInformation;
    }
}