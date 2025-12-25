using Game.Core;
using ScriptableObjects;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// Responsible for generating and regenerating the visual board structure
    /// Could implement strategy / factory in the future.
    /// </summary>
    public class BoardGenerator : MonoBehaviour
    {
        [Header("Visual Configuration")]
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private float cellSpacing = 0.1f;
        
        [Header("Screen Layout")]
        [SerializeField] private float leftMargin = 0.1f;
        [SerializeField] private float rightMargin = 0.1f;
        [SerializeField] private float bottomMargin = 0.1f;
        [SerializeField] private float yOffset;

        private BoardConfiguration _boardConfiguration;
        private Camera _mainCamera;
        private Transform _boardContainer;
        private ITile[,] _tiles;
        
        public ITile[,] tiles => _tiles;

        public int numRows { get; private set; } = 7;
        public int numCols { get; private set; } = 7;
        public float cellSize { get => _boardConfiguration.RunTimeTileSize; private set => _boardConfiguration.RunTimeTileSize = value; }

        private void Awake()
        {
            _mainCamera = Camera.main;
            _boardContainer = transform;
            _boardConfiguration = Settings.boardConfiguration;
            _boardConfiguration.OnUpdated += InitializeConfig;
            InitializeConfig();
        }

        private void OnDestroy()
        {
            _boardConfiguration.OnUpdated -= InitializeConfig;
        }

        public void InitializeConfig()
        {
            numRows = _boardConfiguration.numRows;
            numCols = _boardConfiguration.numCols;
            RegenerateBoard(numRows, numCols);
        }

        private void RegenerateBoard(int rows, int cols)
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
                if (_mainCamera == null)
                {
                    Debug.LogError("Main camera not found!");
                    return;
                }
            }

            if (cellPrefab == null)
            {
                Debug.LogError("Cell prefab is not assigned!");
                return;
            }

            ClearBoard();

            // Initialize the tiles array
            _tiles = new ITile[cols, rows];

            float screenHeight = _mainCamera.orthographicSize * 2f;
            float screenWidth = screenHeight * _mainCamera.aspect;

            // Calculate available space considering margins
            float availableWidth = screenWidth * (1f - leftMargin - rightMargin);
            float availableHeight = screenHeight * (1f - bottomMargin);

            // Calculate cell size to fit the grid (accounting for spacing)
            float cellWidth = (availableWidth - (cols - 1) * cellSpacing) / cols;
            float cellHeight = (availableHeight - (rows - 1) * cellSpacing) / rows;

            cellSize = Mathf.Min(cellWidth, cellHeight);

            // Calculate actual board dimensions with spacing
            float totalBoardWidth = cols * cellSize + (cols - 1) * cellSpacing;
            float totalBoardHeight = rows * cellSize + (rows - 1) * cellSpacing;

            // Calculate board position
            // Horizontally centered
            float boardCenterX = 0f;
            
            // Vertically positioned from bottom with margin
            float screenBottom = -_mainCamera.orthographicSize;
            float boardBottomY = screenBottom + (screenHeight * bottomMargin);
            float boardCenterY = boardBottomY + totalBoardHeight / 2f + yOffset;

            // Calculate starting position for cells (top-left corner of board)
            float startX = -totalBoardWidth / 2f + cellSize / 2f;
            float startY = totalBoardHeight / 2f - cellSize / 2f;

            CreateColumns(rows, cols, startX, startY, totalBoardHeight);

            // Position the board container
            _boardContainer.position = new Vector3(boardCenterX, boardCenterY, 0);
        }

        private void CreateColumns(int rows, int cols, float startX, float startY, float totalBoardHeight)
        {
            for (int col = 0; col < cols; col++)
            {
                GameObject columnObject = new GameObject($"Column_{col}");
                columnObject.transform.SetParent(_boardContainer);

                EdgeCollider2D edgeCollider = columnObject.AddComponent<EdgeCollider2D>();

                float halfWidth = cellSize / 2f;
                float halfHeight = totalBoardHeight / 2f;

                Vector2[] points = {
                    new(-halfWidth, halfHeight),
                    new(-halfWidth, -halfHeight),
                    new(halfWidth, -halfHeight),
                    new(halfWidth, halfHeight)
                };

                edgeCollider.points = points;

                float xPos = startX + col * (cellSize + cellSpacing);
                columnObject.transform.position = new Vector3(xPos, 0, 0);

                CreateCellsInColumn(columnObject.transform, rows, col, startY);
            }
        }

        private void CreateCellsInColumn(Transform columnParent, int rows, int col, float startY)
        {
            for (int row = 0; row < rows; row++)
            {
                GameObject cell = Instantiate(cellPrefab, columnParent);

                float yPos = startY - row * (cellSize + cellSpacing);
                cell.transform.localPosition = new Vector3(0, yPos, 0);
                cell.transform.localScale = Vector3.one * cellSize;

                cell.name = $"Cell_{col}_{row}";

                // Store the ITile reference
                if (cell.TryGetComponent(out ITile tile))
                {
                    _tiles[col, row] = tile;
                }
                else
                {
                    Debug.LogError($"Cell prefab at ({col}, {row}) does not have ITile component!", cell);
                }
            }
        }

        private void ClearBoard()
        {
            while (_boardContainer.childCount > 0)
            {
                DestroyImmediate(_boardContainer.GetChild(0).gameObject);
            }
            
            _tiles = null;
        }

        /// <summary>
        /// Gets the ITile at the specified position
        /// </summary>
        public ITile GetTile(int col, int row)
        {
            if (_tiles == null || col < 0 || col >= numCols || row < 0 || row >= numRows)
            {
                Debug.LogWarning($"Invalid tile position: ({col}, {row})");
                return null;
            }
            
            return _tiles[col, row];
        }

        /// <summary>
        /// Gets all tiles in a specific column
        /// </summary>
        public ITile[] GetColumn(int col)
        {
            if (_tiles == null || col < 0 || col >= numCols)
            {
                Debug.LogWarning($"Invalid column: {col}");
                return null;
            }

            ITile[] column = new ITile[numRows];
            for (int row = 0; row < numRows; row++)
            {
                column[row] = _tiles[col, row];
            }
            
            return column;
        }

        /// <summary>
        /// Gets all tiles in a specific row
        /// </summary>
        public ITile[] GetRow(int row)
        {
            if (_tiles == null || row < 0 || row >= numRows)
            {
                Debug.LogWarning($"Invalid row: {row}");
                return null;
            }

            ITile[] rowTiles = new ITile[numCols];
            for (int col = 0; col < numCols; col++)
            {
                rowTiles[col] = _tiles[col, row];
            }
            
            return rowTiles;
        }

        /// <summary>
        /// Gets the entire tiles array (use carefully)
        /// </summary>
        public ITile[,] GetAllTiles()
        {
            return _tiles;
        }

        [ContextMenu("Regenerate Board")]
        public void RegenerateBoardEditor()
        {
            if (_boardConfiguration != null)
            {
                RegenerateBoard(_boardConfiguration.numRows, _boardConfiguration.numCols);
            }
            else
            {
                Debug.LogError("Board configuration is not set!");
            }
        }

        [ContextMenu("Clear Board")]
        public void ClearBoardEditor()
        {
            ClearBoard();
        }

        [ContextMenu("Reset Margins to Default")]
        public void ResetMarginsToDefault()
        {
            leftMargin = 0.1f;
            rightMargin = 0.1f;
            bottomMargin = 0.1f;
            yOffset = 0f;
            
            if (_boardConfiguration != null)
            {
                RegenerateBoard(_boardConfiguration.numRows, _boardConfiguration.numCols);
            }
        }

        [ContextMenu("Debug Print Tiles")]
        public void DebugPrintTiles()
        {
            if (_tiles == null)
            {
                Debug.Log("Tiles array is null!");
                return;
            }

            Debug.Log($"Tiles Array: {numCols} columns x {numRows} rows");
            
            for (int row = 0; row < numRows; row++)
            {
                string rowString = $"Row {row}: ";
                for (int col = 0; col < numCols; col++)
                {
                    rowString += _tiles[col, row] != null ? "[✓] " : "[✗] ";
                }
                Debug.Log(rowString);
            }
        }
        
        #if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if(cellPrefab == null || !cellPrefab.TryGetComponent(out ITile _))
                Debug.LogError("There is an issue with the current selected tile prefab: ", gameObject);
            
            // Draw margin boundaries for visualization
            if (_mainCamera == null) _mainCamera = Camera.main;
            if (_mainCamera != null)
            {
                float screenHeight = _mainCamera.orthographicSize * 2f;
                float screenWidth = screenHeight * _mainCamera.aspect;
                
                Gizmos.color = Color.yellow;
                
                // Draw left margin
                float leftX = -screenWidth / 2f + screenWidth * leftMargin;
                Gizmos.DrawLine(
                    new Vector3(leftX, -_mainCamera.orthographicSize, 0),
                    new Vector3(leftX, _mainCamera.orthographicSize, 0)
                );
                
                // Draw right margin
                float rightX = screenWidth / 2f - screenWidth * rightMargin;
                Gizmos.DrawLine(
                    new Vector3(rightX, -_mainCamera.orthographicSize, 0),
                    new Vector3(rightX, _mainCamera.orthographicSize, 0)
                );
                
                // Draw bottom margin
                float bottomY = -_mainCamera.orthographicSize + screenHeight * bottomMargin;
                Gizmos.DrawLine(
                    new Vector3(-screenWidth / 2f, bottomY, 0),
                    new Vector3(screenWidth / 2f, bottomY, 0)
                );
            }
        }
        #endif
    }
}