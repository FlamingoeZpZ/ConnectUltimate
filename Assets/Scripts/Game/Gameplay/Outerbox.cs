using UnityEngine;
using System.Collections;
using Game.Core;
using ScriptableObjects;

namespace Game.Gameplay
{
    public class Outerbox : MonoBehaviour
    {
        [SerializeField] private ParticleSystem destroyParticles;
        [SerializeField] private ParticleSystem hitParticles;
        [SerializeField] private float borderThickness = 1f;
        [SerializeField] private float edgeOffset = 0.5f; // How far outside the screen to place walls
        
        private Camera _mainCamera;
        private GameObject topWall, bottomWall, leftWall, rightWall;
        
        private void Awake()
        {
            _mainCamera = Camera.main;
            SetupBorder();
        }

        private void SetupBorder()
        {
            if (_mainCamera == null)
            {
                Debug.LogError("Main camera not found!");
                return;
            }

            // Get the screen bounds in world space
            float height = _mainCamera.orthographicSize * 2f;
            float width = height * _mainCamera.aspect;
            
            Vector3 cameraPos = _mainCamera.transform.position;

            // Create four walls positioned just outside the visible screen area
            topWall = CreateWall("TopWall", 
                new Vector3(cameraPos.x, cameraPos.y + height/2 + edgeOffset, cameraPos.z),
                new Vector2(width + borderThickness * 2, borderThickness));
            
            bottomWall = CreateWall("BottomWall", 
                new Vector3(cameraPos.x, cameraPos.y - height/2 - edgeOffset, cameraPos.z),
                new Vector2(width + borderThickness * 2, borderThickness));
            
            leftWall = CreateWall("LeftWall", 
                new Vector3(cameraPos.x - width/2 - edgeOffset, cameraPos.y, cameraPos.z),
                new Vector2(borderThickness, height + borderThickness * 2));
            
            rightWall = CreateWall("RightWall", 
                new Vector3(cameraPos.x + width/2 + edgeOffset, cameraPos.y, cameraPos.z),
                new Vector2(borderThickness, height + borderThickness * 2));
        }

        private GameObject CreateWall(string name, Vector3 position, Vector2 size)
        {
            GameObject wall = new GameObject(name);
            wall.transform.parent = transform;
            wall.transform.position = position;
            
            BoxCollider2D collider = wall.AddComponent<BoxCollider2D>();
            collider.size = size;
            
            if (Settings.gameConfiguration.bouncyWalls)
            {
                // Solid collider for bouncing
                collider.isTrigger = false;
                
                // Add a component to handle particles on collision
                WallCollisionHandler handler = wall.AddComponent<WallCollisionHandler>();
                handler.Initialize(hitParticles);
            }
            else
            {
                // Trigger collider for destruction
                collider.isTrigger = true;
                
                // Add a component to handle destruction
                WallDestructionHandler handler = wall.AddComponent<WallDestructionHandler>();
                handler.Initialize(destroyParticles);
            }
            
            return wall;
        }

        // Helper component for bouncy walls
        private class WallCollisionHandler : MonoBehaviour
        {
            private ParticleSystem hitParticles;
            
            public void Initialize(ParticleSystem particles)
            {
                hitParticles = particles;
            }
            
            private void OnCollisionEnter2D(Collision2D collision)
            {
                if (hitParticles != null && collision.contactCount > 0)
                {
                    hitParticles.transform.position = collision.contacts[0].point;
                    hitParticles.Play();
                }
            }
        }

        // Helper component for destructive walls
        private class WallDestructionHandler : MonoBehaviour
        {
            private ParticleSystem destroyParticles;
            
            public void Initialize(ParticleSystem particles)
            {
                destroyParticles = particles;
            }
            
            private void OnTriggerEnter2D(Collider2D other)
            {
                Rigidbody2D rb = other.attachedRigidbody;
                if (rb == null || rb.bodyType != RigidbodyType2D.Dynamic) return;

                StartCoroutine(DestroyAfterDelay(other.gameObject, other.transform.position));
            }
            
            private IEnumerator DestroyAfterDelay(GameObject obj, Vector3 crossPoint)
            {
                yield return new WaitForSeconds(0.5f);
                
                if (obj)
                {
                    if (destroyParticles)
                    {
                        destroyParticles.transform.position = crossPoint;
                        destroyParticles.Play();
                    }

                    if (obj.TryGetComponent(out IPiece piece))
                    {
                        piece.Remove();
                    }
                }
            }
        }
    }
}