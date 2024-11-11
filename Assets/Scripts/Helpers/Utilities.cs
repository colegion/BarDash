using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Helpers
{
    public class Utilities : MonoBehaviour
    {
        [SerializeField] private List<ColorClass> gameColors;
    }

    [Serializable]
    public class ColorClass
    {
        public GameColors colorEnum;
        public Color ıtemColor;
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
    }

    
    [Serializable]
    public class TileData
    {
        public int xCoord;
        public int yCoord;
        public int layer;
    }
}
