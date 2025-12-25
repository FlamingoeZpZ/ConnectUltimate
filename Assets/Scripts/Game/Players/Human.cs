using Cysharp.Threading.Tasks;
using Game.Core;
using Game.Pieces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Players
{
    public class Human : MonoBehaviour, IPlayer
    {
        private PiecePlacer _piecePlacer;
        private PlayerInput _playerInput;
        
        
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
            
            _playerInput.actions.Disable();
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
            _playerInput.actions.Enable();
            
            Debug.Log("Waiting for player to start placing");
            
            await UniTask.WaitUntil(_piecePlacer.HasActivePiece); //First wait until a piece actually spawns
            
            Debug.Log("Waiting for player to drop the piece");

            
            IPiece currentCachedPiece =  _piecePlacer.currentPiece;
            await UniTask.WaitWhile(_piecePlacer.HasActivePiece); //Then wait until the piece is dropped...
            
            _playerInput.actions.Disable(); 
            
            Debug.Log("Waiting for piece to fall asleep");
            return await currentCachedPiece.DropPieceLoop(); //
        }

        public void SetPlayerInformation(PlayerInformation info)
        {
            Debug.LogWarning("SetPlayerInformation does nothing right now!", gameObject);
            //throw new System.NotImplementedException();
        }


        public PlayerInformation GetPlayerInformation()
        {
            throw new System.NotImplementedException();
        }
    }
}