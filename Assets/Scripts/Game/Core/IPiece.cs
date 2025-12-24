using UnityEngine;

namespace Game.Core
{
    public interface IPiece
    {
        public Vector3 GetCurrentPosition();
        public IPlayer GetCurrentPlayer();
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
    
    public enum EPlacementType
    {
        None = -1,
        Fail, //This is when the player is bad
        Missed, // This is when it lands on something and got stuck, but not in any tile... 
        Success
    }
}