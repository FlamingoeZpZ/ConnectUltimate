using Game.Core;
using UnityEngine;

namespace Game.Pieces
{
    public class PiecePlacer : MonoBehaviour
    {
        [Header("Settings section")]
        [field: SerializeField]
        public bool useMomentum { get; private set; } = true;

        [SerializeField] private float momentumMultiplier = 0.1f;
        [SerializeField] private float maxMomentum = 3f;
        [SerializeField] private float torqueMultiplier = 0.5f;
        [SerializeField] private float maxTorque = 10f;

        private Camera _mainCamera;
        private PhysicalPiece _currentPiece;
        private IPlayer _currentOwner;

        public PhysicalPiece currentPiece => _currentPiece;

        private class Sampler
        {
            private int _current;
            private uint _total;
            private const int NumSamples = 200;
            private readonly Vector2[] _samples = new Vector2[NumSamples];
            private readonly float[] _times = new float[NumSamples];


            public void Add(Vector2 position, float time)
            {
                _current = (_current + 1) % _samples.Length;
                _samples[_current] = position;
                _times[_current] = time;

                if (_total < _samples.Length)
                {
                    _total++;
                }
            }

            public void Reset()
            {
                _current = -1;
                _total = 0;
            }

            public Vector2 GetVelocity()
            {
                if (_total < 2)
                {
                    return Vector2.zero;
                }

                int oldestIndex = _total < _samples.Length ? 0 : (_current + 1) % _samples.Length;

                Vector2 firstPos = _samples[oldestIndex];
                Vector2 lastPos = _samples[_current];

                float timeDelta = _times[_current] - _times[oldestIndex];

                if (timeDelta <= 0)
                {
                    return Vector2.zero;
                }

                return (lastPos - firstPos) / timeDelta;
            }

            public float GetAngularVelocity(Vector2 currentPiecePosition)
            {
                if (_total < 2)
                {
                    return 0f;
                }

                int oldestIndex = _total < _samples.Length ? 0 : (_current + 1) % _samples.Length;

                // Get positions relative to the piece center
                Vector2 firstPos = _samples[oldestIndex];
                Vector2 lastPos = _samples[_current];

                // Convert screen positions to world positions for accurate calculation
                Vector2 firstRelative = firstPos - currentPiecePosition;
                Vector2 lastRelative = lastPos - currentPiecePosition;

                // Calculate the angular change
                float angle1 = Mathf.Atan2(firstRelative.y, firstRelative.x);
                float angle2 = Mathf.Atan2(lastRelative.y, lastRelative.x);
                float angleDelta = Mathf.DeltaAngle(angle1 * Mathf.Rad2Deg, angle2 * Mathf.Rad2Deg);

                float timeDelta = _times[_current] - _times[oldestIndex];

                if (timeDelta <= 0)
                {
                    return 0f;
                }

                return angleDelta / timeDelta;
            }
        }

        private readonly Sampler _mouseSampler = new();

        void Start()
        {
            _mainCamera = Camera.main;
        }

        public bool HasActivePiece() => _currentPiece != null;

        public void ReleasePiece()
        {
            if (_currentPiece == null) return;

            if (useMomentum)
            {
                Vector2 vel = _mouseSampler.GetVelocity() * momentumMultiplier;
                if (vel.magnitude > maxMomentum) vel = vel.normalized * maxMomentum;

                // Calculate torque based on angular velocity from mouse movement
                Vector2 screenPos = _mainCamera.WorldToScreenPoint(_currentPiece.transform.position);
                float angularVel = _mouseSampler.GetAngularVelocity(screenPos) * torqueMultiplier;
                angularVel = Mathf.Clamp(angularVel, -maxTorque, maxTorque);

                _currentPiece.DropAt(vel, angularVel);
            }
            else
            {
                _currentPiece.DropAt(Vector2.zero, 0f);
            }

            _currentPiece = null;
        }


        public void HandlePiece(Vector2 loc)
        {
            PlacePiece(loc);
            _mouseSampler.Add(loc, Time.time); // Changed from Time.deltaTime to Time.time
        }

        public void CreateChip(Vector2 startingLocation)
        {
            
            Debug.Log("Creating a new piece.");
            
            _currentPiece = PiecePool.instance.SpawnPiece(_currentOwner);
            _mouseSampler.Reset();
            PlacePiece(startingLocation);
        }

        public void PlacePiece(Vector2 screenPosition)
        {
            if (_currentPiece == null) return;
            
            Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, _mainCamera.nearClipPlane));
            worldPosition.z = 0; // Ensure it's on the 2D plane
            _currentPiece.transform.position = worldPosition;
        }

        public void SetOwner(IPlayer human)
        {
            _currentOwner = human;
        }
    }
}