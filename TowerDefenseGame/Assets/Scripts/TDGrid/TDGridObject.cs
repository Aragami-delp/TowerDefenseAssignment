using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDGridObject
{
    private bool m_forbidden = false;
    private IGridBuilding m_gridBuilding;

    public bool Buildable => m_gridBuilding != null;
}