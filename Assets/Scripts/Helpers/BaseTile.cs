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
        protected ItemType TileArea;
        private GameColors _tileColor;
        private TileElementType _elementType;
        protected bool isMoving;
        public int X => _x;
        public int Y => _y;

        public void ConfigureSelf(TileData data)
        {
            _x = data.xCoord;
            _y = data.yCoord;
            TileArea = (ItemType)data.tileType;
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
                SetTransform(new Vector3(_x, -0.329f, _y));
            }
            else
            {
                cloakMesh.enabled = false;
                tileRenderer.material.color = Utilities.GetColor(_tileColor);
            }

        }

        public virtual void SetTransform()
        {
            transform.SetParent(GameController.Instance.GetParentByType(TileArea));
            transform.localPosition = new Vector3(_x, 0, _y);
        }
        public virtual void SetTransform(Vector3 position)
        {
            transform.localPosition = position;
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

        public void SetIsMoving(bool move)
        {
            isMoving = move;
        }

        public bool IsElementTile()
        {
            return _elementType != TileElementType.Null;
        }

        public int GetLayer()
        {
            return _layer;
        }

        public bool IsMoving()
        {
            return isMoving;
        }

        public ItemType GetItemType()
        {
            return TileArea;
        }

        public GameColors GetTileColor()
        {
            return _tileColor;
        }

        public virtual void Move(Transform target, Action onComplete = null)
        {
            throw new NotImplementedException();
        }
        public virtual void Move(Vector3 targetVector)
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
