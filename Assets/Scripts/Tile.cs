using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    protected int X;
    protected int Y;
    private int _layer;
    private Cell _parentCell;

    public void ConfigureSelf(int x, int y)
    {
        X = x;
        Y = y;
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
