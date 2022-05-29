using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// One tile in the grid where the enmies can walk on
/// </summary>
public class TDGridObjectWay : TDGridObject
{
    public TDGridObjectWay(TDGridXZ _ownGrid, int _x, int _y) : base(_ownGrid, _x, _y)
    {
    }
}
