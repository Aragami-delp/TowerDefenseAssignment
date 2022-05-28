using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Helper;
using Cinemachine;

public class TDGridManager : MonoBehaviour
{
    public static TDGridManager Instance = null;

    private TDGridXZ m_grid;
    [SerializeField] private bool m_showDebugGrid = false;
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

    [SerializeField, Header("Build:")] private LayerMask m_buildableLayers;

    private int mapX;
    private int mapZ;
    private const float TILE_SCALE = 2f;

    public bool CurrentlyPlacing = false;
    private Tower m_currentlyPlacingTower;
    private SOTower m_currentlyPlacingSOTower;
    private Vector2? m_lastBuildPos;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        char[,] mapDetails = GetMapDetails(m_way);
        mapX = mapDetails.GetLength(0);
        mapZ = mapDetails.GetLength(1);
        m_grid = new TDGridXZ(mapX, mapZ, TILE_SCALE, Vector3.zero, m_showDebugGrid);
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
        if (CurrentlyPlacing)
        {
            BuildingTower();
        }
    }

    public void BuildTower(SOTower _soTower)
    {
        m_currentlyPlacingSOTower = _soTower;
        m_currentlyPlacingTower = Instantiate(_soTower.TowerPrefab, new Vector3(0, -20, 0), Quaternion.identity); // out of sight if not on valid building spot yet
        CurrentlyPlacing = true;
    }

    public void CancelBuildTower()
    {
        Destroy(m_currentlyPlacingTower.gameObject);
        m_currentlyPlacingSOTower = null;
        m_currentlyPlacingTower = null;
        CurrentlyPlacing = false;
        m_lastBuildPos = null;
    }

    private void BuildingTower()
    {
        Vector3? mousePos = InputHelper.GetMouseWorldPosition3D(m_buildableLayers);
        if (mousePos.HasValue)
        {
            m_grid.GetXZ(mousePos.Value, out int x, out int z);
            if (ValidBuildingSpot(m_currentlyPlacingSOTower, x, z))
            {
                m_currentlyPlacingTower.transform.position = m_grid.GetWorldPosition(x, z);
                if (Input.GetMouseButtonDown(0))
                    PlaceTowerDown(m_currentlyPlacingSOTower);
            }
        }
    }

    private void PlaceTowerDown(SOTower _soTower)
    {
        Vector3? buildPos = InputHelper.GetMouseWorldPosition3D(m_buildableLayers);
        if (buildPos.HasValue)
        {
            if (ValidBuildingSpot(_soTower, buildPos)) // Not really needed but just to make sure (in case it is used elsewere)
            {
                TDGridObjectEmpty placeableGround = m_grid.GetGridObject(buildPos.Value) as TDGridObjectEmpty;
                //Tower newTower = Instantiate(_soTower.TowerPrefab, m_grid.GetWorldPosition(placeableGround.X, placeableGround.Z), Quaternion.identity);
                for (int x = placeableGround.X; x <= placeableGround.X + _soTower.Footprint.x - 1; x++)
                {
                    for (int z = placeableGround.Z; z <= placeableGround.Z + _soTower.Footprint.y - 1; z++)
                    {
                        TDGridObject otherObject = m_grid.GetGridObject(x, z);
                        if (typeof(TDGridObjectEmpty) == otherObject.GetType())
                        {
                            ((TDGridObjectEmpty)otherObject).SetTower(m_currentlyPlacingTower);
                        }
                    }
                }
                CurrentlyPlacing = false;
                m_lastBuildPos = null;
            }
        }
    }

    private bool ValidBuildingSpot(SOTower _soTower, int _x, int _z)
    {
        bool retVal = true;
        if (m_lastBuildPos != null && m_lastBuildPos.Value.x == _x && m_lastBuildPos.Value.y == _z)
            return true;
        if (_x + _soTower.Footprint.x <= mapX && _z + _soTower.Footprint.y <= mapZ)
        {

            // Check all tiles if placeable
            for (int x = _x; x <= _x + _soTower.Footprint.x - 1; x++)
            // Footprint x = 1 | 0 + 1 - 1 = 0 -> Valid   Footprint x = 2 | 0 + 1 - 2 = -1 -> Invalid
            // Footprint x = 1 | 20 + 1 - 1 = 20 -> Valid   Footprint x = 2 | 20 + 1 - 2 = 19 -> Valid
            {
                for (int z = _z; z <= _z + _soTower.Footprint.y - 1; z++)
                {
                    TDGridObject otherObject = m_grid.GetGridObject(x, z);
                    if (typeof(TDGridObjectEmpty) != otherObject.GetType() || ((TDGridObjectEmpty)otherObject).Occupied)
                    {
                        retVal = false;
                    }
                }
            }
        }
        if (retVal)
            m_lastBuildPos = new Vector2(_x, _z);

        return retVal;
    }

    private bool ValidBuildingSpot(SOTower _soTower, Vector3? _buildPos = null)
    {
        if (_buildPos == null)
        {
            _buildPos = InputHelper.GetMouseWorldPosition3D(m_buildableLayers);
        }
        if (_buildPos.HasValue)
        {
            m_grid.GetXZ(_buildPos.Value, out int _x, out int _z);
            return ValidBuildingSpot(_soTower, _x, _z);
        }
        return false;
    }
}
