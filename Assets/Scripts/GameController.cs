using System;
using System.Collections;
using System.Collections.Generic;
using GoalSystem;
using Helpers;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public partial class GameController : MonoBehaviour
{
   [SerializeField] private SlotController slotController;
   [SerializeField] private DrinkController drinkController;
   [SerializeField] private Transform drinkParent;
   [SerializeField] private Transform waitressParent;
   private static GameController _instance;
   private LevelGenerator _levelGenerator;
   private Dictionary<ItemType, Cell[,]> _grid;
   private const int WaitressLayer = 1;
   public static bool EditingLevel => SceneManager.GetActiveScene().name == "LevelEditor";

   #region Level Values
   private int _levelWaitressCount;
   private int _levelDrinkCount;
   private int _completedWaitressCount = 0;
   private int _completedDrinkCount = 0;
   #endregion
   
   public static GameController Instance
   {
      get
      {
         if (_instance == null)
         {
            _instance = FindObjectOfType<GameController>();
         }

         return _instance;
      }
   }

   private void OnEnable()
   {
      AddListeners();
   }

   private void OnDisable()
   {
      RemoveListeners();
   }

   private void Start()
   {
      transform.AddComponent<LevelGenerator>();
      _levelGenerator = GetComponent<LevelGenerator>();
      if (!EditingLevel)
      {
         LoadLevel();
         CalculateLevelWaitressCount();
         CalculateLevelDrinkCount();
         Debug.Log("level waitress count : " + _levelWaitressCount);
      }
   }

   private void CalculateLevelWaitressCount()
   {
      var waitressGrid = _grid[ItemType.WaitressArea];
      foreach (var cell in waitressGrid)
      {
         if (cell.GetTile(WaitressLayer) != null)
         {
            _levelWaitressCount++;
         }
      }
   }
   
   private void CalculateLevelDrinkCount()
   {
      var drinkGrid = _grid[ItemType.DrinkArea];
      foreach (var cell in drinkGrid)
      {
         if (cell.GetTile(2) != null)
         {
            _levelDrinkCount++;
         }
      }
   }

   private void LoadLevel()
   {
      int levelIndex = PlayerPrefs.GetInt("LevelIndex", 1);
      if (levelIndex > 10)
      {
         levelIndex = 1;
         PlayerPrefs.SetInt("LevelIndex", levelIndex);
      }
      TextAsset levelFile = Resources.Load<TextAsset>($"Levels/Level{levelIndex}");

      if (levelFile != null)
      {
         // Deserialize JSON into LevelData
         LevelData levelData = JsonUtility.FromJson<LevelData>(levelFile.text);

         if (levelData != null)
         {
            // Use the deserialized data to generate the level grid
            _grid = _levelGenerator.GenerateLevel(levelData);

            // Set the drink grid to the drink area portion of the generated grid
            drinkController.SetDrinkGrid(_grid[ItemType.DrinkArea]);
            _levelGenerator.AfterAllCellsInitialized();
         }
         else
         {
            Debug.LogError("Failed to parse level data.");
         }
      }
      else
      {
         Debug.LogError("Level file not found.");
      }
   }

   public bool TryFindPath(BaseTile tile, out List<Cell> travelPath)
   {
      travelPath = new List<Cell>();
      var listToCheck = _grid[tile.GetItemType()];
      var startCoord = new Vector2Int(tile.X, tile.Y);

      // Queue for BFS, storing the current coordinate and the path to it
      var queue = new Queue<(Vector2Int coord, List<Cell> path)>();
      var visited = new HashSet<Vector2Int>();
      
      if (startCoord.y == listToCheck.GetLength(1) - 1)
      {
         // Mark the path as "found" without moving
         travelPath = new List<Cell>();
         listToCheck[tile.X, tile.Y].SetTileNull(tile.GetLayer());
         return true;
      }
    
      // Add the start tile to the queue
      queue.Enqueue((startCoord, new List<Cell>()));
      visited.Add(startCoord);

      while (queue.Count > 0)
      {
         var (currentCoord, currentPath) = queue.Dequeue();

         // Get the cell at the current coordinate
         if (IsCoordinateValid(listToCheck, currentCoord.x, currentCoord.y) &&
             IsCellAvailable(listToCheck, currentCoord.x, currentCoord.y, tile, out Cell currentCell))
         {
            // Add the current cell to the path
            currentPath.Add(currentCell);

            if (IsPathFound(listToCheck, currentCoord.y, startCoord.y))
            {
               travelPath = currentPath;
               ClearPathData(tile, currentPath);
               return true;
            }

            // Explore neighboring cells
            for (int i = 0; i < Enum.GetValues(typeof(Direction)).Length; i++)
            {
               var vector = Utilities.Vectors.GetValueOrDefault((Direction)i);
               var nextCoord = new Vector2Int(currentCoord.x + vector.x, currentCoord.y + vector.y);

               if (!visited.Contains(nextCoord) && 
                   IsCoordinateValid(listToCheck, nextCoord.x, nextCoord.y))
               {
                  visited.Add(nextCoord);
                  var newPath = new List<Cell>(currentPath) { currentCell };
                  queue.Enqueue((nextCoord, newPath));
               }
            }
         }
      }

      // No path found
      return false;
   }
   
   private void ClearPathData(BaseTile tile, List<Cell> path)
   {
      var listToCheck = _grid[tile.GetItemType()];
      TryDisableElements(listToCheck, tile);
      listToCheck[tile.X, tile.Y].SetTileNull(WaitressLayer);

      foreach (var cell in path)
      {
         cell.SetTileNull(WaitressLayer);
      }
   }

   private void TryDisableElements(Cell[,] grid, BaseTile tile)
   {
      for (int i = 0; i < Enum.GetValues(typeof(Direction)).Length; i++)
      {
         var vector = Utilities.Vectors.GetValueOrDefault((Direction)i);
         var neighbor = new Vector2Int(tile.X + vector.x, tile.Y + vector.y);
         if (IsCoordinateValid(grid, neighbor.x, neighbor.y))
         {
            var cell = grid[neighbor.x, neighbor.y];
            var neighborTile = cell.GetTile(WaitressLayer);
            if (neighborTile != null && neighborTile.IsElementTile())
            {
               neighborTile.DisableElement();
            }
         }
      }
   }

   private bool IsPathFound(Cell[,] grid, int y, int startY)
   {
      // If the starting Y is already in the topmost row
      if (startY == grid.GetLength(1) - 1)
      {
         return true; // Path is already "found" since it's on the topmost row
      }

      // Otherwise, check if the current Y is in the topmost row
      return y == grid.GetLength(1) - 1;
   }

   private bool IsCoordinateValid(Cell[,] grid, int x, int y)
   {
      return x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1) && grid[x, y] != null;
   }

   private bool IsCellAvailable(Cell[,] grid, int x, int y, BaseTile tile, out Cell cell)
   {
      var temp = grid[x, y];
      if (temp == null)
      {
         cell = null;
         return false;
      }
      if (temp.IsTileAvailable(WaitressLayer))
      {
         cell = temp;
         return true;
      }
      else
      {
         var tempTile = temp.GetTile(WaitressLayer);
         if (tile == tempTile)
         {
            cell = temp;
            return true;
         }
      }

      cell = null;
      return false;
   }

   public bool IsInputAcceptable()
   {
      return slotController.IsAvailableSlotExist();
   }

   public Transform GetParentByType(ItemType type)
   {
      return type == ItemType.DrinkArea ? drinkParent : waitressParent;
   }

   public Cell[,] GetGridByType(ItemType type)
   {
      return _grid.GetValueOrDefault(type);
   }

   private void AddListeners()
   {

   }

   private void RemoveListeners()
   {

   }
   private void CheckLevelCompleteConditionProvided()
   {
      Debug.Log($"level waitress count {_levelWaitressCount} and completed count {_completedWaitressCount}:");
      if (_levelWaitressCount == _completedWaitressCount)
      {
         GameEnd(true);
      }

   }
}
//Events partial class
public partial class GameController
{
   #region Event Delegates
   public static event Action<Waitress, WaitressSlot> OnWaitressMadeFinalMovement;
   public static event Action<bool> OnGameEnd;
   public static event Action<bool> OnSetLevel;
   #endregion

   #region Event Functions
   public void WaitressMadeFinalMovement(Waitress waitress, WaitressSlot waitressSlot)
   {
      //if (OnWaitressMadeFinalMovement != null)
      //{
        // OnWaitressMadeFinalMovement(waitress, waitressSlot);
         _completedWaitressCount++;
         _completedDrinkCount += waitressSlot.CurrentDrinkCount;
         CheckLevelCompleteConditionProvided();
      //}

   }

   [ContextMenu("Check fail popup")]
   public void Fail()
   {
      if (OnGameEnd != null)
      {
         OnGameEnd(false);
         SetLevel(false);
      }
   }
   
   [ContextMenu("Check win")]
   public void Win()
   {
      if (OnGameEnd != null)
      {
         OnGameEnd(true);
         SetLevel(true);
      }
   }
   
   public void GameEnd(bool isWin)
   {
      if (OnGameEnd != null)
      {
         if (isWin)
         {
            var index = PlayerPrefs.GetInt("LevelIndex", 1);
            PlayerPrefs.SetInt("LevelIndex", index+1);
         }
         OnGameEnd(isWin);
         SetLevel(isWin);
      }

   }
   public void SetLevel(bool isNextLevel)
   {
      if (OnSetLevel != null)
      {
         OnSetLevel(isNextLevel);
         _completedWaitressCount = 0;
         _completedDrinkCount = 0;
      }

   }

   #endregion

}
