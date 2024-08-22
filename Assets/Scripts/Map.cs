using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Hardware;
using UnityEngine;

public class Map : MonoBehaviour
{
    Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();
    
    public int seed = 69;
    public int mapHeight = 8;
    public int mapWidth = 16;
    public int chunkSize = 4;
    public float surfaceDensity = 1.0f;
    public int densityMin = -32;
    public int densityMax = 16;

    private void Start()
    {
        Random.InitState(seed);


        for (int x = 0; x < mapWidth; x += chunkSize)
        {
            for(int y = 0; y < mapWidth; y += chunkSize)
            {
                Chunk chunk = new Chunk(new Vector3Int(x, 0, y), new Vector3Int[chunkSize, mapHeight, chunkSize], densityMin, densityMax);
                chunks.Add(chunk.pos, chunk);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            EditorApplication.isPlaying = false;
        }
    }

    private void OnDrawGizmos()
    {
        foreach (var chunk in chunks)
        {
            chunk.Value.Draw(surfaceDensity);
        }
    }

    
}
