using System.Collections.Generic;
using Helpers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EditorTool
{
    public class EditorLevelGenerator : MonoBehaviour
    {
        [SerializeField] private Transform waitressParent;
        [SerializeField] private Transform drinkParent;
        private readonly string _cellPath = "Utilities/Cell";
        private readonly string _waitressPath = "Utilities/Waitress";
        private readonly string _drinkPath = "Utilities/Drink";
        private readonly string _tilePath = "Utilities/BaseTile";
        private Dictionary<ItemType, Cell[,]> _cells = new Dictionary<ItemType, Cell[,]>();

        private const int BaseWidth = 2;
        private const float BaseXPos = -0.5f;

        private void GenerateTiles(Dictionary<int, List<TileData>> tileDataPerColumn, string prefabPath,
            ItemType areaType)
        {
            var tileObject = Resources.Load<BaseTile>(prefabPath);

            foreach (var columnEntry in tileDataPerColumn)
            {
                var tileList = columnEntry.Value;

                foreach (var tile in tileList)
                {
                    var parent = GameController.Instance.GetParentByType(areaType);
                    var tempTile = Object.Instantiate(tileObject, parent);
                    tempTile.ConfigureSelf(tile);

                    var parentCell = GetCell(tile);
                    if (parentCell != null)
                    {
                        parentCell.SetTile(tempTile);
                    }
                }
            }
        }
        public Dictionary<ItemType, Cell[,]> GenerateLevelFromEditor(LevelEditor editor)
        {
            foreach (Transform child in waitressParent)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in drinkParent)
            {
                Destroy(child.gameObject);
            }
            
            GenerateGridCells(ItemType.WaitressArea, editor.puzzleColumnCount, editor.puzzleRowCount);
            GenerateGridCells(ItemType.DrinkArea, editor.drinkColumnCount, editor.drinkRowCount);
            GenerateTiles(editor.WaitressTiles, _waitressPath, ItemType.WaitressArea);
            GenerateTiles(editor.TileDataPerColumn, _drinkPath, ItemType.DrinkArea);

            return _cells;
        }

        private void GenerateGridCells(ItemType areaType, int width, int height)
        {
            _cells[areaType] = new Cell[width, height];

            var parent = GameController.Instance.GetParentByType(areaType);
            var xPos = BaseXPos * (width - BaseWidth + 1);
            parent.transform.position = new Vector3(xPos, 0, parent.transform.position.z);

            var cellObject = Resources.Load<Cell>(_cellPath);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var tempCell = Object.Instantiate(cellObject, parent);
                    tempCell.ConfigureSelf(new CellData
                    {
                        xCoord = x,
                        yCoord = y,
                        cellType = (int)areaType
                    });
                    AppendCells(tempCell);
                }
            }
        }

        private void GenerateTiles(List<TileData> tileList, string prefabPath, ItemType areaType)
        {
            var tileObject = Resources.Load<BaseTile>(prefabPath);

            foreach (var tile in tileList)
            {
                var parent = GameController.Instance.GetParentByType(areaType);
                var tempTile = Object.Instantiate(tileObject, parent);
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
            if (!_cells.TryAdd(cell.CellArea, new Cell[8, 8]))
            {
                var list = _cells[cell.CellArea];
                list[cell.X, cell.Y] = cell;
                _cells[cell.CellArea] = list;
            }
        }

        private Cell GetCell(TileData data)
        {
            var list = _cells[(ItemType)data.tileType];
            return list[data.xCoord, data.yCoord];
        }
    }
}