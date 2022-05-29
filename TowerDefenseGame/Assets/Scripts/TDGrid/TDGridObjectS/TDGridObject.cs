using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TDGridObject : MonoBehaviour
{
    /// <summary>
    /// Own underlying grid
    /// </summary>
    protected TDGridXZ m_ownGrid;
    /// <summary>
    /// Own grid position in X
    /// </summary>
    public int X { get; private set; }
    /// <summary>
    /// Own grid position in Z
    /// </summary>
    public int Z { get; private set; }

    /// <summary>
    /// Creates a new grid object
    /// </summary>
    /// <param name="_ownGrid">Own grid</param>
    /// <param name="_x">Own x coordiante in grid</param>
    /// <param name="_z">Own z coordiante in grid</param>
    public TDGridObject(TDGridXZ _ownGrid, int _x, int _z)
    {
        m_ownGrid = _ownGrid;
        X = _x;
        Z = _z;
    }

    /// <summary>
    /// Initializes this grid object
    /// </summary>
    /// <param name="_ownGrid">Own grid</param>
    /// <param name="_x">Own x coordiante in grid</param>
    /// <param name="_z">Own z coordiante in grid</param>
    /// <returns>Returns itself</returns>
    public virtual TDGridObject Init(TDGridXZ _ownGrid, int _x, int _z)
    {
        m_ownGrid = _ownGrid;
        X = _x;
        Z = _z;
        return this;
    }

    public Vector3 WorldPos => m_ownGrid.GetWorldPosition(X, Z);

    public Vector3 WorldCenterPos => m_ownGrid.GetWorldCenterPosition(X, Z);

    public override string ToString()
    {
        return "X: " + X + ", Z: " + Z + ", " + base.ToString();
    }
}