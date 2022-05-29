using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDGridObjectEmpty : TDGridObject
{
    public bool Occupied => m_tower != null;
    private Tower m_tower;

    public TDGridObjectEmpty(TDGridXZ _ownGrid, int _x, int _y) : base(_ownGrid, _x, _y)
    {
    }

    /// <summary>
    /// Assing a tower to this grid position
    /// </summary>
    /// <param name="_tower">Tower to assign</param>
    public void SetTower(Tower _tower)
    {
        m_tower = _tower;
    }
}
