using Cysharp.Threading.Tasks;
using Game;
using Managers;
using UnityEngine;

/// <summary>
/// Dependency Inversion...
/// </summary>
public class GameDriver : MonoBehaviour
{
    [SerializeField] private PlayerConfiguration playerConfig;
    [SerializeField] private BoardConfiguration boardConfig;

    private PlayerManager _playerManager;
    private BoardManager _boardManager;
    
    private int _numGamesPlayed;
    
    public int NumGamesPlayed => _numGamesPlayed;


    private void Start()
    {
        ResetGameplayLoop();
    }

    [ContextMenu("ResetGameplayLoop")]
    public void ResetGameplayLoop()
    {
        _playerManager = PlayerManager.instance;
        _boardManager = BoardManager.instance;
        
        _playerManager.InitializeConfig(playerConfig);
        _boardManager.InitializeConfig(boardConfig);
        
        
        _ = HandleGameLoop();
    }

    public async UniTask HandleGameLoop()
    {
        //Choose the player...
        _playerManager.CreatePlayers();
        await _playerManager.ChooseFirstPlayer(_numGamesPlayed);
        
        IPlayer[] winners = null;
        
        while (!_boardManager.IsFull()) // Force us to enter...?
        {
            //Wait for the player to place a piece...
            Vector2Int chosenLocation = await _playerManager.currentPlayer.PlacePiece();

            //Place the piece in the desired location, and if possible complete the animation
            await _boardManager.PlacePiece(_playerManager.currentPlayer, chosenLocation.x, chosenLocation.y);

            if (_boardManager.IsGameOver(ref winners)) break;
            
            //Get the next player... Do an animation if applicable...
            await _playerManager.ChooseNextPlayer();
        }
        
        //End the game... Who won?
        if (winners != null && winners.Length != 0) await AwardWin(winners);
        
        else await HandleTie();
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
            Debug.Log("SOMEONE HAS WON: " + x.GetPlayerInformation().TeamNumber + " --> " + (x as MonoBehaviour)?.name);
        }
    }
}
