using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// One tile in the grid where the enemies will spawn
/// </summary>
public class TDGridObjectStart : TDGridObjectWay
{
    public TDGridObjectStart(TDGridXZ _ownGrid, int _x, int _y) : base(_ownGrid, _x, _y)
    {
    }
}
