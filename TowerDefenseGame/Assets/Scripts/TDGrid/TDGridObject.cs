using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDGridObject<T>
{
    private T m_gridContent;

    public bool HasContent => m_gridContent != null;
}