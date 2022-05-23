using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDGrid<T>
{
    private int m_width;
    private int m_height;
    private float m_cellSize;
    private Vector3 m_originPos;
    private TDGridObject[,] m_gridArray;

    private bool m_drawDebugGrid = false;

    public TDGrid(int _width, int _height, float _cellSize, Vector3 _originPosition, bool _drawDebugGrid = false, Transform _debugPlaceholder = null)
    {
        m_width = _width;
        m_height = _height;
        m_cellSize = _cellSize;
        m_originPos = _originPosition;

        m_drawDebugGrid = _drawDebugGrid;

        m_gridArray = new TDGridObject[_width, _height];

        for (int x = 0; x < m_gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < m_gridArray.GetLength(1); y++)
            {
                if (m_drawDebugGrid && _debugPlaceholder != null)
                    GameObject.Instantiate(_debugPlaceholder, GetWorldCenterPosition(x, y), Quaternion.identity);
                if (m_drawDebugGrid)
                {
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }
            }
        }

        if (m_drawDebugGrid)
        {
            Debug.DrawLine(GetWorldPosition(0, _height), GetWorldPosition(_width, _height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(_width, 0), GetWorldPosition(_width, _height), Color.white, 100f);
        }
    }

    private Vector3 GetWorldPosition(int _gridX, int _gridY)
    {
        return new Vector3(_gridX, _gridY) * m_cellSize + m_originPos;
    }

    private Vector3 GetWorldCenterPosition(int _gridX, int _gridY)
    {
        return GetWorldPosition(_gridX, _gridY) + new Vector3(m_cellSize, m_cellSize) * .5f;
    }

    private void GetXY(Vector3 _worldPos, out int _x, out int _y)
    {
        _x = Mathf.FloorToInt((_worldPos - m_originPos).x / m_cellSize);
        _y = Mathf.FloorToInt((_worldPos - m_originPos).y / m_cellSize);
    }

    public TDGridObject GetGridObject(int _gridX, int _gridY)
    {
        if (_gridX >= 0 && _gridY >= 0 && _gridX < m_width && _gridY < m_height)
        {
            return m_gridArray[_gridX, _gridY];
        }
        Debug.LogError("Out of grid range: GetValue(" + _gridX + ", " + _gridY + ")");
        return default(TDGridObject);
    }

    public TDGridObject GetGridObject(Vector3 _worldPos)
    {
        int gridX, gridY;
        GetXY(_worldPos, out gridX, out gridY);
        return GetGridObject(gridX, gridY);
    }

    public void SetGridObject(int _gridX, int _gridY, TDGridObject _value)
    {
        if (_gridX >= 0 && _gridY >= 0 && _gridX < m_width && _gridY < m_height)
        {
            m_gridArray[_gridX, _gridY] = _value;
        }
        Debug.LogError("Out of grid range: SetValue(" + _gridX + ", " + _gridY + ", " + _value + ")");
    }

    public void SetGridObject(Vector3 _worldPos, TDGridObject value)
    {
        int gridX, gridY;
        GetXY(_worldPos, out gridX, out gridY);
        SetGridObject(gridX, gridY, value);
    }
}
