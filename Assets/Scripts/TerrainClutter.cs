using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TerrainClutter : ScriptableObject
{
    public ClutterEntry[] grassClutter;
    public ClutterEntry[] hillClutter;
    public ClutterEntry[] mountainClutter;
    public ClutterEntry[] fishPrefabs;
    public ClutterEntry[] boatsPrefabs;
}

public class ClutterEntry
{
    public float percentage;
    public GameObject prefab;
}