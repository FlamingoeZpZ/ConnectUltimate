using Cysharp.Threading.Tasks;
using Game.Players;
using Scriptable_Objects;
using UnityEngine;

namespace Managers
{
    [DefaultExecutionOrder(-1000)]
    public class PlayerManager : MonoBehaviour, IConfiguration
    {
        private PlayerConfiguration _playerConfig;
        
        [SerializeField] private Material[] sharedPlayerMaterials;
        [SerializeField] private Human humanPlayerPrefab;

        private IPlayer[] _players;
        private int _currentPlayer;

        public IPlayer currentPlayer => _players[_currentPlayer];

        public static PlayerManager instance { get; private set; }

        private void Awake()
        {
            if (instance && instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        public void CreatePlayers()
        {
            Debug.LogWarning("The CreatePlayers() function likely needs work, 2025-12-24", gameObject);
            
            int n = _playerConfig.numPlayers;
            _players = new IPlayer[n];

            for (int i = 0; i < n; i++)
            {
                PlayerInformation playerInfo = new PlayerInformation(i, sharedPlayerMaterials[i]);

                if (i < _playerConfig.numAI)
                {
                    _players[i] = new RandomAI();
                }
                else
                {
                   var human = Instantiate(humanPlayerPrefab, transform);
                   human.name = "Player: " + i;
                   _players[i] = human;
                }

                _players[i].SetPlayerInformation(playerInfo);
            }
        }

        public void InitializeConfig(ScriptableObject configData)
        {
            if (configData is PlayerConfiguration data)
            {
                _playerConfig = data;
            }
            else
            {
                Debug.LogError("In valid data", gameObject);
            }
        }

        public async UniTask ChooseFirstPlayer(int offset)
        {
            //This function should do an animation or wait for it or something...
            _currentPlayer = _playerConfig.randomStartingPlayer? Random.Range(0, _playerConfig.numPlayers) : (_playerConfig.startingPlayer + offset) % _playerConfig.numPlayers;
        }
        
        public async UniTask ChooseNextPlayer()
        {
            _currentPlayer = (_currentPlayer + 1) %  _playerConfig.numPlayers;
        }
    }
}