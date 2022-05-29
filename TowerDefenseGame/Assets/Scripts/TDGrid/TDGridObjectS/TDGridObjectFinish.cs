using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// One tile in the grid where the enemies go to
/// </summary>
public class TDGridObjectFinish : TDGridObjectWay
{
    public TDGridObjectFinish(TDGridXZ _ownGrid, int _x, int _y) : base(_ownGrid, _x, _y)
    {
    }
}
