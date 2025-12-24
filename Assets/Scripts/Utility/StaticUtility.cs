using UnityEngine;

namespace Utility
{
    public class StaticUtility
    {
        public static readonly int TileLayer = 1 << LayerMask.NameToLayer("Tile");
        public static readonly int PieceLayer = 1<< LayerMask.NameToLayer("Piece");

        public static readonly ContactFilter2D TileFilter = new()
        {
            layerMask = TileLayer,
            useLayerMask = true,
            useDepth = false,
            useTriggers = true
        };
    }
}