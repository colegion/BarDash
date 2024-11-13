using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;

public class DrinkController : MonoBehaviour
{
    private Cell[,] _drinkGrid;
    private const int DrinkLayer = 2;

    public void SetDrinkGrid(Cell[,] grid)
    {
        _drinkGrid = grid;
    }

    public List<Cell> GetBottomRow()
    {
        List<Cell> bottomCells = new List<Cell>();

        int rows = _drinkGrid.GetLength(0);
        for (int x = 0; x < rows; x++)
        {
            bottomCells.Add(_drinkGrid[x, 0]);
        }
        
        return bottomCells;
    }

    public void UpdateColumn(int columnIndex)
    {
        bool columnUpdated;

        do
        {
            columnUpdated = false;

            // Start from row 1 and move upwards to the top
            for (int row = 1; row < _drinkGrid.GetLength(0); row++)
            {
                var currentCell = _drinkGrid[row, columnIndex];

                // If the current cell has a drink, check if it can slide down
                if (!currentCell.IsTileAvailable(DrinkLayer))
                {
                    var drinkTile = currentCell.GetTile(DrinkLayer);

                    // Slide the drink down to the nearest available cell below
                    for (int belowRow = row - 1; belowRow >= 0; belowRow--)
                    {
                        var belowCell = _drinkGrid[belowRow, columnIndex];

                        if (belowCell.IsTileAvailable(DrinkLayer))
                        {
                            // Move the tile down to the available cell
                            currentCell.SetTileNull(DrinkLayer);
                            belowCell.SetTile(drinkTile);

                            // Animate the movement downwards
                            drinkTile.Move(belowCell.transform, null);

                            // Mark that the column was updated
                            columnUpdated = true;
                            break; // Stop after moving to the nearest available cell
                        }
                    }
                }
            }

        } while (columnUpdated); // Repeat until no further moves are possible in the column
    }
}
