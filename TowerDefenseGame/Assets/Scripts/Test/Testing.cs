using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] private Transform m_placeholderPrefab;
    void Start()
    {
        TDGrid<int> grid = new(4, 2, 2f, new Vector3(5, 0), true, m_placeholderPrefab);
        TDGrid<int> grid2 = new(4, 2, 2f, new Vector3(-1, 10), true, m_placeholderPrefab);
    }
}
