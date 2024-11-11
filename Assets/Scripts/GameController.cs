using System;
using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;

public class GameController : MonoBehaviour
{
   private GameController _instance;
   private LevelGenerator _levelGenerator;
   private Dictionary<ItemType, Cell[,]> _grid;
   private const int _waitressLayer = 1;
   
   public GameController Instance
   {
      get
      {
         if (_instance == null)
         {
            _instance = new GameObject("GameController").AddComponent<GameController>();
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
      _levelGenerator = new LevelGenerator();
      LoadLevel();
   }

   private void LoadLevel()
   {
      //todo: Read json by player pref saved level index here.
      _grid = _levelGenerator.GenerateLevel(new LevelData());
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
      return x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1);
   }

   private bool IsCellAvailable(Cell[,]grid, int x, int y, out Cell cell)
   {
      var temp = grid[x, y];
      if (temp.IsTileAvailable(_waitressLayer))
      {
         cell = temp;
         return true;
      }

      cell = null;
      return false;
   }

   private void AddListeners()
   {
      
   }

   private void RemoveListeners()
   {
      
   }
}
