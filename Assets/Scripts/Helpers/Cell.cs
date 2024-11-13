using System.Collections.Generic;
using UnityEngine;

namespace Helpers
{
    public class Cell : MonoBehaviour
    {
        public int X { get; private set; }

        public int Y { get; private set; }

        public ItemType CellArea { get; private set; }

        private Dictionary<int, BaseTile> _tiles;

        public void ConfigureSelf(CellData data)
        {
            X = data.xCoord;
            Y = data.yCoord;
            CellArea = (ItemType)data.cellType;
            _tiles = new Dictionary<int, BaseTile>();
            SetTransform();
        }

        private void SetTransform()
        {
            transform.SetParent(GameController.Instance.GetParentByType(CellArea));
            transform.localPosition = new Vector3(X, -1, Y);
        }

        public void SetTile(BaseTile baseTile)
        {
            _tiles.TryAdd(baseTile.GetLayer(), baseTile);
            baseTile.SetParentCell(this);
        }

        public void SetTileNull(int layer)
        {
            if (_tiles[layer] != null)
            {
                var tile = _tiles[layer];
                tile.SetParentCell(null);
                _tiles[layer] = null;
            }
        }

        public bool IsTileAvailable(int layer)
        {
            return _tiles[layer] == null;
        }

        public BaseTile GetTile(int layer)
        {
            return _tiles.GetValueOrDefault(layer);
        }
    }
}
