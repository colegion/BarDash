using System.Collections.Generic;
using Helpers;
using UnityEditor;
using UnityEngine;

namespace EditorTool
{
    [CustomEditor(typeof(LevelEditor))]
    public class CustomInspector : Editor
    {
        private LevelEditor _editor;

        public override void OnInspectorGUI()
        {
            _editor = (LevelEditor)target;

            DrawPuzzleGridOptions();
            EditorGUILayout.Space();
            DrawDrinkAreaOptions();
            EditorGUILayout.Space();

            if (_editor.PuzzleCells.Count > 0)
            {
                DrawWaitressOptions();
            }

            if (_editor.DrinkCells.Count > 0)
            {
                DrawDrinkDataLists();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Clear Drink Data"))
            {
                _editor.ClearDrinkData();
            }

            EditorGUILayout.Space();
            DrawMoveCountOptions();
            DrawLevelIndexField();
        
            if (GUILayout.Button("Generate Level"))
            {
                GenerateLevel();
            }

            EditorGUILayout.Space(100);
            if (GUILayout.Button("Save Level"))
            {
                _editor.SaveLevel();
            }
            EditorUtility.SetDirty(_editor);
        }

        private void DrawPuzzleGridOptions()
        {
            EditorGUILayout.LabelField("Puzzle Grid Options", EditorStyles.boldLabel);
            _editor.puzzleColumnCount = EditorGUILayout.IntField("Puzzle Column Count (X)", _editor.puzzleColumnCount);
            _editor.puzzleRowCount = EditorGUILayout.IntField("Puzzle Row Count (Y)", _editor.puzzleRowCount);

            if (GUILayout.Button("Save Puzzle Grid"))
            {
                _editor.SavePuzzleGrid();
            }
        }

        private void DrawDrinkAreaOptions()
        {
            EditorGUILayout.LabelField("Drink Area Options", EditorStyles.boldLabel);
            _editor.drinkColumnCount = EditorGUILayout.IntField("Drink Area Column Count", _editor.drinkColumnCount);
            _editor.drinkRowCount = EditorGUILayout.IntField("Drink Area Row Count", _editor.drinkRowCount);

            if (GUILayout.Button("Save Drink Grid"))
            {
                _editor.SaveDrinkGrid();
            }

            EditorGUILayout.Space();
        }

        private void DrawWaitressOptions()
        {
            EditorGUILayout.LabelField("Waitress Tile Options", EditorStyles.boldLabel);

            int maxTiles = _editor.puzzleColumnCount * _editor.puzzleRowCount;

            if (_editor.WaitressTiles.Count > maxTiles)
            {
                _editor.WaitressTiles.RemoveRange(maxTiles, _editor.WaitressTiles.Count - maxTiles);
            }

            for (int j = 0; j < _editor.WaitressTiles.Count; j++)
            {
                EditorGUILayout.LabelField($"Waitress Tile {j + 1}", EditorStyles.boldLabel);
                _editor.WaitressTiles[j].xCoord = EditorGUILayout.IntSlider("X Coord", _editor.WaitressTiles[j].xCoord, 0,
                    _editor.puzzleColumnCount - 1);
                _editor.WaitressTiles[j].yCoord = EditorGUILayout.IntSlider("Y Coord", _editor.WaitressTiles[j].yCoord, 0,
                    _editor.puzzleRowCount - 1);
                _editor.WaitressTiles[j].tileColor =
                    (int)(GameColors)EditorGUILayout.EnumPopup("Color", (GameColors)_editor.WaitressTiles[j].tileColor);
                _editor.WaitressTiles[j].elementType = (int)(TileElementType)EditorGUILayout.EnumPopup("Element Type",
                    (TileElementType)_editor.WaitressTiles[j].elementType);

                if (GUILayout.Button("Remove Waitress Tile"))
                {
                    _editor.WaitressTiles.RemoveAt(j);
                    break;
                }

                EditorGUILayout.Space();
            }

            if (_editor.WaitressTiles.Count < maxTiles)
            {
                if (GUILayout.Button("Add Waitress Tile"))
                {
                    _editor.WaitressTiles.Add(new TileData
                    {
                        xCoord = 0,
                        yCoord = 0,
                        layer = 1,
                        tileType = 0,
                        tileColor = (int)GameColors.White,
                        elementType = (int)TileElementType.Null
                    });
                }
            }
        }

        private void DrawDrinkDataLists()
        {
            EditorGUILayout.LabelField("Drink Data Per Column", EditorStyles.boldLabel);

            // Iterate through each column
            for (int i = 0; i < _editor.drinkColumnCount; i++)
            {
                EditorGUILayout.LabelField($"Column {i + 1}", EditorStyles.boldLabel);

                // Ensure the column list exists if it's missing
                if (!_editor.TileDataPerColumn.ContainsKey(i))
                {
                    _editor.TileDataPerColumn[i] = new List<TileData>();
                }

                var tileList = _editor.TileDataPerColumn[i];

                // Iterate through the tiles in the current column
                for (int j = 0; j < tileList.Count; j++)
                {
                    EditorGUILayout.LabelField($"Tile {j + 1}", EditorStyles.boldLabel);

                    // Update tile properties
                    tileList[j].xCoord = i; // Fixed column for the tile
                    tileList[j].yCoord = EditorGUILayout.IntField("Y Coord", tileList[j].yCoord);

                    // Ensure proper enum casting for color
                    tileList[j].tileColor =
                        (int)(GameColors)EditorGUILayout.EnumPopup("Color", (GameColors)tileList[j].tileColor);

                    // Ensure proper enum casting for element type
                    tileList[j].elementType =
                        (int)(TileElementType)EditorGUILayout.EnumPopup("Element Type",
                            (TileElementType)tileList[j].elementType);

                    // Button to remove tile
                    if (GUILayout.Button("Remove Tile"))
                    {
                        tileList.RemoveAt(j);
                        break; // Exit loop after removal to avoid modifying the list during iteration
                    }

                    EditorGUILayout.Space();
                }

                // Button to add a new drink
                if (GUILayout.Button("Add Drink"))
                {
                    _editor.TileDataPerColumn[i].Add(new TileData
                    {
                        xCoord = i,
                        yCoord = 0,
                        layer = 2,
                        tileType = 1, // Assuming 1 represents the 'Drink' type
                        tileColor = (int)GameColors.White,
                        elementType = (int)TileElementType.Null
                    });
                }

                EditorGUILayout.Space();
            }
        
            if (GUILayout.Button("Save Drink Data"))
            {
                SaveDrinkData();
            }
            EditorUtility.SetDirty(_editor);
        }

        private void SaveDrinkData()
        {
            // Iterate through each column
            for (int i = 0; i < _editor.drinkColumnCount; i++)
            {
                // If the column doesn't exist in the dictionary, initialize it
                if (!_editor.TileDataPerColumn.ContainsKey(i))
                {
                    _editor.TileDataPerColumn[i] = new List<TileData>();
                }

                var tileList = _editor.TileDataPerColumn[i];

                // Do not clear the list here. Simply update the tile data.
                // Iterate through the existing tiles in the list and save updated data
                for (int j = 0; j < tileList.Count; j++)
                {
                    // Update the tile data here if necessary (e.g., if user edited any tile properties)
                    tileList[j] = new TileData
                    {
                        xCoord = i,  // Fixed x coordinate for the column
                        yCoord = tileList[j].yCoord,  // Preserve the y coordinate
                        layer = tileList[j].layer,    // Preserve the layer
                        tileType = tileList[j].tileType, // Preserve the tile type
                        tileColor = tileList[j].tileColor, // Preserve the tile color
                        elementType = tileList[j].elementType // Preserve the element type
                    };
                }

                // Optionally, log confirmation that the column data is saved
                Debug.Log($"Column {i + 1} saved with {tileList.Count} tiles.");
            }

            // Log confirmation that the data is saved for all columns
            Debug.Log("Drink data saved to TileDataPerColumn. Total columns: " + _editor.drinkColumnCount);

            // Ensure the editor data is marked as dirty to persist changes
            EditorUtility.SetDirty(_editor);
        }



        private void DrawMoveCountOptions()
        {
            EditorGUILayout.LabelField("Move Count Options", EditorStyles.boldLabel);
            _editor.MoveCount = EditorGUILayout.IntField("Move Count", _editor.MoveCount);
            if (GUILayout.Button("Save Move Count"))
            {
                _editor.SaveMoveCount();
            }
        }

        private void DrawLevelIndexField()
        {
            _editor.levelIndex = EditorGUILayout.IntField("Level Index", _editor.levelIndex);
        }

        private void GenerateLevel()
        {
            // Ensure your level generator is ready
            if (_editor.levelGenerator != null)
            {
                // Trigger the CreateLevel method from your level generator
                _editor.levelGenerator.GenerateLevelFromEditor(_editor);

                // You can also add any additional logic or feedback here
                Debug.Log("Level generation triggered!");
            }
            else
            {
                // Handle case where level generator is not assigned
                Debug.LogError("Level Generator is not assigned!");
            }
        }
    }
}