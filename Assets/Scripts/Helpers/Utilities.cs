using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Helpers
{
    public class Utilities : MonoBehaviour
    {
        [SerializeField] private List<ColorClass> gameColors;
        [SerializeField] private List<CellMeshType> cellMeshes;
        private static List<ColorClass> _colors;
        private static List<CellMeshType> _cellMeshes;
        public static Dictionary<Direction, Vector2Int> Vectors = new Dictionary<Direction, Vector2Int>()
        {
            { Direction.Up, Vector2Int.up },
            { Direction.Right, Vector2Int.right },
            { Direction.Down, Vector2Int.down },
            { Direction.Left, Vector2Int.left }
        };
        
        private void Awake()
        {
            _colors = gameColors;
            _cellMeshes = cellMeshes;
        }

        public static Color GetColor(GameColors colorEnum)
        {
            foreach (var c in _colors)
            {
                if (c.colorEnum == colorEnum)
                {
                    return c.itemColor;
                }
            }

            return Color.yellow;
        }

        public static CellOrientation GetOrientation(int x, int y, ItemType type)
        {
            var grid = GameController.Instance.GetGridByType(type);
            var width = grid.GetLength(0);
            var height = grid.GetLength(1);
            
            if (x < 0 || y < 0 || x >= width || y >= height)
                throw new ArgumentOutOfRangeException();
            
            if (x == 0 && y == 0) return CellOrientation.DownLeft;
            if (x == 0 && y == height - 1) return CellOrientation.UpLeft;
            if (x == width - 1 && y == 0) return CellOrientation.DownRight;
            if (x == width - 1 && y == height - 1) return CellOrientation.UpRight;
            
            if (x == 0) return CellOrientation.Left;
            if (x == width - 1) return CellOrientation.Right;
            if (y == 0) return CellOrientation.Down;
            if (y == height - 1) return CellOrientation.Up;
            
            return CellOrientation.Middle;
        }

        public static Mesh GetCellMesh(CellOrientation orientation)
        {
            return _cellMeshes.Find(x => x.orientation == orientation).mesh;
        }
    }

    [Serializable]
    public class ColorClass
    {
        public GameColors colorEnum;
        public Color itemColor;
    }

    public enum ItemType
    {
        WaitressArea, 
        DrinkArea
    }

    public enum GameColors
    {
        White,
        Red,
        Green,
        Blue,
        Orange,
        Brown,
        Pink
    }

    public enum TileElementType
    {
        Null,
        Cloak,
        Tip
    }
    
    public enum Direction
    {
        Up,
        Right,
        Down,
        Left,
    }

    public enum CellOrientation
    {
        Up,
        UpRight,
        Right,
        DownRight,
        Middle,
        Down,
        DownLeft,
        Left,
        UpLeft
    }

    [Serializable]
    public class CellMeshType
    {
        public CellOrientation orientation;
        public Mesh mesh;
    }

    [Serializable]
    public class LevelData
    {
        public List<AreaGridSize> areaGridSizes; 
        public List<CellData> cells;
        public List<TileData> tiles;
    }
    
    [Serializable]
    public class CellData
    {
        public int xCoord;
        public int yCoord;
        public int cellType;
    }

    
    [Serializable]
    public class TileData
    {
        public int xCoord;
        public int yCoord;
        public int layer;
        public int tileType;
        public int tileColor;
        public int elementType;
    }

    [Serializable]
    public class AreaGridSize
    {
        public int itemType;
        public int width;
        public int height;
    }
}
