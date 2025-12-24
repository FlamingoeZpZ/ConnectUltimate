using System.Collections.Generic;
using Game.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Pieces
{

//Do I actually even want this?
    public class PiecePool : MonoBehaviour
    {
        [SerializeField] private Piece prefab;

        private const int BaseNumTiles = 16;

        private readonly List<Piece> _activePieces = new();
        private readonly Queue<Piece> _inactivePieces = new();
        public static PiecePool instance { get; private set; }


        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }

            instance = this;

            DontDestroyOnLoad(gameObject);

            for (int i = 0; i < BaseNumTiles; ++i)
            {
                EnqueuePiece(CreatePiece());
            }

            //Safety? Unload all pieces if we swap scenes.
            SceneManager.sceneUnloaded += _ => ReturnAllPieces();
        }



        public Piece SpawnPiece(IPlayer owner)
        {
            Piece piece = GetOrCreatePiece();
            piece.AssignPiece(owner);
            ActivatePiece(piece);
            return piece;
        }

        public void ReturnAllPieces()
        {
            for (int i = _activePieces.Count - 1; i >= 0; i--)
            {
                EnqueuePiece(_activePieces[i]);
                _activePieces.RemoveAt(i);
            }
        }

        public void ReturnActivePiece(Piece piece)
        {
            if (_activePieces.Contains(piece))
            {
                EnqueuePiece(piece);
                _activePieces.Remove(piece);
            }
        }

        private Piece GetOrCreatePiece()
        {
            if (_inactivePieces.TryDequeue(out var piece)) return piece;
            return CreatePiece();
        }

        private Piece CreatePiece()
        {
            Piece piece = Instantiate(prefab, transform);
            return piece;
        }

        private void EnqueuePiece(Piece piece)
        {
            _inactivePieces.Enqueue(piece);
            piece.gameObject.SetActive(false);
        }

        private void ActivatePiece(Piece piece)
        {
            _activePieces.Add(piece);
            piece.gameObject.SetActive(true);
        }
    }
}
