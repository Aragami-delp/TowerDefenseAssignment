using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Helper;
using Cinemachine;

public class TDGridManager : MonoBehaviour
{
    #region Inspector Vars
#if UNITY_EDITOR
    [SerializeField, Tooltip("Shows Gizmo outlines of the grid")] private bool m_showDebugGrid = false;
#endif
    [SerializeField, Tooltip("To organize all grid tiles"), Header("Map:")] private Transform m_mapParent;
    [SerializeField, Tooltip("Prefab for basic grid tile")] private TDGridObjectEmpty m_floorTile;
    [SerializeField, Tooltip("Prefab for enemy walking tile")] private TDGridObjectWay m_wayTile;
    [SerializeField, Tooltip("Prefab for enemy spawn tile")] private TDGridObjectStart m_startWayTile;
    [SerializeField, Tooltip("Prefab for enemy target tile")] private TDGridObjectFinish m_endWayTile;
    [SerializeField, Tooltip("Shifting the way height to make it stand out more")] private float m_wayShift;
    [SerializeField, ReadOnly, Tooltip("Symbol for Floor Tile")] private char m_tileIs = '0';
    [SerializeField, ReadOnly, Tooltip("Symbol for Way Tile")] private char m_wayIs = '1';
    [SerializeField, ReadOnly, Tooltip("Symbol for Start Tile")] private char m_startIs = '2';
    [SerializeField, ReadOnly, Tooltip("Symbol for End Tile")] private char m_endIs = '3';
    [SerializeField, TextArea(5, 50)] private string m_way;

    [SerializeField, Header("Build:"), Tooltip("Layers that may be used to determine grid tiles")] private LayerMask m_buildableLayers;
    #endregion

    #region Private Vars
    /// <summary>
    /// Underlying grid
    /// </summary>
    private TDGridXZ m_grid;
    /// <summary>
    /// Max value for X using the current Way
    /// </summary>
    private int maxMapX;
    /// <summary>
    /// Max value for Z using the current Way
    /// </summary>
    private int maxMapZ;
    /// <summary>
    /// Distance between each tile center
    /// </summary>
    private const float TILE_SCALE = 2f;

    /// <summary>
    /// Currently in placing towers mode
    /// </summary>
    private bool m_currentlyPlacing = false;
    /// <summary>
    /// The currently instantiated tower to move arround before placing
    /// </summary>
    private Tower m_currentlyPlacingTower;
    /// <summary>
    /// SO of the currenlty selected Tower
    /// </summary>
    private SOTower m_currentlyPlacingSOTower;
    /// <summary>
    /// Saves last position to avoid validating the same coordinates multiple times
    /// </summary>
    private Vector2? m_lastBuildPos;

    /// <summary>
    /// All tiles the enemies can use including start and finish
    /// </summary>
    private List<TDGridObjectWay> m_enemyWalkPathTiles = new List<TDGridObjectWay>();
    /// <summary>
    /// Enemies spawn here
    /// </summary>
    private TDGridObjectStart m_enemyStart;
    /// <summary>
    /// Enemies go here
    /// </summary>
    private TDGridObjectFinish m_enemyFinish;
    #endregion

    #region Singleton
    public static TDGridManager Instance = null;
    private void Awake()
    {
        if (Instance)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }
    #endregion

    #region Mono Callbacks
    private void Start()
    {
        char[,] mapDetails = GetMapDetails(m_way);
        maxMapX = mapDetails.GetLength(0);
        maxMapZ = mapDetails.GetLength(1);
#if UNITY_EDITOR
        m_grid = new TDGridXZ(maxMapX, maxMapZ, TILE_SCALE, Vector3.zero, m_showDebugGrid);
#else
        m_grid = new TDGridXZ(mapX, mapZ, TILE_SCALE, Vector3.zero);
#endif
        PopulateMap(m_grid, mapDetails);
        DetermineEnemyPath();
    }

    private void Update()
    {
        if (m_currentlyPlacing)
        {
            BuildingTower();
        }
    }
    #endregion

    #region Build Map
    /// <summary>
    /// Loads the  multiple string map data into a 2D char array
    /// </summary>
    /// <param name="_mapData">Multiple String with data</param>
    /// <returns>Single Character data for each map pos</returns>
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

    /// <summary>
    /// Builds the map based on given data
    /// </summary>
    /// <param name="_grid">Underlying grid</param>
    /// <param name="_mapDetails">Data of the map</param>
    private void PopulateMap(TDGridXZ _grid, char[,] _mapDetails)
    {
        for (int x = 0; x < maxMapX; x++)
        {
            for (int z = 0; z < maxMapZ; z++)
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
                        if (m_enemyStart == null)
                            m_enemyStart = (TDGridObjectStart)_grid.SetGridObject(x, z, Instantiate(m_startWayTile, _grid.GetWorldPosition(x, z) + new Vector3(0, m_wayShift, 0), Quaternion.identity, m_mapParent).Init(_grid, x, z));
                        else
                            Debug.LogWarning("Start tile already exists. Skipping");
                        break;
                    case '3':
                        if (m_enemyFinish == null)
                            m_enemyFinish = (TDGridObjectFinish)_grid.SetGridObject(x, z, Instantiate(m_endWayTile, _grid.GetWorldPosition(x, z) + new Vector3(0, m_wayShift, 0), Quaternion.identity, m_mapParent).Init(_grid, x, z));
                        else
                            Debug.LogWarning("Finish tile already exists. Skipping");
                        break;
                    default:
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Creates a list with the order of the enemy path
    /// </summary>
    private void DetermineEnemyPath()
    {
        TDGridObjectWay currentTile = m_enemyStart;
        m_enemyWalkPathTiles.Add(currentTile);
        while (currentTile != null)
        {
            TDGridObjectWay[] nextTiles = m_grid.GetAdjacentTiles<TDGridObjectWay>(currentTile.X, currentTile.Z);
            currentTile = null;
            foreach (TDGridObjectWay nextWay in nextTiles)
            {
                if (!m_enemyWalkPathTiles.Contains(nextWay))
                {
                    m_enemyWalkPathTiles.Add(nextWay);
                    currentTile = nextWay;
                    break;
                }
            }
        }
        m_enemyWalkPathTiles.Add(m_enemyFinish);
    }
    #endregion

    #region BuildingMode
    /// <summary>
    /// Start building a tower
    /// </summary>
    /// <param name="_soTower">SO of the tower to build</param>
    public void BuildTower(SOTower _soTower)
    {
        m_currentlyPlacingSOTower = _soTower;
        m_currentlyPlacingTower = Instantiate(_soTower.TowerPrefab, new Vector3(0, -20, 0), Quaternion.identity); // out of sight if not on valid building spot yet
        m_currentlyPlacing = true;
    }

    /// <summary>
    /// Stop building a tower
    /// </summary>
    public void CancelBuildTower()
    {
        if (m_currentlyPlacing)
        {
            Destroy(m_currentlyPlacingTower.gameObject);
            m_currentlyPlacingSOTower = null;
            m_currentlyPlacingTower = null;
            m_currentlyPlacing = false;
            m_lastBuildPos = null;
        }
    }

    /// <summary>
    /// Move around and confirm placing a tower
    /// </summary>
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

    /// <summary>
    /// Places the tower
    /// </summary>
    /// <param name="_soTower">SO of the tower to place</param>
    private void PlaceTowerDown(SOTower _soTower)
    {
        Vector3? buildPos = InputHelper.GetMouseWorldPosition3D(m_buildableLayers);
        if (buildPos.HasValue)
        {
            if (ValidBuildingSpot(_soTower, buildPos)) // Not really needed but just to make sure (in case it is used elsewere)
            {
                TDGridObjectEmpty placeableGround = m_grid.GetGridObject(buildPos.Value) as TDGridObjectEmpty;
                // Make all tile the tower occupies aware of it
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
                UIManager.Instance?.DisableCancel();
                m_currentlyPlacing = false;
                m_lastBuildPos = null;
            }
        }
    }

    /// <summary>
    /// Is the selected spot valid (valid tiles and not occupied)
    /// </summary>
    /// <param name="_soTower">SO of the tower to place</param>
    /// <param name="_x">X coordinate on the grid</param>
    /// <param name="_z">X coordinate on the grid</param>
    /// <returns>True if allowed to build at given XZ</returns>
    private bool ValidBuildingSpot(SOTower _soTower, int _x, int _z)
    {
        bool retVal = true;
        // Last validate same positions?
        if (m_lastBuildPos != null && m_lastBuildPos.Value.x == _x && m_lastBuildPos.Value.y == _z)
            return true;
        // Not out of map (for multi tile towers)
        if (_x + _soTower.Footprint.x <= maxMapX && _z + _soTower.Footprint.y <= maxMapZ)
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
        // Save for reuse in case of same coordinates on next validate
        if (retVal)
            m_lastBuildPos = new Vector2(_x, _z);

        return retVal;
    }

    /// <summary>
    /// Is the selected spot valid (valid tiles and not occupied)
    /// </summary>
    /// <param name="_soTower">SO of the tower to place</param>
    /// <param name="_buildPos">Building position in world space</param>
    /// <returns>True if allowed to build at given Vector3</returns>
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
    #endregion
}
