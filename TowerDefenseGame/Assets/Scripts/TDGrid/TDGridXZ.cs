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

#if UNITY_EDITOR
    public TDGridXZ(int _width, int _height, float _cellSize, Vector3 _originPosition, bool _drawDebugGrid = false)
#else
    public TDGridXZ(int _width, int _height, float _cellSize, Vector3 _originPosition)
#endif
    {
        m_width = _width;
        m_height = _height;
        m_cellSize = _cellSize;
        m_originPos = _originPosition;

        m_gridArray = new TDGridObject[_width, _height];

        #region Debug
#if UNITY_EDITOR
        if (_drawDebugGrid)
        {
            for (int x = 0; x < m_gridArray.GetLength(0); x++)
            {
                for (int z = 0; z < m_gridArray.GetLength(1); z++)
                {
                    Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, _height), GetWorldPosition(_width, _height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(_width, 0), GetWorldPosition(_width, _height), Color.white, 100f);
        }
#endif
        #endregion
    }

    /// <summary>
    /// Gets the world position of the tile with the given XZ coordinates
    /// </summary>
    /// <param name="_gridX">Coordinate X</param>
    /// <param name="_gridZ">Coordinate Z</param>
    /// <returns>World pos of grid tile</returns>
    public Vector3 GetWorldPosition(int _gridX, int _gridZ)
    {
        return new Vector3(_gridX, 0, _gridZ) * m_cellSize + m_originPos;
    }

    /// <summary>
    /// Gets the world position of the center of the tile with the given XZ coordinates
    /// </summary>
    /// <param name="_gridX">Coordinate X</param>
    /// <param name="_gridZ">Coordinate Z</param>
    /// <returns>World pos of grid tile center</returns>
    public Vector3 GetWorldCenterPosition(int _gridX, int _gridZ)
    {
        return GetWorldPosition(_gridX, _gridZ) + new Vector3(m_cellSize, 0, m_cellSize) * .5f;
    }

    /// <summary>
    /// Gets XZ of the given world position in the grid
    /// </summary>
    /// <param name="_worldPos">World position</param>
    /// <param name="_x">X on grid</param>
    /// <param name="_z">Z on grid</param>
    public void GetXZ(Vector3 _worldPos, out int _x, out int _z)
    {
        _x = Mathf.FloorToInt((_worldPos - m_originPos).x / m_cellSize);
        _z = Mathf.FloorToInt((_worldPos - m_originPos).z / m_cellSize);
    }

    /// <summary>
    /// Returns the GridObject at a given grid position
    /// </summary>
    /// <param name="_gridX">Position in X</param>
    /// <param name="_gridZ">Position in Z</param>
    /// <returns>GridObject at given position</returns>
    public TDGridObject GetGridObject(int _gridX, int _gridZ)
    {
        if (_gridX >= 0 && _gridZ >= 0 && _gridX < m_width && _gridZ < m_height)
        {
            return m_gridArray[_gridX, _gridZ];
        }
        Debug.LogError("Out of grid range: GetValue(" + _gridX + ", " + _gridZ + ")");
        return default(TDGridObject);
    }

    /// <summary>
    /// Returns the GridObject at a given world position
    /// </summary>
    /// <param name="_worldPos">Position in world space</param>
    /// <returns>GridObject at given position</returns>
    public TDGridObject GetGridObject(Vector3 _worldPos)
    {
        int gridX, gridZ;
        GetXZ(_worldPos, out gridX, out gridZ);
        return GetGridObject(gridX, gridZ);
    }

    /// <summary>
    /// Assigns a GridObject to the given grid position
    /// </summary>
    /// <param name="_gridX">Grid position X</param>
    /// <param name="_gridZ">Grid position Z</param>
    /// <param name="_value">GridObject to assign to</param>
    /// <returns>Assigned GridObject</returns>
    public TDGridObject SetGridObject(int _gridX, int _gridZ, TDGridObject _value)
    {
        if (_gridX >= 0 && _gridZ >= 0 && _gridX < m_width && _gridZ < m_height)
        {
            m_gridArray[_gridX, _gridZ] = _value;
        }
        else
            Debug.LogError("Out of grid range: SetValue(" + _gridX + ", " + _gridZ + ", " + _value + ")");
        return _value;
    }

    /// <summary>
    /// Assigns a GridObject to the given world position
    /// </summary>
    /// <param name="_worldPos">Grid position in world space</param>
    /// <param name="_value">GridObject to assign to</param>
    /// <returns>Assigned GridObject</returns>
    public TDGridObject SetGridObject(Vector3 _worldPos, TDGridObject _value)
    {
        int gridX, gridZ;
        GetXZ(_worldPos, out gridX, out gridZ);
        SetGridObject(gridX, gridZ, _value);
        return _value;
    }

    /// <summary>
    /// Returns all 4 directly adjacent tile if they exist and are the same type
    /// </summary>
    /// <typeparam name="T">Wanted TDGridObject type</typeparam>
    /// <param name="_ownX">X position of tile in the middle</param>
    /// <param name="_ownZ">Z position of tile in the middle</param>
    /// <returns>0-4 length array of adjacent tiles</returns>
    public T[] GetAdjacentTiles<T>(int _ownX, int _ownZ) where T : TDGridObject
    {
        List<T> retVal = new List<T>(1); // Min 1 adjacent most likely
        if (_ownX > 0) // Not out of map
        {
            TDGridObject current = m_gridArray[_ownX - 1, _ownZ];
            if (typeof(T) == current.GetType()) // Same type as needed
            {
                retVal.Add(current as T);
            }
        }
        if (_ownZ > 0)
        {
            TDGridObject current = m_gridArray[_ownX, _ownZ - 1];
            if (typeof(T) == current.GetType())
            {
                retVal.Add(current as T);
            }
        }
        if (_ownX < m_width - 1)
        {
            TDGridObject current = m_gridArray[_ownX + 1, _ownZ];
            if (typeof(T) == current.GetType())
            {
                retVal.Add(current as T);
            }
        }
        if (_ownZ < m_height - 1)
        {
            TDGridObject current = m_gridArray[_ownX, _ownZ + 1];
            if (typeof(T) == current.GetType())
            {
                retVal.Add(current as T);
            }
        }
        return retVal.ToArray();
    }
}