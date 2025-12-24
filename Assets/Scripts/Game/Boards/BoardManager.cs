using Cysharp.Threading.Tasks;
using Game.Core;
using ScriptableObjects;
using UnityEngine;

namespace Game.Boards
{
    [DefaultExecutionOrder(-1000)]
    public class BoardManager : MonoBehaviour, IConfiguration
    {
        
        [Header("Unity config")]
        private BoardConfiguration _boardConfiguration;
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private float screenCoverage = 0.9f; // How much of the screen the board should cover (0-1)
        [SerializeField] private float cellSpacing = 0.1f; // Space between cells

        public static BoardManager instance { get; private set; }

        private int[,] _tiles;
        private Camera _mainCamera;
        
        
        private void Awake()
        {
            if (instance && instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            
            _mainCamera = Camera.main;
        }
        
    
        private void RegenerateBoard(int numRows, int numCols)
        { 
            
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
                if (_mainCamera == null) return;
            }

            if (cellPrefab == null)
            {
                Debug.LogError("Cell prefab is not assigned!");
                return;
            }

            // Clear existing board
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }

            // Get the screen bounds in world space
            float screenHeight = _mainCamera.orthographicSize * 2f;
            float screenWidth = screenHeight * _mainCamera.aspect;

            // Calculate available board size based on screen coverage
            float availableWidth = screenWidth * screenCoverage;
            float availableHeight = screenHeight * screenCoverage;

            // Calculate cell size to fit the grid (accounting for spacing)
            float cellWidth = (availableWidth - (numCols - 1) * cellSpacing) / numCols;
            float cellHeight = (availableHeight - (numRows - 1) * cellSpacing) / numRows;
        
            // Make cells square by using the smaller dimension
            float cellSize = Mathf.Min(cellWidth, cellHeight);

            // Calculate actual board dimensions with spacing
            float totalBoardWidth = numCols * cellSize + (numCols - 1) * cellSpacing;
            float totalBoardHeight = numRows * cellSize + (numRows - 1) * cellSpacing;

            // Calculate starting position (top-left corner)
            float startX = -totalBoardWidth / 2f + cellSize / 2f;
            float startY = totalBoardHeight / 2f - cellSize / 2f;

            // Create columns
            for (int col = 0; col < numCols; col++)
            {
                GameObject columnObject = new GameObject($"Column_{col}");
                columnObject.transform.SetParent(transform);

                // Create EdgeCollider2D with U-shape (open at top)
                EdgeCollider2D edgeCollider = columnObject.AddComponent<EdgeCollider2D>();
            
                float halfWidth = cellSize / 2f;
                float halfHeight = totalBoardHeight / 2f;
            
                // Define points for U-shape: bottom-left -> bottom -> bottom-right -> top-right
                // (leaving top open for coins to drop in)
                Vector2[] points = new Vector2[]
                {
                    new Vector2(-halfWidth, halfHeight),   // Top-left
                    new Vector2(-halfWidth, -halfHeight),  // Bottom-left
                    new Vector2(halfWidth, -halfHeight),   // Bottom-right
                    new Vector2(halfWidth, halfHeight)     // Top-right
                };
            
                edgeCollider.points = points;
            
                float xPos = (startX + col * (cellSize + cellSpacing));
                columnObject.transform.position = new Vector3(xPos, 0, 0);
            
                // Create cells in this column
                for (int row = 0; row < numRows; row++)
                {
                    GameObject cell = Instantiate(cellPrefab, columnObject.transform);
                
                    // Position the cell
                    float yPos = startY - row * (cellSize + cellSpacing);
                    cell.transform.localPosition = new Vector3(0, yPos, 0);

                    // Scale the cell to fit
                    cell.transform.localScale = Vector3.one * cellSize;
                
                    cell.name = $"Cell_{col}_{row}";
                }
            }

            // Center the board
            transform.position = Vector3.zero;
        }

        /// <summary>
        /// Given where to place a tile....
        /// Returns a winner if applicable, else returns -1... 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public async UniTask PlacePiece(IPlayer player, int x, int y)
        {
            _tiles[x, y] = player.GetPlayerInformation().TeamNumber;
        }
        
        
        
        public void InitializeConfig(ScriptableObject configData)
        {
            if (configData is BoardConfiguration data)
            {
                _boardConfiguration = data;
                RegenerateBoard(_boardConfiguration.numRows, _boardConfiguration.numCols);
            }
            else
            {
                Debug.LogError("Invalid data", gameObject);
            }
        }


        public bool IsGameOver(ref IPlayer[] winners)
        {
            return false;
        }

        public bool IsFull()
        {
            
            return false;
        }
        
        // Helper method to call from inspector or other scripts
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
    }
}