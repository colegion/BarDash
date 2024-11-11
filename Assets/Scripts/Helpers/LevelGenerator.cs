using System.Collections.Generic;
using UnityEngine;

namespace Helpers
{
    public class LevelGenerator
    {
        private readonly string _cellPath = "Utilities/Cell";
        private readonly string _tilePath = "Utilities/Tile";
        private Dictionary<ItemType, List<Cell>> _cells;
        
        public void GenerateLevel(LevelData levelData)
        {
            _cells = new Dictionary<ItemType, List<Cell>>();

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
        }

        private void AppendCells(Cell cell)
        {
            if (!_cells.TryAdd(cell.CellArea, new List<Cell>()))
            {
                var list = _cells[cell.CellArea];
                list.Add(cell);
                _cells[cell.CellArea] = list;
            }
        }

        private Cell GetCell(TileData data)
        {
            var list = _cells[data.tileType];
            return list.Find(cell => cell.X == data.xCoord && cell.Y == data.yCoord);
        }
    }
}

