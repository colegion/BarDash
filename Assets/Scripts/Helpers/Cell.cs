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
            CellArea = data.cellType;
            _tiles = new Dictionary<int, BaseTile>();
            SetTransform();
        }

        private void SetTransform()
        {
            transform.position = new Vector3(X, 0, Y);
        }

        public void SetTile(BaseTile baseTile)
        {
            _tiles.TryAdd(baseTile.GetLayer(), baseTile);
            baseTile.SetParentCell(this);
        }

        public bool IsTileAvailable(int layer)
        {
            return _tiles[layer] == null;
        }

        public BaseTile GetTile(int layer)
        {
            return _tiles[layer];
        }
    }
}
