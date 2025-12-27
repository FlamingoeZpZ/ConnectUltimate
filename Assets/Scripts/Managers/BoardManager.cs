using Game.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class BoardManager
    {
        private readonly BoardGenerator _boardGenerator;
        
        // Directions for checking: right, down, down-right, down-left
        private static readonly Vector2Int[] Directions = new[]
        {
            new Vector2Int(1, 0),   // Horizontal
            new Vector2Int(0, 1),   // Vertical
            new Vector2Int(1, 1),   // Diagonal down-right
            new Vector2Int(1, -1)   // Diagonal down-left
        };
        
        private const int WinLength = 4;
        
        public BoardManager(BoardGenerator generator)
        {
            _boardGenerator = generator;
        }
        
        public bool IsFull()
        {
            foreach (ITile t in _boardGenerator.tiles)
            {
                if (t.GetCurrentPiece() == null) return false;
            }
            return true;
        }

        public void PlacePiece(PieceData data)
        {
            Debug.LogError("I don't think we're tracking this nicely... :(");   
        }

        /// <summary>
        /// Check if the game is over by finding 4 or more consecutive pieces of the same owner.
        /// Returns true if game is over and populates winners array with winning players.
        /// </summary>
        public bool IsGameOver(ref IPlayer[] winners)
        {
            if (_boardGenerator.tiles == null)
            {
                winners = null;
                return false;
            }

            HashSet<IPlayer> winningPlayers = new HashSet<IPlayer>();
            int cols = _boardGenerator.numCols;
            int rows = _boardGenerator.numRows;

            // Check all positions as potential starting points
            for (int col = 0; col < cols; col++)
            {
                for (int row = 0; row < rows; row++)
                {
                    ITile tile = _boardGenerator.GetTile(col, row);
                    if (tile == null) continue;
                    
                    IPiece piece = tile.GetCurrentPiece();
                    if (piece == null) continue;
                    
                    IPlayer player = piece.GetCurrentPlayer();
                    if (player == null) continue;

                    // Check each direction from this position
                    foreach (Vector2Int direction in Directions)
                    {
                        if (CheckLineFromPosition(col, row, direction.x, direction.y, player))
                        {
                            winningPlayers.Add(player);
                        }
                    }
                }
            }

            if (winningPlayers.Count > 0)
            {
                winners = new IPlayer[winningPlayers.Count];
                winningPlayers.CopyTo(winners);
                return true;
            }

            winners = null;
            return false;
        }

        /// <summary>
        /// Check if there are 4 consecutive pieces of the same player starting from position in given direction
        /// </summary>
        private bool CheckLineFromPosition(int startCol, int startRow, int deltaCol, int deltaRow, IPlayer targetPlayer)
        {
            int count = 0;
            int col = startCol;
            int row = startRow;

            // Count consecutive pieces in the direction
            while (IsValidPosition(col, row) && count < WinLength)
            {
                ITile tile = _boardGenerator.GetTile(col, row);
                if (tile == null) break;
                
                IPiece piece = tile.GetCurrentPiece();
                if (piece == null) break;
                
                IPlayer player = piece.GetCurrentPlayer();
                if (player == null || !player.Equals(targetPlayer)) break;

                count++;
                col += deltaCol;
                row += deltaRow;
            }

            return count >= WinLength;
        }

        /// <summary>
        /// Check if position is within board bounds
        /// </summary>
        private bool IsValidPosition(int col, int row)
        {
            return col >= 0 && col < _boardGenerator.numCols && 
                   row >= 0 && row < _boardGenerator.numRows;
        }

        /// <summary>
        /// Optional: Get all winning positions for visual feedback
        /// </summary>
        public List<Vector2Int> GetWinningPositions()
        {
            List<Vector2Int> winningPositions = new List<Vector2Int>();
            int cols = _boardGenerator.numCols;
            int rows = _boardGenerator.numRows;

            for (int col = 0; col < cols; col++)
            {
                for (int row = 0; row < rows; row++)
                {
                    ITile tile = _boardGenerator.GetTile(col, row);
                    if (tile == null) continue;
                    
                    IPiece piece = tile.GetCurrentPiece();
                    if (piece == null) continue;
                    
                    IPlayer player = piece.GetCurrentPlayer();
                    if (player == null) continue;

                    foreach (Vector2Int direction in Directions)
                    {
                        List<Vector2Int> line = GetWinningLine(col, row, direction.x, direction.y, player);
                        if (line != null && line.Count >= WinLength)
                        {
                            winningPositions.AddRange(line);
                        }
                    }
                }
            }

            return winningPositions;
        }

        /// <summary>
        /// Get the positions of a winning line starting from given position
        /// </summary>
        private List<Vector2Int> GetWinningLine(int startCol, int startRow, int deltaCol, int deltaRow, IPlayer targetPlayer)
        {
            List<Vector2Int> positions = new List<Vector2Int>();
            int col = startCol;
            int row = startRow;

            while (IsValidPosition(col, row))
            {
                ITile tile = _boardGenerator.GetTile(col, row);
                if (tile == null) break;
                
                IPiece piece = tile.GetCurrentPiece();
                if (piece == null) break;
                
                IPlayer player = piece.GetCurrentPlayer();
                if (player == null || !player.Equals(targetPlayer)) break;

                positions.Add(new Vector2Int(col, row));
                col += deltaCol;
                row += deltaRow;
            }

            return positions.Count >= WinLength ? positions : null;
        }
    }
}