using System;
using Cysharp.Threading.Tasks;
using Game.Core;
using Game.Players;
using ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class PlayerManager : MonoBehaviour
    {
        private PlayerConfiguration _playerConfig;
        
        [SerializeField] private Material[] sharedPlayerMaterials;
        [SerializeField] private GameObject humanPlayerPrefab;

        private IPlayer[] _players;
        private int _currentPlayer;

        public IPlayer currentPlayer => _players[_currentPlayer];

        private void Awake()
        {
            _playerConfig = Settings.playerConfiguration;
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
                   _players[i] = human.GetComponent<IPlayer>();
                }

                _players[i].SetPlayerInformation(playerInfo);
            }
        }

        public async UniTask ChooseFirstPlayer(int offset)
        {
            //This function should do an animation or wait for it or something...
            _currentPlayer = _playerConfig.randomStartingPlayer? Random.Range(0, _playerConfig.numPlayers) : (_playerConfig.startingPlayer + offset) % _playerConfig.numPlayers;
        }
        
        public void ChooseNextPlayer()
        {
            _currentPlayer = (_currentPlayer + 1) %  _playerConfig.numPlayers;
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if(humanPlayerPrefab == null || !humanPlayerPrefab.TryGetComponent(out IPlayer _))
                Debug.LogError("There is an issue with the current selected player prefab: ", gameObject);
            
        }
#endif
    }
}