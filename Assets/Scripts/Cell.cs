using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    protected int X;
    protected int Y;

    private Dictionary<int, Tile> _tiles;

    public void ConfigureSelf(int x, int y)
    {
        X = x;
        Y = y;
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
