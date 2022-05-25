using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Helper;

public class TDGridManager : MonoBehaviour
{
    private TDGridXZ<IGridBuilding> m_grid;
    [SerializeField] private Transform m_mapParent;
    [SerializeField] private Transform m_floorTile;
    [SerializeField] private Transform m_wayTile;
    [SerializeField] private Transform m_startWayTile;
    [SerializeField] private Transform m_endWayTile;
    [SerializeField, ReadOnly] private char m_tileIs = '0';
    [SerializeField, ReadOnly] private char m_wayIs = '1';
    [SerializeField, ReadOnly] private char m_startIs = '2';
    [SerializeField, ReadOnly] private char m_endIs = '3';
    [SerializeField, TextArea(5, 50)] private string m_way;

    private void Start()
    {
        char[,] mapDetails = GetMapDetails(m_way);
        m_grid = new TDGridXZ<IGridBuilding>(mapDetails.GetLength(0), mapDetails.GetLength(1), 2f, Vector3.zero);
        PopulateMap(m_grid, mapDetails);
    }

    private char[,] GetMapDetails(string _mapData)
    {
        string[] splitted = _mapData.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        char[,] mapDetails = new char[splitted.Length, splitted.Max(w => w.Length)];
        for (int x = 0; x<splitted.Length; x++)
        {
            string row = splitted[x];
            for (int y = 0; y < splitted[x].Length; y++)
            {
                mapDetails[x, y] = splitted[x][y];
            }
        }
        return mapDetails;
    }

    private void PopulateMap(TDGridXZ<IGridBuilding> _grid, char[,] _mapDetails)
    {
        for (int x = 0; x < _mapDetails.GetLength(0); x++)
        {
            for (int y = 0; y < _mapDetails.GetLength(1); y++)
            {
                switch (_mapDetails[x,y])
                {
                    case '0':
                        Instantiate(m_floorTile, _grid.GetWorldPosition(x, y), Quaternion.identity, m_mapParent);
                        break;
                    case '1':
                        Instantiate(m_wayTile, _grid.GetWorldPosition(x, y), Quaternion.identity, m_mapParent);
                        break;
                    case '2':
                        Instantiate(m_startWayTile, _grid.GetWorldPosition(x, y), Quaternion.identity, m_mapParent);
                        break;
                    case '3':
                        Instantiate(m_endWayTile, _grid.GetWorldPosition(x, y), Quaternion.identity, m_mapParent);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
