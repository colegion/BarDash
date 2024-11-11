using System.Collections.Generic;
using UnityEngine;

namespace Helpers
{
    public class LevelGenerator
    {
        private readonly string _cellPath = "Utilities/Cell";
        private readonly string _tilePath = "Utilities/Tile";
        private List<Cell> _cells;
        
        public void GenerateLevel(LevelData levelData)
        {
            _cells = new List<Cell>();

            var levelCells = levelData.cells;
            var cellObject = Resources.Load<Cell>(_cellPath);

            foreach (var cell in levelCells)
            {
                var tempCell = Object.Instantiate(cellObject);
                tempCell.ConfigureSelf(cell.xCoord, cell.yCoord);
                _cells.Add(tempCell);
            }

            var tiles = levelData.tiles;
            var tileObject = Resources.Load<Tile>(_tilePath);
            foreach (var tile in tiles)
            {
                var tempTile = Object.Instantiate(tileObject);
                tempTile.ConfigureSelf(tile);
                var parentCell = GetCellByCoordinate(tile.xCoord, tile.yCoord);
                if (parentCell != null)
                {
                    parentCell.SetTile(tempTile);
                }
            }
        }

        private Cell GetCellByCoordinate(int x, int y)
        {
            return _cells.Find(cell => cell.X == x && cell.Y == y);
        }
    }
}

