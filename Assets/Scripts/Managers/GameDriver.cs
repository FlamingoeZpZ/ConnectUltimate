using Cysharp.Threading.Tasks;
using Game.Core;
using ScriptableObjects;
using UnityEngine;

namespace Managers{
    
    [RequireComponent(typeof(BoardGenerator), typeof(PlayerManager)), DefaultExecutionOrder(1000)]
    public class GameDriver : MonoBehaviour
    {
        private GameConfiguration _gameConfig;

        private PlayerManager _playerManager;
        private BoardManager _boardManager;
        private BoardGenerator _boardGenerator;

        private int _numGamesPlayed;

        public int numGamesPlayed => _numGamesPlayed;

        private void Start()
        {
            _playerManager = GetComponent<PlayerManager>();
            _boardGenerator = GetComponent<BoardGenerator>();
            
            _gameConfig = Settings.gameConfiguration;
            
            
            ResetGameplayLoop();
        }

        [ContextMenu("ResetGameplayLoop")]
        public void ResetGameplayLoop()
        {
            _boardManager = new(_boardGenerator);
            ControlManager.SetCurrentTurn(null);
            _ = HandleGameLoop();
        }

        public async UniTask HandleGameLoop()
        {
            //Choose the player...
            _playerManager.CreatePlayers();
            await _playerManager.ChooseFirstPlayer(_numGamesPlayed);

            IPlayer[] winners = null;

            //Check is the board full? If it is exit the loop
            while (!_boardManager.IsFull())
            {
                await DisplayCurrentPlayerTurn(_playerManager.currentPlayer);
                
                
                //Wait for the player to place a piece...
                PieceData placementData;
                do
                {
                    ControlManager.SetCurrentTurn(_playerManager.currentPlayer);
                    
                    var currentCachedPiece = await _playerManager.currentPlayer.PlacePiece();
                    
                    ControlManager.DisableGameControls();
                    
                    placementData = await currentCachedPiece.DropPieceLoop();
                    
                    if (!WasCurrentMoveAccepted(placementData))
                    {
                        placementData.Piece.Remove();
                        InvalidPieceEffect(placementData).Forget();
                    }

                    Debug.Log($"Piece Placement Type: {placementData.PlacementType}, was the move accepted? {WasCurrentMoveAccepted(placementData)}");
                    
                } while (!WasCurrentMoveAccepted(placementData));

                if (placementData.PlacementType == EPlacementType.Success)
                {
                    _boardManager.PlacePiece(placementData);
                    await placementData.Tile.SetCurrentPiece(placementData.Piece);
                    PlacePieceEffect(placementData).Forget();
                }
                else
                {
                    MissedPieceEffect(placementData).Forget();
                }
                
               
                //Check has the game ended? If it has exit the gameplay loop
                if (_boardManager.IsGameOver(ref winners)) break;

                //Get the next player... 
                _playerManager.ChooseNextPlayer();
            }

            //End the game... Who won?
            if (winners != null && winners.Length != 0) await AwardWin(winners);

            else await HandleTie();
        }

        //Any (7, 111) & Success (4, 100) != 0 (100) was successfully placed = true, move to next turn.
        private bool WasCurrentMoveAccepted(PieceData placementData) => (_gameConfig.moveToNextTurnWhen & placementData.PlacementType) != 0;

        #region Animation Data FIX THIS
        private async UniTask DisplayCurrentPlayerTurn(IPlayer playerManagerCurrentPlayer)
        {
            Debug.LogWarning("Implement DisplayCurrentPlayerTurn effect, playing an animation");

        }
        
        private async UniTaskVoid PlacePieceEffect(PieceData placementData)
        {
            Debug.LogWarning("Implement PlacePieceEffect effect, playing an animation");
        }
        private async UniTaskVoid InvalidPieceEffect(PieceData placementData)
        {
            Debug.LogWarning("Implement InvalidPieceEffect effect, playing an animation");
        }
        private async UniTaskVoid MissedPieceEffect(PieceData placementData)
        {
            Debug.LogWarning("Implement MissedPieceEffect effect, playing an animation");
        }

        private async UniTask HandleTie()
        {
            Debug.Log("GAME ENDED WITH A TIE");
        }

        private async UniTask AwardWin(IPlayer[] winner)
        {
            Debug.Log("SOMEONE HAS WON");
            foreach (var x in winner)
            {
                Debug.Log("SOMEONE HAS WON: " + x.GetPlayerInformation().TeamNumber + " --> " +
                          (x as MonoBehaviour)?.name);
            }
        }
        #endregion
    }
}
