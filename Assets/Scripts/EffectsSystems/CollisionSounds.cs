using UnityEngine;

namespace EffectsSystems
{
    [RequireComponent(typeof(Collider2D))]
    public class CollisionSounds : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D other)
        {
            SoundEffectManager.instance.ChooseAndPlaySoundEffect(other.gameObject);
        }
    }
}
