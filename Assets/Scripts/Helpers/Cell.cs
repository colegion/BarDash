using System.Collections.Generic;
using UnityEngine;

namespace Helpers
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private MeshFilter meshFilter;
        public int X { get; private set; }
        public int Y { get; private set; }

        public ItemType CellArea { get; private set; }
        private Dictionary<int, BaseTile> _tiles;

        public void ConfigureSelf(CellData data)
        {
            X = data.xCoord;
            Y = data.yCoord;
            CellArea = (ItemType)data.cellType;
            _tiles = new Dictionary<int, BaseTile>()
            {
                {0, null},
                {1, null},
                {2, null},
            };
            SetTransform();
        }

        private void SetTransform()
        {
            transform.SetParent(GameController.Instance.GetParentByType(CellArea));
            transform.localPosition = new Vector3(X, -1, Y);
        }

        public void SetMesh()
        {
            var orientation = Utilities.GetOrientation(X, Y, CellArea);
            var mesh = Utilities.GetCellMesh(orientation);
            meshFilter.mesh = mesh;
        }

        public void SetTile(BaseTile baseTile)
        {
            _tiles[baseTile.GetLayer()] = baseTile;
            baseTile.SetParentCell(this);
        }

        public void SetTileNull(int layer)
        {
            if (!_tiles.ContainsKey(layer)) return;
            
            var tile = _tiles.GetValueOrDefault(layer);
            if (tile != null)
            {
                tile.SetParentCell(null);
                _tiles[layer] = null;
            }
        }

        public bool IsTileAvailable(int layer)
        {
            if (!_tiles.ContainsKey(layer)) return true;
            return _tiles[layer] == null;
        }

        public BaseTile GetTile(int layer)
        {
            return _tiles.GetValueOrDefault(layer);
        }

        public Vector3 GetWorldPosition()
        {
            return transform.position;
        }
    }
}