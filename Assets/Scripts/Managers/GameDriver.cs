using System;
using Cysharp.Threading.Tasks;
using Game.Core;
using ScriptableObjects;
using UnityEngine;

namespace Managers{
    
    [RequireComponent(typeof(BoardGenerator), typeof(PlayerManager), typeof(DisplayManager)), DefaultExecutionOrder(1000)]
    public class GameDriver : MonoBehaviour
    {
        private GameConfiguration _gameConfig;

        private PlayerManager _playerManager;
        private BoardManager _boardManager;
        private BoardGenerator _boardGenerator;
        private DisplayManager _displayManager;

        private int _numGamesPlayed;

        public int numGamesPlayed => _numGamesPlayed;

        public static event Action onGameRestarted;

        private void Start()
        {
            _playerManager = GetComponent<PlayerManager>();
            _boardGenerator = GetComponent<BoardGenerator>();
            _displayManager = GetComponent<DisplayManager>();
            
            _gameConfig = Settings.gameConfiguration;
            
            
            ResetGameplayLoop();
        }

        [ContextMenu("ResetGameplayLoop")]
        public void ResetGameplayLoop()
        {
            onGameRestarted?.Invoke();

            _boardManager = new(_boardGenerator);
            _boardGenerator.InitializeConfig();
            ControlManager.SetCurrentTurn(null);
            _ = HandleGameLoop();
            Debug.Log("Game has been reset!");

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
                await _displayManager.DisplayCurrentPlayerTurn(_playerManager.currentPlayer);
                
                
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
                        _displayManager.InvalidPieceEffect(placementData).Forget();
                    }

                    Debug.Log($"Piece Placement Type: {placementData.PlacementType}, was the move accepted? {WasCurrentMoveAccepted(placementData)}");
                    
                } while (!WasCurrentMoveAccepted(placementData));

                if (placementData.PlacementType == EPlacementType.Success)
                {
                    _boardManager.PlacePiece(placementData);
                    await placementData.Tile.SetCurrentPiece(placementData.Piece);
                    _displayManager.PlacePieceEffect(placementData).Forget();
                }
                else
                {
                    _displayManager.MissedPieceEffect(placementData).Forget();
                }
                
               
                //Check has the game ended? If it has exit the gameplay loop
                if (_boardManager.IsGameOver(ref winners)) break;

                //Get the next player... 
                _playerManager.ChooseNextPlayer();
            }

            //End the game... Who won?
            if (winners != null && winners.Length != 0) await _displayManager.AwardWin(winners);
            else await _displayManager.HandleTie();
            
            ResetGameplayLoop();
        }

        //Any (7, 111) & Success (4, 100) != 0 (100) was successfully placed = true, move to next turn.
        private bool WasCurrentMoveAccepted(PieceData placementData) => (_gameConfig.moveToNextTurnWhen & placementData.PlacementType) != 0;
    }
}
