using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tower", menuName = "TD/Tower")]
public class SOTower : ScriptableObject
{
    public string Name = "Tower name";
    public float FireRate = 1f;
    public int Damage = 1;
    public float Range = 1f;
    public int Cost = 10;
    public Vector2 Footprint = Vector2.one; // in grid size
    public Tower TowerPrefab;
}
