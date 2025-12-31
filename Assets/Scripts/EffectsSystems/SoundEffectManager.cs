using UnityEngine;
using UnityEngine.Audio;

namespace EffectsSystems
{

    [RequireComponent(typeof(AudioSource))]
    public class SoundEffectManager : MonoBehaviour
    {
        public static SoundEffectManager instance { get; private set; }
        private AudioSource _source;

        [Header("Collision Sounds")]
        [SerializeField] private AudioResource hitBoardSound;
        [SerializeField] private AudioResource hitBouncerSound;

        private void Awake()
        {
            if (instance && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            _source = GetComponent<AudioSource>();
        }

        public void PlayBoardHitSound()
        {
            _source.resource = hitBoardSound;
            _source.Play();
        }

        public void PlayBouncerSound()
        { 
            _source.resource = hitBouncerSound;
            _source.Play();
        }

        public void ChooseAndPlaySoundEffect(GameObject targetTag)
        {
            if(targetTag.CompareTag("Board"))
                    PlayBoardHitSound();
            else if(targetTag.CompareTag("Bouncer"))
                PlayBouncerSound();
        }
    }
}