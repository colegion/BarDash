using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    private int _x;
    private int _y;

    public int X => _x;
    public int Y => _y;

    private Dictionary<int, Tile> _tiles;

    public void ConfigureSelf(int x, int y)
    {
        _x = x;
        _y = y;
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
