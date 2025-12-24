using System;
using System.Collections;
using Game.Core;
using Utility;
using UnityEngine;

namespace Game.Pieces
{
    public class Piece : MonoBehaviour, IPiece
    {
        private IPlayer _owner;
        private Rigidbody2D _rb;
        private readonly Collider2D[] _results = new Collider2D[1];
        private Coroutine _updateLoop;

        public Vector3 GetCurrentPosition() => transform.position;
        public IPlayer GetCurrentPlayer() => _owner;

        //bool is if it settled properly
    

        public event Action<EPlacementType> OnPieceSettled;

        private void OnEnable() => DisablePhysics();
        private void OnDisable() => DisablePhysics();

        private void DisablePhysics()
        {
            _rb ??= GetComponent<Rigidbody2D>();

            _rb.bodyType = RigidbodyType2D.Static;
            _rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        private IEnumerator CheckForSnapCoroutine()
        {
            float despawnTimer = 20f;
            float lowVelocityTimer = 0f;
            const float lowVelocityThreshold = 0.1f;
            const float lowVelocityDuration = 0.2f;

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
                            tile.currentPiece = this;
                            SnapIntoPlace(tile, GameSettings.TileAnimationTime);
                            _updateLoop = null;
                            yield break; // Exit coroutine successfully
                        }

                        Debug.Log($"No tile found at position {transform.position} for piece {gameObject.name}", gameObject);
                        PiecePool.instance.ReturnActivePiece(this);
                        OnPieceSettled?.Invoke(EPlacementType.Missed);
                        _updateLoop = null;
                        yield break;
                    }
                }
                else
                {
                    // Reset timer if velocity goes back up
                    lowVelocityTimer = 0f;
                }

                yield return null; // Wait for next frame
            }

            // Despawn after 20 seconds if nothing happened
            Debug.Log($"Piece {gameObject.name} despawned after 20 seconds", gameObject);
            PiecePool.instance.ReturnActivePiece(this);
            OnPieceSettled?.Invoke(EPlacementType.Fail);
            _updateLoop = null;
        }

        public void AssignPiece(IPlayer newOwner)
        {
            _owner = newOwner;
        }

        public void DropAt(Vector2 velocity, float torque) =>
            DropAt(velocity, torque, transform.position, transform.rotation);

        public void DropAt(Vector2 velocity, float torque, Vector3 location, Quaternion rotation)
        {

            _rb.bodyType = RigidbodyType2D.Dynamic;
            _rb.constraints = RigidbodyConstraints2D.None;

            transform.SetPositionAndRotation(location, rotation);

            _rb.AddForce(velocity, ForceMode2D.Impulse);
            _rb.AddTorque(torque, ForceMode2D.Impulse);

            if (_updateLoop != null) StopCoroutine(_updateLoop);
            _updateLoop = StartCoroutine(CheckForSnapCoroutine());
        }

        public void SnapIntoPlace(ITile tile, float animationTime = 0)
        {
            DisablePhysics();

            if (animationTime <= 0) transform.SetPositionAndRotation(tile.transform.position, tile.transform.rotation);
            else StartCoroutine(SnapIntoPlaceLoop(tile, animationTime));
            if (_updateLoop != null)
            {
                StopCoroutine(_updateLoop);
                _updateLoop = null;
                OnPieceSettled?.Invoke(EPlacementType.Success);
            }
        }

        private IEnumerator SnapIntoPlaceLoop(ITile tile, float animationTime)
        {
            Vector3 location = tile.transform.position;
            Quaternion rotation = tile.transform.rotation;

            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;
            float elapsed = 0f;

            while (elapsed < animationTime)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / animationTime;

                // Smooth interpolation using ease-out curve
                t = 1f - Mathf.Pow(1f - t, 3f);

                transform.position = Vector3.Lerp(startPos, location, t);
                transform.rotation = Quaternion.Slerp(startRot, rotation, t);

                yield return null;
            }

            // Ensure final position is exact
            transform.SetPositionAndRotation(location, rotation);
            OnPieceSettled?.Invoke(EPlacementType.Success);
            tile.GetComponent<SpriteRenderer>().sharedMaterial = GetComponent<SpriteRenderer>().sharedMaterial;
        }
    }
}