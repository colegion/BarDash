using System;
using System.Collections;
using System.Collections.Generic;
using GoalSystem;
using Helpers;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class GameController : MonoBehaviour
{
   [SerializeField] private SlotController slotController;
   [SerializeField] private DrinkController drinkController;
   [SerializeField] private Transform drinkParent;
   [SerializeField] private Transform waitressParent;
   private static GameController _instance;
   private LevelGenerator _levelGenerator;
   private Dictionary<ItemType, Cell[,]> _grid;
   private const int _waitressLayer = 1;
   
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
      LoadLevel();
   }

   private void LoadLevel()
   {
      // Load the JSON text file based on level index, or a default file
      TextAsset levelFile = Resources.Load<TextAsset>("Levels/TestLevel");

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
      //@todo: function incomplete. 
      List<Cell> cellsToTravel = new List<Cell>();
      bool pathFound = false;
      var listToCheck = _grid[tile.GetItemType()];
      var currentCoord = new Vector2Int(tile.X, tile.Y);

      while (!pathFound)
      {
         for (int i = 0; i < Enum.GetValues(typeof(Direction)).Length; i++)
         {
            var vector = Utilities.Vectors[(Direction)i];
            var nextCoord = new Vector2Int(currentCoord.x + vector.x, currentCoord.y + vector.y);
            if (IsCoordinateValid(listToCheck, nextCoord.x, nextCoord.y))
            {
               if (IsCellAvailable(listToCheck, nextCoord.x, nextCoord.y, out Cell cell))
               {
                  currentCoord = nextCoord;
                  cellsToTravel.Add(cell);
                  break;
               }
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
      return pathFound;
   }

   private bool IsPathFound(Cell[,] grid, int y)
   {
      return y >= grid.GetLength(1);
   }

   private bool IsCoordinateValid(Cell[,] grid, int x, int y)
   {
      return x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1) && grid[x,y] != null;
   }

   private bool IsCellAvailable(Cell[,]grid, int x, int y, out Cell cell)
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

   private void AddListeners()
   {
      
   }

   private void RemoveListeners()
   {
      
   }
}
