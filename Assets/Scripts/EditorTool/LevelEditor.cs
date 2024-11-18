using System;
using System.Collections.Generic;
using Helpers;
using UnityEngine;

namespace EditorTool
{
    public class LevelEditor : MonoBehaviour
    {
        public EditorLevelGenerator levelGenerator;
        public int puzzleColumnCount;
        public int puzzleRowCount;
        public int drinkColumnCount;
        public int drinkRowCount;

        public List<CellData> PuzzleCells { get; private set; } = new List<CellData>();
        public List<CellData> DrinkCells { get; private set; } = new List<CellData>();
        public int MoveCount { get; set; }
        public Dictionary<int, List<TileData>> TileDataPerColumn { get; private set; } = new Dictionary<int, List<TileData>>();
        public List<TileData> WaitressTiles { get; private set; } = new List<TileData>();

        private void Start()
        {
            levelGenerator = FindObjectOfType<EditorLevelGenerator>();
        }

        public void SavePuzzleGrid()
        {
            PuzzleCells.Clear();
            for (int x = 0; x < puzzleColumnCount; x++)
            {
                for (int y = 0; y < puzzleRowCount; y++)
                {
                    PuzzleCells.Add(new CellData { xCoord = x, yCoord = y, cellType = 0 });
                }
            }
            Debug.Log($"Saved Puzzle Grid with {PuzzleCells.Count} cells.");
        }

        public void SaveDrinkGrid()
        {
            DrinkCells.Clear();
            for (int x = 0; x < drinkColumnCount; x++)
            {
                for (int y = 0; y < drinkRowCount; y++)
                {
                    DrinkCells.Add(new CellData { xCoord = x, yCoord = y, cellType = 1 });
                }
            }

            // Initialize the TileDataPerColumn dictionary
            TileDataPerColumn.Clear();
            for (int i = 0; i < drinkColumnCount; i++)
            {
                TileDataPerColumn[i] = new List<TileData>();
            }

            Debug.Log($"Saved Drink Grid with {DrinkCells.Count} cells.");
        }
        
        public void SaveDrinkData()
        {
            for (int i = 0; i < drinkColumnCount; i++)
            {
                if (!TileDataPerColumn.ContainsKey(i))
                {
                    TileDataPerColumn[i] = new List<TileData>();
                }
            }

            Debug.Log("Drink data saved! Total columns: " + drinkColumnCount);
        }

        // Clear the drink data if needed
        public void ClearDrinkData()
        {
            TileDataPerColumn.Clear();
            Debug.Log("Drink data cleared!");
        }

        public void SaveMoveCount()
        {
            // Logic to save the move count (could involve writing to a file or serializing data)
            Debug.Log($"Move Count Saved: {MoveCount}");
        }
    }
}