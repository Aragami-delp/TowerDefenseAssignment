using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TDGridObject
{
    private TDGridXZ m_ownGrid;
    private int m_x;
    private int m_y;

    public TDGridObject(TDGridXZ _ownGrid, int _x, int _y)
    {
        m_ownGrid = _ownGrid;
        m_x = _x;
        m_y = _y;
    }
}