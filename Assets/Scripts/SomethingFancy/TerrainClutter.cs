using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class TerrainClutter : ScriptableObject
{
    [SerializeField]
    private ClutterEntry[] grassClutter;
    [SerializeField]
    private ClutterEntry[] hillClutter;
    [SerializeField]
    private ClutterEntry[] mountainClutter;
    [SerializeField]
    private ClutterEntry[] fishClutter;
    [SerializeField]
    private ClutterEntry[] boatsClutter;


    public GameObject GetGrassClutter()
    {
        return GetClutter(grassClutter);
    }

    public GameObject GetHillClutter()
    {
        return GetClutter(hillClutter);
    }

    public GameObject GetMountainClutter()
    {
        return GetClutter(mountainClutter);
    }

    public GameObject GetFishClutter() 
    { 
        return GetClutter(fishClutter);
    }

    public GameObject GetBoatClutter()
    {
        return GetClutter(boatsClutter);
    }

    private GameObject GetClutter(ClutterEntry[] clutter)
    {
        var selected = UnityEngine.Random.Range(0, clutter.Sum(x => x.chance));

        var count = 0f;

        GameObject returntarget = null;

        for (int i = 0; i < clutter.Length; i++)
        {
            if (selected <= count)
            {
                break;
            }
            count += clutter[i].chance;
            returntarget = clutter[i].prefab;
        }
        return returntarget;
    }
}

[Serializable]
public class ClutterEntry
{
    public float chance;
    public GameObject prefab;
}