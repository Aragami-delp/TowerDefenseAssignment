using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDGridXZ
{
    private int m_width;
    private int m_height;
    private float m_cellSize;
    private Vector3 m_originPos;
    private TDGridObject[,] m_gridArray;

    private bool m_drawDebugGrid = false;

    public TDGridXZ(int _width, int _height, float _cellSize, Vector3 _originPosition, bool _drawDebugGrid = false, Transform _debugPlaceholder = null)
    {
        m_width = _width;
        m_height = _height;
        m_cellSize = _cellSize;
        m_originPos = _originPosition;

        m_drawDebugGrid = _drawDebugGrid;

        m_gridArray = new TDGridObject[_width, _height];

        for (int x = 0; x < m_gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < m_gridArray.GetLength(1); z++)
            {
                if (m_drawDebugGrid && _debugPlaceholder != null)
                    GameObject.Instantiate(_debugPlaceholder, GetWorldPosition(x, z), Quaternion.identity);
                if (m_drawDebugGrid)
                {
                    Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.white, 100f);
                }
            }
        }

        if (m_drawDebugGrid)
        {
            Debug.DrawLine(GetWorldPosition(0, _height), GetWorldPosition(_width, _height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(_width, 0), GetWorldPosition(_width, _height), Color.white, 100f);
        }
    }

    public Vector3 GetWorldPosition(int _gridX, int _gridZ)
    {
        return new Vector3(_gridX, 0, _gridZ) * m_cellSize + m_originPos;
    }

    private Vector3 GetWorldCenterPosition(int _gridX, int _gridZ)
    {
        return GetWorldPosition(_gridX, _gridZ) + new Vector3(m_cellSize, 0, m_cellSize) * .5f;
    }

    private void GetXY(Vector3 _worldPos, out int _x, out int _z)
    {
        _x = Mathf.FloorToInt((_worldPos - m_originPos).x / m_cellSize);
        _z = Mathf.FloorToInt((_worldPos - m_originPos).z / m_cellSize);
    }

    public TDGridObject GetGridObject(int _gridX, int _gridZ)
    {
        if (_gridX >= 0 && _gridZ >= 0 && _gridX < m_width && _gridZ < m_height)
        {
            return m_gridArray[_gridX, _gridZ];
        }
        Debug.LogError("Out of grid range: GetValue(" + _gridX + ", " + _gridZ + ")");
        return default(TDGridObject);
    }

    public TDGridObject GetGridObject(Vector3 _worldPos)
    {
        int gridX, gridZ;
        GetXY(_worldPos, out gridX, out gridZ);
        return GetGridObject(gridX, gridZ);
    }

    public void SetGridObject(int _gridX, int _gridZ, TDGridObject _value)
    {
        if (_gridX >= 0 && _gridZ >= 0 && _gridX < m_width && _gridZ < m_height)
        {
            m_gridArray[_gridX, _gridZ] = _value;
        }
        Debug.LogError("Out of grid range: SetValue(" + _gridX + ", " + _gridZ + ", " + _value + ")");
    }

    public void SetGridObject(Vector3 _worldPos, TDGridObject value)
    {
        int gridX, gridZ;
        GetXY(_worldPos, out gridX, out gridZ);
        SetGridObject(gridX, gridZ, value);
    }
}