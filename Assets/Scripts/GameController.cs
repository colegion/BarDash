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
   private const int _waitressLayer = 1;
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
         LoadLevel();
   }

   private void LoadLevel()
   {
      // Load the JSON text file based on level index, or a default file
      TextAsset levelFile = Resources.Load<TextAsset>("Levels/Level3");

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
      List<Cell> cellsToTravel = new List<Cell>();
      bool pathFound = false;
      var listToCheck = _grid[tile.GetItemType()];
      var currentCoord = new Vector2Int(tile.X, tile.Y);

      while (!pathFound)
      {
         for (int i = 0; i < Enum.GetValues(typeof(Direction)).Length; i++)
         {
            var vector = Utilities.Vectors.GetValueOrDefault((Direction)i);
            if(vector == Vector2Int.zero) continue;
            var nextCoord = new Vector2Int(currentCoord.x + vector.x, currentCoord.y + vector.y);
            if (IsCoordinateValid(listToCheck, nextCoord.x, nextCoord.y))
            {
               if (IsCellAvailable(listToCheck, nextCoord.x, nextCoord.y, out Cell cell))
               {
                  currentCoord = nextCoord;
                  cellsToTravel.Add(cell);
               }
               break;
            }
            else
            {
               pathFound = IsPathFound(listToCheck, nextCoord.y);
               if (pathFound) break;
            }
         }

         if (cellsToTravel.Count == 0)
         {
            break;
         }
      }


      travelPath = cellsToTravel;
      if (pathFound)
      {
         TryDisableElements(listToCheck, tile);
         listToCheck[tile.X, tile.Y].SetTileNull(_waitressLayer);
      }

      return pathFound;
   }

   private void TryDisableElements(Cell[,] grid, BaseTile tile)
   {
      for (int i = 0; i < Enum.GetValues(typeof(Direction)).Length; i++)
      {
         var vector = Utilities.Vectors.GetValueOrDefault((Direction)i);
         if(vector == Vector2Int.zero) continue;
         var neighbor = new Vector2Int(tile.X + vector.x, tile.Y + vector.y);
         if (IsCoordinateValid(grid, neighbor.x, neighbor.y))
         {
            var cell = grid[neighbor.x, neighbor.y];
            var neighborTile = cell.GetTile(_waitressLayer);
            if (neighborTile != null && neighborTile.IsElementTile())
            {
               neighborTile.DisableElement();
            }
         }
      }
   }

   private bool IsPathFound(Cell[,] grid, int y)
   {
      return y >= grid.GetLength(1);
   }

   private bool IsCoordinateValid(Cell[,] grid, int x, int y)
   {
      return x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1) && grid[x, y] != null;
   }

   private bool IsCellAvailable(Cell[,] grid, int x, int y, out Cell cell)
   {
      var temp = grid[x, y];
      if (temp == null)
      {
         cell = null;
         return false;
      }
      if (temp.IsTileAvailable(_waitressLayer))
      {
         cell = temp;
         return true;
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
      if (OnWaitressMadeFinalMovement != null)
      {
         OnWaitressMadeFinalMovement(waitress, waitressSlot);
         _completedWaitressCount++;
         _completedDrinkCount += waitressSlot.CurrentDrinkCount;
         CheckLevelCompleteConditionProvided();
      }

   }
   public void GameEnd(bool isWin)
   {
      if (OnGameEnd != null)
      {
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
