using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Helper;
using Cinemachine;

public class TDGridManager : MonoBehaviour
{
    private TDGridXZ m_grid;
    [SerializeField, Header("Map:")] private Transform m_mapParent;
    [SerializeField] private TDGridObjectEmpty m_floorTile;
    [SerializeField] private TDGridObjectWay m_wayTile;
    [SerializeField] private TDGridObjectStart m_startWayTile;
    [SerializeField] private TDGridObjectFinish m_endWayTile;
    [SerializeField] private float m_wayShift;
    [SerializeField, ReadOnly] private char m_tileIs = '0';
    [SerializeField, ReadOnly] private char m_wayIs = '1';
    [SerializeField, ReadOnly] private char m_startIs = '2';
    [SerializeField, ReadOnly] private char m_endIs = '3';
    [SerializeField, TextArea(5, 50)] private string m_way;

    [SerializeField, Header("Build:")] private Tower m_towerPrefab;
    [SerializeField] private LayerMask m_buildableLayers;

    private int mapX;
    private int mapZ;
    private const float TILE_SCALE = 2f;

    private void Start()
    {
        char[,] mapDetails = GetMapDetails(m_way);
        mapX = mapDetails.GetLength(0);
        mapZ = mapDetails.GetLength(1);
        m_grid = new TDGridXZ(mapX, mapZ, TILE_SCALE, Vector3.zero, true);
        PopulateMap(m_grid, mapDetails);
    }

    private char[,] GetMapDetails(string _mapData)
    {
        string[] splitted = _mapData.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        char[,] mapDetails = new char[splitted.Length, splitted.Max(w => w.Length)];
        for (int x = 0; x<splitted.Length; x++)
        {
            string row = splitted[x];
            for (int z = 0; z < splitted[x].Length; z++)
            {
                mapDetails[x, z] = splitted[x][z];
            }
        }
        return mapDetails;
    }

    private void PopulateMap(TDGridXZ _grid, char[,] _mapDetails)
    {
        for (int x = 0; x < mapX; x++)
        {
            for (int z = 0; z < mapZ; z++)
            {
                switch (_mapDetails[x, z])
                {
                    case '0':
                        _grid.SetGridObject(x, z, Instantiate(m_floorTile, _grid.GetWorldPosition(x, z), Quaternion.identity, m_mapParent).Init(_grid, x, z));
                        break;
                    case '1':
                        _grid.SetGridObject(x, z, Instantiate(m_wayTile, _grid.GetWorldPosition(x, z) + new Vector3(0, m_wayShift, 0), Quaternion.identity, m_mapParent).Init(_grid, x, z));
                        break;
                    case '2':
                        _grid.SetGridObject(x, z, Instantiate(m_startWayTile, _grid.GetWorldPosition(x, z) + new Vector3(0, m_wayShift, 0), Quaternion.identity, m_mapParent).Init(_grid, x, z));
                        break;
                    case '3':
                        _grid.SetGridObject(x, z, Instantiate(m_endWayTile, _grid.GetWorldPosition(x, z) + new Vector3(0, m_wayShift, 0), Quaternion.identity, m_mapParent).Init(_grid, x, z));
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void Update()
    {
        BuildTower(m_towerPrefab);
    }

    private void BuildTower(Tower _towerPrefab)
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 buildPos = InputHelper.GetMouseWorldPosition3D(m_buildableLayers);
            TDGridObject buildGround = m_grid.GetGridObject(buildPos);
            if (typeof(TDGridObjectEmpty) == buildGround.GetType())
            {
                TDGridObjectEmpty placeableGround = buildGround as TDGridObjectEmpty;
                placeableGround.PlaceTower(Instantiate(_towerPrefab, m_grid.GetWorldPosition(placeableGround.X, placeableGround.Z), Quaternion.identity));
            }
        }
    }
}
