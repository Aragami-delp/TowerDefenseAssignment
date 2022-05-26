using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TDGridObject : MonoBehaviour
{
    private TDGridXZ m_ownGrid;
    public int X { get; private set; }
    public int Z { get; private set; }

    public TDGridObject(TDGridXZ _ownGrid, int _x, int _z)
    {
        m_ownGrid = _ownGrid;
        X = _x;
        Z = _z;
    }

    public TDGridObject Init(TDGridXZ _ownGrid, int _x, int _z)
    {
        m_ownGrid = _ownGrid;
        X = _x;
        Z = _z;
        return this;
    }
}