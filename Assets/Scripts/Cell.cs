using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public int X { get; private set; }

    public int Y { get; private set; }

    public ItemType CellArea { get; private set; }

    private Dictionary<int, Tile> _tiles;

    public void ConfigureSelf(CellData data)
    {
        X = data.xCoord;
        Y = data.yCoord;
        CellArea = data.cellType;
        _tiles = new Dictionary<int, Tile>();
    }

    public void SetTile(Tile tile)
    {
        _tiles.TryAdd(tile.GetLayer(), tile);
        tile.SetParentCell(this);
    }

    public Tile GetTile(int layer)
    {
        return _tiles[layer];
    }
}
