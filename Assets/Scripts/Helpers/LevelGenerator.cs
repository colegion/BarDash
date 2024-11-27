using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Helpers
{
    public class LevelGenerator : MonoBehaviour
    {
        private readonly string _cellPath = "Utilities/Cell";
        private readonly string _waitressPath = "Utilities/Waitress";
        private readonly string _drinkPath = "Utilities/Drink";
        private readonly string _tilePath = "Utilities/BaseTile";
        private Dictionary<ItemType, Cell[,]> _cells = new Dictionary<ItemType, Cell[,]>();

        private const int BaseWidth = 2;
        private const float BaseXPos = -0.5f;
        public Dictionary<ItemType, Cell[,]> GenerateLevel(LevelData levelData)
        {
            var grids = levelData.areaGridSizes;
            foreach (var grid in grids)
            {
                _cells[(ItemType)grid.itemType] = new Cell[grid.width, grid.height];
                var parent = GameController.Instance.GetParentByType((ItemType)grid.itemType);
                var xPos = BaseXPos * (grid.width - BaseWidth + 1);

                parent.transform.position = new Vector3(xPos, 0, parent.transform.position.z);
            }

            var levelCells = levelData.cells;
            var cellObject = Resources.Load<Cell>(_cellPath);

            foreach (var cell in levelCells)
            {
                var parent = GameController.Instance.GetParentByType((ItemType)cell.cellType);
                var width = _cells[(ItemType)cell.cellType].GetLength(0);
                var height = _cells[(ItemType)cell.cellType].GetLength(1);
                var tempCell = Object.Instantiate(cellObject, parent);
                tempCell.ConfigureSelf(cell);
                AppendCells(tempCell, width, height);
            }

            var tiles = levelData.tiles;
            
            foreach (var tile in tiles)
            {
                var path = (ItemType)tile.tileType == ItemType.DrinkArea ? _drinkPath : _waitressPath;
                var parent = GameController.Instance.GetParentByType((ItemType)tile.tileType);
                var tileObject = Resources.Load<BaseTile>(path);
                var tempTile = Object.Instantiate(tileObject, parent);
                tempTile.ConfigureSelf(tile);
                var parentCell = GetCell(tile);
                if (parentCell != null)
                {
                    parentCell.SetTile(tempTile);
                }
            }
            //Works with PoolManager!!
            //  for (int i = 0; i < PoolManager.Instance.GetPool<BaseTile>("Drink").Count; i++)
            // {
            //     PoolManager.Instance.GetPool<BaseTile>("Drink").ElementAt(i).ConfigureSelf(tiles[i]);
            //     var parentCell = GetCell(tiles[i]);
            //     if (parentCell != null)
            //     {
            //         parentCell.SetTile(PoolManager.Instance.GetPool<BaseTile>("Drink").ElementAt(i));
            //     }
            // }

            return _cells;
        }

        public void AfterAllCellsInitialized()
        {
            foreach (var pair in _cells)
            {
                foreach (var cell in pair.Value)
                {
                    cell.SetMesh();
                }
            }
        }

        private void AppendCells(Cell cell, int width, int height)
        {
            if (!_cells.TryAdd(cell.CellArea, new Cell[width, height]))
            {
                var list = _cells[cell.CellArea];
                list[cell.X, cell.Y] = cell;
                _cells[cell.CellArea] = list;
            }
        }

        private Cell GetCell(TileData data)
        {
            var list = _cells[(ItemType)data.tileType];
            for(int i = 0; i < list.Length; i++)
            {
                for (int j = 0; j < list.Length; j++)
                {
                    if (i == data.xCoord && j == data.yCoord)
                        return list[i, j];
                }
            }

            return null;
        }
    }
}

