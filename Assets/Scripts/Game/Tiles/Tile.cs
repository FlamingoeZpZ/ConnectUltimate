using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core;
using UnityEngine;
using ScriptableObjects;

namespace Game.Tiles
{
    public class Tile : MonoBehaviour, ITile
    {
        private IPiece _currentPiece;
        private CancellationTokenSource _cts;

        private void OnEnable()
        {
            _cts = new CancellationTokenSource();
        }

        private void OnDisable()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        public async UniTask SetCurrentPiece(IPiece newPiece)
        {
            Debug.Log("I have a new piece!", gameObject);
            
            _currentPiece = newPiece;

            if (_currentPiece is not MonoBehaviour) return; // Nothing else we do matters as the piece must also be physical.

            Debug.Log("I have a new piece and it's real");
            
            // Animate the snap with visual feedback
            float animationTime = Settings.effectsConfiguration.tileAnimationTime;
            
            if (animationTime > 0)
            {
                await AnimateSnapAsync(newPiece, animationTime);
            }

            // Match materials for visual consistency
            if (TryGetComponent<SpriteRenderer>(out var tileRenderer) && 
                newPiece is Component pieceComponent && 
                pieceComponent.TryGetComponent<SpriteRenderer>(out var pieceRenderer))
            {
                tileRenderer.sharedMaterial = pieceRenderer.sharedMaterial;
            }
        }

        private async UniTask AnimateSnapAsync(IPiece piece, float animationTime)
        {
            if (piece is not Component pieceComponent) return;

            Transform pieceTransform = pieceComponent.transform;
            Vector3 startPos = pieceTransform.position;
            Quaternion startRot = pieceTransform.rotation;
            Vector3 targetPos = transform.position;
            Quaternion targetRot = transform.rotation;

            float elapsed = 0f;

            while (elapsed < animationTime)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / animationTime;

                // Smooth interpolation using ease-out curve
                t = 1f - Mathf.Pow(1f - t, 3f);

                pieceTransform.position = Vector3.Lerp(startPos, targetPos, t);
                pieceTransform.rotation = Quaternion.Slerp(startRot, targetRot, t);

                await UniTask.Yield(_cts.Token);
            }

            // Ensure final position is exact
            pieceTransform.SetPositionAndRotation(targetPos, targetRot);
        }

        public IPiece GetCurrentPiece() => _currentPiece;
    }
}