using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using TileData = Helpers.TileData;

public class Tile : MonoBehaviour, IMovable
{
    protected int X;
    protected int Y;
    private int _layer;
    private Cell _parentCell;

    public void ConfigureSelf(TileData data)
    {
        X = data.xCoord;
        Y = data.yCoord;
        _layer = data.layer;
    }
    
    public void Move()
    {
        throw new System.NotImplementedException();
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
