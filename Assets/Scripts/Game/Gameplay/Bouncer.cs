using UnityEngine;

namespace Game.Gameplay
{
    public class Bouncer : MonoBehaviour
    {
        private static readonly int Bounce = Animator.StringToHash("Bounce");
        [SerializeField] private float addForce;
        [SerializeField] private float cooldownTime;

        private Animator _animator;
        
        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            Vector2 forceDirection = Vector2.zero;
            foreach (var contact in other.contacts)
            {
                forceDirection -=  contact.normal;
            }
            forceDirection /= other.contacts.Length;
            other.rigidbody.AddForce(forceDirection * addForce, ForceMode2D.Impulse);
            _animator?.SetTrigger(Bounce);
        }
    }
}
