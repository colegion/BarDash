using System;
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

    public void UpdateColumn(int columnIndex, Action onComplete)
    {
        bool columnUpdated;

        do
        {
            columnUpdated = false;
            
            for (int row = 1; row < _drinkGrid.GetLength(1); row++)
            {
                var currentCell = _drinkGrid[columnIndex, row];
                if (!currentCell.IsTileAvailable(DrinkLayer))
                {
                    var drinkTile = currentCell.GetTile(DrinkLayer);
                    for (int belowRow = row - 1; belowRow >= 0; belowRow--)
                    {
                        var belowCell = _drinkGrid[columnIndex, belowRow];

                        if (belowCell.IsTileAvailable(DrinkLayer))
                        {
                            currentCell.SetTileNull(DrinkLayer);
                            belowCell.SetTile(drinkTile);
                            drinkTile.Move(new Vector3(belowCell.transform.position.x,-0.329f,belowCell.transform.position.z));
                            columnUpdated = true;
                            break;
                        }
                    }
                }
            }

        } while (columnUpdated);
        
        onComplete?.Invoke();
    }
}
