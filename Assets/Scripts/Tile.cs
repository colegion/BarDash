using System.Collections;
using System.Collections.Generic;
using Helpers;
using Interfaces;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using TileData = Helpers.TileData;

public class Tile : MonoBehaviour
{
    protected int X;
    protected int Y;
    private int _layer;
    private Cell _parentCell;
    private ItemType _tileArea;

    public void ConfigureSelf(TileData data)
    {
        X = data.xCoord;
        Y = data.yCoord;
        _tileArea = data.tileType;
        _layer = data.layer;
    }
    
    public void SetParentCell(Cell parent)
    {
        _parentCell = parent;
    }
    
    public int GetLayer()
    {
        return _layer;
    }
}
