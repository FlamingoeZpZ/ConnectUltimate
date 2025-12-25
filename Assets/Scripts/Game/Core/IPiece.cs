using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Core
{
    public interface IPiece
    {
        public Vector3 GetCurrentPosition();
        public IPlayer GetCurrentPlayer();
        public UniTask<PieceData> DropPieceLoop();
    }
    
    public struct PieceData
    {
        public readonly EPlacementType PlacementType;
        public readonly IPiece Piece;

        public PieceData(EPlacementType placementType, IPiece piece)
        {
            PlacementType = placementType;
            Piece = piece;
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