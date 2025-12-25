using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core;
using ScriptableObjects;
using Utility;
using UnityEngine;

namespace Game.Pieces
{
    public class PhysicalPiece : MonoBehaviour, IPiece
    {
        private IPlayer _owner;
        private Rigidbody2D _rb;
        private readonly Collider2D[] _results = new Collider2D[1];
        private CancellationTokenSource _cts;

        public Vector3 GetCurrentPosition() => transform.position;
        public IPlayer GetCurrentPlayer() => _owner;


        private void OnDisable()
        {
            // Cancel any running tasks when disabled/returned to pool
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        private void DisablePhysics()
        {
            _rb ??= GetComponent<Rigidbody2D>();

            _rb.bodyType = RigidbodyType2D.Static;
            _rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        public async UniTask<PieceData> DropPieceLoop()
        {
            float despawnTimer = Settings.gameConfiguration.coinSettleTime;
            float lowVelocityTimer = 0f;
            const float lowVelocityThreshold = 0.1f;
            const float lowVelocityDuration = 0.2f;

            try
            {
                while (despawnTimer > 0f)
                {
                    despawnTimer -= Time.deltaTime;

                    // Check if velocity is low enough
                    if (_rb.linearVelocity.magnitude < lowVelocityThreshold)
                    {
                        lowVelocityTimer += Time.deltaTime;

                        // If velocity has been low for the required duration
                        if (lowVelocityTimer >= lowVelocityDuration)
                        {
                            // Check for tile at current position
                            int num = Physics2D.OverlapPoint(transform.position, StaticUtility.TileFilter, _results);

                            if (num > 0 && _results[0].TryGetComponent(out ITile tile))
                            {
                                Debug.Log("I'm trying to snap into place!", gameObject);
                                
                                // Wait for tile to handle the piece placement
                                DisablePhysics();
                                
                                return new (EPlacementType.Success, this, tile); // Exit successfully
                            }
                            Debug.Log($"No tile found at position {transform.position} for piece {gameObject.name}", gameObject);
                            Remove();
                            return new (EPlacementType.GotStuck, this, null);
                        }
                    }
                    else
                    {
                        // Reset timer if velocity goes back up
                        lowVelocityTimer = 0f;
                    }

                    await UniTask.Yield(_cts.Token); // Wait for next frame
                }

                // Despawn after 20 seconds if nothing happened
                Debug.Log($"Piece {gameObject.name} despawned after {Settings.gameConfiguration.coinSettleTime} seconds", gameObject);
                Remove();
                return new (EPlacementType.TimerDespawned, this, null);
            }
            catch (OperationCanceledException)
            {
                // Task was cancelled (object disabled/destroyed), this is expected
                Debug.Log($"Piece {gameObject.name} task cancelled", gameObject);
                return new (EPlacementType.None, this, null);
            }
        }

        public void Remove()
        {
            PiecePool.instance.ReturnActivePiece(this);
        }

        public void Respawn()
        {
            DisablePhysics();
            _cts = new CancellationTokenSource();
            gameObject.SetActive(true);
        }

        public void AssignPlayer(IPlayer newOwner)
        {
            _owner = newOwner;
        }

        public void DropAt(Vector2 velocity, float torque) => DropAt(velocity, torque, transform.position, transform.rotation);

        public void DropAt(Vector2 velocity, float torque, Vector3 location, Quaternion rotation)
        {
            _rb.bodyType = RigidbodyType2D.Dynamic;
            _rb.constraints = RigidbodyConstraints2D.None;

            transform.SetPositionAndRotation(location, rotation);

            _rb.AddForce(velocity, ForceMode2D.Impulse);
            _rb.AddTorque(torque, ForceMode2D.Impulse);

            // Cancel previous task if any
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
        }
    }
}