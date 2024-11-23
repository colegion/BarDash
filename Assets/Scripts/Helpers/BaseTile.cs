using System;
using Interfaces;
using Unity.VisualScripting;
using UnityEngine;

namespace Helpers
{
    public class BaseTile : MonoBehaviour, IMovable, IPoolable
    {
        [SerializeField] private Renderer cloakMesh;
        [SerializeField] private Renderer tileRenderer;
        private int _x;
        private int _y;
        private int _layer;
        private Cell _parentCell;
        private ItemType _tileArea;
        private GameColors _tileColor;
        private TileElementType _elementType;

        public int X => _x;
        public int Y => _y;

        public void ConfigureSelf(TileData data)
        {
            _x = data.xCoord;
            _y = data.yCoord;
            _tileArea = (ItemType)data.tileType;
            _tileColor = (GameColors)data.tileColor;
            _elementType = (TileElementType)data.elementType;
            _layer = data.layer;

            SetTransform();
            SelCloakMesh();
            //TEMPORARY MESSY SHITTY CODE
            if (data.tileType == 1)
            {
                cloakMesh.enabled = true;
                cloakMesh.material.SetColor("_SideColor", Utilities.GetColor(_tileColor));
                cloakMesh.material.SetColor("_TopColor", Utilities.GetColor(_tileColor));
            }
            else
            {
                cloakMesh.enabled = false;
                tileRenderer.material.color = Utilities.GetColor(_tileColor);
            }

        }

        private void SetTransform()
        {
            transform.SetParent(GameController.Instance.GetParentByType(_tileArea));
            transform.localPosition = new Vector3(_x, 0, _y);
        }

        private void SelCloakMesh()
        {
            cloakMesh.enabled = _elementType == TileElementType.Cloak;
        }

        public void SetParentCell(Cell parent)
        {
            _parentCell = parent;
        }

        public void DisableElement()
        {
            cloakMesh.enabled = false;
        }

        public bool IsElementTile()
        {
            return _elementType != TileElementType.Null;
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

        public void OnCreatedForPool()
        {
            Debug.Log("HHA");

        }

        public void OnAssignPool()
        {

        }

        public void OnReleasePool()
        {

        }

        public void OnDeletePool()
        {
            throw new NotImplementedException();
        }
    }
}
