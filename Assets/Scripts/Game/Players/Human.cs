
using Cysharp.Threading.Tasks;
using Game.Pieces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Players
{
    public class Human : MonoBehaviour, IPlayer
    {
        private PiecePlacer _piecePlacer;
        private PlayerInput _playerInput;
        
        private Piece.EPlacementType  _currentPieceType;
        
        private void Awake()
        {
            _piecePlacer = GetComponent<PiecePlacer>();
            _piecePlacer.SetOwner(this);

            BindControls();
        }
        #region Controls
        private void BindControls()
        {
            _playerInput = GetComponent<PlayerInput>();
            _playerInput.actions["TapRelease"].started  += PlacePiecePlayer;
            _playerInput.actions["TapRelease"].canceled += TryDropPiecePlayer;
            _playerInput.actions["MousePos"].performed  += HandlePiecePlayer;
        }

        private void OnDestroy()
        {
            _playerInput.actions["TapRelease"].started  -= PlacePiecePlayer;
            _playerInput.actions["TapRelease"].canceled -= TryDropPiecePlayer;
            _playerInput.actions["MousePos"].performed  -= HandlePiecePlayer;
        }
        private void TryDropPiecePlayer(InputAction.CallbackContext _) => _piecePlacer.ReleasePiece();
        private void HandlePiecePlayer(InputAction.CallbackContext obj) => _piecePlacer.HandlePiece(obj.ReadValue<Vector2>());
        private void PlacePiecePlayer(InputAction.CallbackContext obj) => _piecePlacer.CreateChip(obj.ReadValue<Vector2>());

        #endregion
        
        public async UniTask<PieceData> PlacePiece()
        {
            _currentPieceType = Piece.EPlacementType.None;
            
            _playerInput.actions.Enable();
            await UniTask.WaitUntil(_piecePlacer.HasActivePiece); //First wait until a piece actually spawns
            
            Piece currentCachedPiece =  _piecePlacer.currentPiece;
            currentCachedPiece.OnPieceSettled += OnMyPieceSettled;
            
            await UniTask.WaitWhile(_piecePlacer.HasActivePiece); //Then wait until the piece is dropped...
            
            _playerInput.actions.Disable(); 
            
            await UniTask.WaitWhile(IsCurrentPieceActive); // Finally, wait until the piece has settled.
            
            currentCachedPiece.OnPieceSettled -= OnMyPieceSettled; // Unsub from this piece, we don't care anymore...

            return new PieceData(_currentPieceType, currentCachedPiece);
        }

        private bool IsCurrentPieceActive() => _currentPieceType == Piece.EPlacementType.None;

        private void OnMyPieceSettled(Piece.EPlacementType obj)
        {
            
        }

        public void SetPlayerInformation(PlayerInformation info)
        {
            throw new System.NotImplementedException();
        }


        public PlayerInformation GetPlayerInformation()
        {
            throw new System.NotImplementedException();
        }
    }
}