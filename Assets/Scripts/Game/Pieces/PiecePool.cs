using System.Collections.Generic;
using Game.Core;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Pieces
{

    public class PiecePool : MonoBehaviour
    {
        [SerializeField] private PhysicalPiece prefab;
        private BoardConfiguration _boardConfiguration;

        private const int BaseNumTiles = 16;

        private readonly List<PhysicalPiece> _activePieces = new();
        private readonly Queue<PhysicalPiece> _inactivePieces = new();
        public static PiecePool instance { get; private set; }


        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }

            instance = this;
            _boardConfiguration = Settings.boardConfiguration;

            DontDestroyOnLoad(gameObject);

            for (int i = 0; i < BaseNumTiles; ++i)
            {
                EnqueuePiece(CreatePiece());
            }

            //Safety? Unload all pieces if we swap scenes.
            SceneManager.sceneUnloaded += _ => ReturnAllPieces();
        }



        public PhysicalPiece SpawnPiece(IPlayer owner)
        {
            PhysicalPiece physicalPiece = GetOrCreatePiece();
            physicalPiece.transform.localScale = Vector3.one * (_boardConfiguration.RunTimeTileSize - 0.02f);
            physicalPiece.AssignPiece(owner);
            ActivatePiece(physicalPiece);
            return physicalPiece;
        }

        public void ReturnAllPieces()
        {
            for (int i = _activePieces.Count - 1; i >= 0; i--)
            {
                EnqueuePiece(_activePieces[i]);
                _activePieces.RemoveAt(i);
            }
        }

        public void ReturnActivePiece(PhysicalPiece physicalPiece)
        {
            if (_activePieces.Contains(physicalPiece))
            {
                EnqueuePiece(physicalPiece);
                _activePieces.Remove(physicalPiece);
            }
        }

        private PhysicalPiece GetOrCreatePiece()
        {
            if (_inactivePieces.TryDequeue(out var piece)) return piece;
            return CreatePiece();
        }

        private PhysicalPiece CreatePiece()
        {
            PhysicalPiece physicalPiece = Instantiate(prefab, transform);
            return physicalPiece;
        }

        private void EnqueuePiece(PhysicalPiece physicalPiece)
        {
            _inactivePieces.Enqueue(physicalPiece);
            physicalPiece.gameObject.SetActive(false);
        }

        private void ActivatePiece(PhysicalPiece physicalPiece)
        {
            _activePieces.Add(physicalPiece);
            physicalPiece.gameObject.SetActive(true);
        }
    }
}
