using Interfaces;
using UnityEngine;

namespace Helpers
{
    public class BaseTile : MonoBehaviour, IMovable
    {
        protected int X;
        protected int Y;
        private int _layer;
        private Cell _parentCell;
        private ItemType _tileArea;
        private GameColors _tileColor;

        public void ConfigureSelf(TileData data)
        {
            X = data.xCoord;
            Y = data.yCoord;
            _tileArea = data.tileType;
            _tileColor = data.tileColor;
            _layer = data.layer;
            SetTransform();
        }
        
        private void SetTransform()
        {
            transform.position = new Vector3(X, 0, Y);
        }
    
        public void SetParentCell(Cell parent)
        {
            _parentCell = parent;
        }
    
        public int GetLayer()
        {
            return _layer;
        }

        public virtual void Move()
        {
            throw new System.NotImplementedException();
        }
    }
}
