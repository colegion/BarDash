using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Helpers
{
    public class LevelGenerator
    {
        private readonly string _cellPath = "Utilities/Cell";
        private readonly string _tilePath = "Utilities/Tile";
        private Dictionary<ItemType, Cell[,]> _cells;
        public Dictionary<ItemType, Cell[,]> GenerateLevel(LevelData levelData)
        {
            _cells = new Dictionary<ItemType, Cell[,]>();

            var levelCells = levelData.cells;
            var cellObject = Resources.Load<Cell>(_cellPath);

            foreach (var cell in levelCells)
            {
                var tempCell = Object.Instantiate(cellObject);
                tempCell.ConfigureSelf(cell);
                AppendCells(tempCell);
            }

            var tiles = levelData.tiles;
            var tileObject = Resources.Load<BaseTile>(_tilePath);
            foreach (var tile in tiles)
            {
                var tempTile = Object.Instantiate(tileObject);
                tempTile.ConfigureSelf(tile);
                var parentCell = GetCell(tile);
                if (parentCell != null)
                {
                    parentCell.SetTile(tempTile);
                }
            }

            return _cells;
        }

        private void AppendCells(Cell cell)
        {
            if (!_cells.TryAdd(cell.CellArea, new Cell[,]{}))
            {
                var list = _cells[cell.CellArea];
                list[cell.X, cell.Y] = cell;
                _cells[cell.CellArea] = list;
            }
        }

        private Cell GetCell(TileData data)
        {
            var list = _cells[data.tileType];
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

