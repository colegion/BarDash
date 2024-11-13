using System;
using Interfaces;
using UnityEngine;

namespace Helpers
{
    public class BaseTile : MonoBehaviour, IMovable
    {
        [SerializeField] private MeshRenderer tileRenderer;
        private int _x;
        private int _y;
        private int _layer;
        private Cell _parentCell;
        private ItemType _tileArea;
        private GameColors _tileColor;

        public int X => _x;
        public int Y => _y;

        public void ConfigureSelf(TileData data)
        {
            _x = data.xCoord;
            _y = data.yCoord;
            _tileArea = (ItemType)data.tileType;
            _tileColor = (GameColors)data.tileColor;
            _layer = data.layer;
            SetTransform();
            tileRenderer.material.color = Utilities.GetColor(_tileColor);
        }
        
        private void SetTransform()
        {
            transform.SetParent(GameController.Instance.GetParentByType(_tileArea));
            transform.localPosition = new Vector3(_x, 0, _y);
        }
    
        public void SetParentCell(Cell parent)
        {
            _parentCell = parent;
        }
    
        public int GetLayer()
        {
            return _layer;
        }

        public ItemType GetItemType()
        {
            return _tileArea;
        }

        public GameColors GetTileColor()
        {
            return _tileColor;
        }

        public virtual void Move(Transform target, Action onComplete = null)
        {
            throw new NotImplementedException();
        }
    }
}
