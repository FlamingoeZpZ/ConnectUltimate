using System;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace Game.Core
{
    public interface IPiece
    {
        public Vector3 GetCurrentPosition();
        public IPlayer GetCurrentPlayer();
        public void AssignPlayer(IPlayer newPlayer);
        public UniTask<PieceData> DropPieceLoop();
        public void Remove();
        public void Respawn();
    }
    
    public struct PieceData
    {
        public readonly EPlacementType PlacementType;
        [NotNull] public readonly IPiece Piece;
        [CanBeNull] public readonly ITile Tile;

        public PieceData(EPlacementType placementType, IPiece piece, ITile tile)
        {
            PlacementType = placementType;
            Piece = piece;
            Tile = tile;
        }
    }
    
    [Flags]
    public enum EPlacementType
    {
        None = 0, 
        GotStuck = 1, // This is when it lands on something and got stuck, but not in any tile... 
        TimerDespawned = 2,  //This is when the player is bad 
        Success = 4, // This is when the piece lands properly.
        MissOrSuccess = TimerDespawned | Success,
        Any = GotStuck | TimerDespawned | Success,
    }
}