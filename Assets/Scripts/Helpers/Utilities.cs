using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Helpers
{
    public class Utilities : MonoBehaviour
    {
        [SerializeField] private List<ColorClass> gameColors;

        private static List<ColorClass> _colors;

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
        }

        public static Color GetColor(GameColors colorEnum)
        {
            foreach (var c in _colors)
            {
                if (c.colorEnum == colorEnum)
                {
                    return c.ıtemColor;
                }
            }

            return Color.yellow;
        }
    }

    [Serializable]
    public class ColorClass
    {
        public GameColors colorEnum;
        public Color ıtemColor;
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

    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }

    [Serializable]
    public class LevelData
    {
        public List<CellData> cells;
        public List<TileData> tiles;
    }
    
    [Serializable]
    public class CellData
    {
        public int xCoord;
        public int yCoord;
        public ItemType cellType;
    }

    
    [Serializable]
    public class TileData
    {
        public int xCoord;
        public int yCoord;
        public int layer;
        public ItemType tileType;
        public GameColors tileColor;
    }
}
