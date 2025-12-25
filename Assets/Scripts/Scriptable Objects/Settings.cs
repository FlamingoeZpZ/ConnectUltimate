using System;
using UnityEngine;

namespace ScriptableObjects
{
    [DefaultExecutionOrder(-1000)]
    public class Settings : MonoBehaviour
    {
        [SerializeField] private GameConfiguration gameConfig;
        [SerializeField] private BoardConfiguration boardConfig;
        [SerializeField] private PlayerConfiguration playerConfig;
        [SerializeField] private EffectsConfiguration effectsConfig;
        [SerializeField] private AudioConfiguration audioConfig;

        public static GameConfiguration gameConfiguration => _instance.gameConfig;
        public static BoardConfiguration boardConfiguration => _instance.boardConfig;
        public static PlayerConfiguration playerConfiguration => _instance.playerConfig;
        public static EffectsConfiguration effectsConfiguration =>_instance.effectsConfig;
        public static AudioConfiguration audioConfiguration => _instance.audioConfig;
        
        private static Settings _instance;
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}