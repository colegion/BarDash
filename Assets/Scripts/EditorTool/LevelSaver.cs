using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Helpers;
using UnityEditor;
using UnityEngine;

namespace EditorTool
{
    public class LevelSaver : MonoBehaviour
    {
        public void SaveLevel(LevelEditor editor)
        {
            LevelData levelData = new LevelData
            {
                areaGridSizes = new List<AreaGridSize>
                {
                    new AreaGridSize
                    {
                        itemType = 0, // Puzzle Area
                        width = editor.puzzleColumnCount,
                        height = editor.puzzleRowCount
                    },
                    new AreaGridSize
                    {
                        itemType = 1, // Drink Area
                        width = editor.drinkColumnCount,
                        height = editor.drinkRowCount
                    }
                },
                cells = new List<CellData>(),
                tiles = new List<TileData>()
            };

            levelData.cells.AddRange(editor.PuzzleCells);
            levelData.cells.AddRange(editor.DrinkCells);
            
            foreach (var column in editor.TileDataPerColumn)
            {
                levelData.tiles.AddRange(column.Value);
            }
            levelData.tiles.AddRange(editor.WaitressTiles);
            string json = JsonUtility.ToJson(levelData, true);
            string path = "Assets/Resources/Levels";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filePath = $"{path}/Level{editor.levelIndex}.json";
            File.WriteAllText(filePath, json);
            
            AssetDatabase.Refresh();

            Debug.Log("Level saved at: " + filePath);
        }
    }
}
