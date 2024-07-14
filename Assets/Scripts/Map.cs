using System.Collections;
using System.Collections.Generic;
using UnityEditor.Hardware;
using UnityEngine;

public class Map : MonoBehaviour
{
    public TerrainClutter clutter;
    public TerrainMaterials material;
    public bool IsSeeded;
    public int seed = 39;

    [Header("Terrain")]
    public float scale = .1f;
    public int size = 100;
    public int height = 10;
    public float elevationScale = 0.2f;
    public float waterLevel = .2f;
    public float waterHeightDisplacement = -.1f;
    public float hillLevel = .5f;
    //public float hillHeightDisplacement = .4f;
    public float mountainLevel = .8f;
    //public float mountainHeightDisplacement = .8f;

    [Header("Clutter")]
    public float treeNoiseScale = .05f;
    public float treeDensity = .5f;
    public float riverNoiseScale = .06f;
    public int rivers = 5;

    Cell[,] grid;
    float[] elevation;

    Quaternion qRight = Quaternion.AngleAxis(90, Vector3.up);
    Quaternion qLeft = Quaternion.AngleAxis(-90, Vector3.up);
    Quaternion qNone = Quaternion.AngleAxis(0, Vector3.up);
    Quaternion qBack = Quaternion.AngleAxis(180, Vector3.up);

    void Start()
    {
        if (IsSeeded)
            Random.InitState(seed);

        InitializeGrid();

        //int iterator = 0;
        //while(SmoothGrid() && iterator < height * 10)
        //{
        //    iterator++;
        //}

        //GenerateRivers(grid);
        DrawTerrainMesh();
        DrawEdgeMesh();
        //GenerateClutter();
    }

    void InitializeGrid()
    {
        float[,] noiseMap = new float[size, size];
        (float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noiseValue = Mathf.PerlinNoise(x * scale + xOffset, y * scale + yOffset);
                noiseMap[x, y] = noiseValue;
            }
        }

        //float[,] falloffMap = new float[size, size];
        //for (int y = 0; y < size; y++)
        //{
        //    for (int x = 0; x < size; x++)
        //    {
        //        float xv = x / (float)size * 2 - 1;
        //        float yv = y / (float)size * 2 - 1;
        //        float v = Mathf.Max(Mathf.Abs(xv), Mathf.Abs(yv));
        //        falloffMap[x, y] = Mathf.Pow(v, 3f) / (Mathf.Pow(v, 3f) + Mathf.Pow(2.2f - 2.2f * v, 3f));
        //    }
        //}

        elevation = new float[height];
        for (int i = 0; i < height; i++)
        {
            elevation[i] = 1f / height * i;
        }

        grid = new Cell[size, size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noiseValue = noiseMap[x, y];

                int elevation = GetElevation(noiseValue - waterLevel);

                //noiseValue -= falloffMap[x, y];
                bool isWater = noiseValue < waterLevel;
                bool isHill = hillLevel < noiseValue && noiseValue < mountainLevel;
                bool isMountain = noiseValue > mountainLevel;

                if (isWater)
                    elevation = 0;

                Cell cell = new Cell(new Vector2Int(x, y), isWater, isHill, isMountain, elevation);
                grid[x, y] = cell;
            }
        }
    }

    int GetElevation(float noiseValue)
    {
        for(int i = 0; i < height - 1; i++) 
        {
            if (elevation[i] < noiseValue && noiseValue < elevation[i +1])
                return i;
        }
        return height - 1;
    }

    bool SmoothGrid()
    {
        bool changed = false;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if (grid[x, y].elevation == 0)
                    continue;

                int myHeight = 0;

                if (x > 0 && grid[x - 1, y].elevation >= grid[x, y].elevation)
                    myHeight++;

                if (y > 0 && grid[x, y - 1].elevation >= grid[x, y].elevation)
                    myHeight++;

                if (x < size - 1 && grid[x + 1, y].elevation >= grid[x, y].elevation)
                    myHeight++;

                if (y < size - 1 && grid[x, y + 1].elevation >= grid[x, y].elevation)
                    myHeight++;

                if (y > 0 && grid[x, y - 1].elevation < grid[x, y].elevation && y < size - 1 && grid[x, y + 1].elevation < grid[x, y].elevation
                    || x > 0 && grid[x - 1, y].elevation < grid[x, y].elevation && x < size - 1 && grid[x + 1, y].elevation < grid[x, y].elevation)
                    myHeight -= 5;

                if (myHeight > 1)
                    continue;

                if (grid[x, y].isHill && elevation[grid[x, y].elevation - 1] < hillLevel)
                {
                    grid[x, y] = new Cell(new Vector2Int(x, y), false, false, false, grid[x, y].elevation -1);
                    changed = true;
                    continue;
                }

                if (grid[x, y].isMountain && elevation[grid[x, y].elevation - 1] < mountainLevel)
                {
                    grid[x, y] = new Cell(new Vector2Int(x, y), false, true, false, grid[x, y].elevation - 1);
                    changed = true;
                    continue;
                }

                grid[x, y] = new Cell(new Vector2Int(x, y), false, true, false, grid[x, y].elevation - 1);
                changed = true;
            }
        }

        return changed;
    }

    //void GenerateRivers(Cell[,] grid)
    //{
    //    float[,] noiseMap = new float[size, size];
    //    (float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
    //    for (int y = 0; y < size; y++)
    //    {
    //        for (int x = 0; x < size; x++)
    //        {
    //            float noiseValue = Mathf.PerlinNoise(x * riverNoiseScale + xOffset, y * riverNoiseScale + yOffset);
    //            noiseMap[x, y] = noiseValue;
    //        }
    //    }

    //    GridGraph gg = AstarData.active.graphs[0] as GridGraph;
    //    gg.center = new Vector3(size / 2f - .5f, 0, size / 2f - .5f);
    //    gg.SetDimensions(size, size, 1);
    //    AstarData.active.Scan(gg);
    //    AstarData.active.AddWorkItem(new AstarWorkItem(ctx =>
    //    {
    //        for (int y = 0; y < size; y++)
    //        {
    //            for (int x = 0; x < size; x++)
    //            {
    //                GraphNode node = gg.GetNode(x, y);
    //                node.Walkable = noiseMap[x, y] > .4f;
    //            }
    //        }
    //    }));
    //    AstarData.active.FlushGraphUpdates();

    //    int k = 0;
    //    for (int i = 0; i < rivers; i++)
    //    {
    //        GraphNode start = gg.nodes[Random.Range(16, size - 16)];
    //        GraphNode end = gg.nodes[Random.Range(size * (size - 1) + 16, size * size - 16)];
    //        ABPath path = ABPath.Construct((Vector3)start.position, (Vector3)end.position, (Path result) =>
    //        {
    //            for (int j = 0; j < result.path.Count; j++)
    //            {
    //                GraphNode node = result.path[j];
    //                int x = Mathf.RoundToInt(((Vector3)node.position).x);
    //                int y = Mathf.RoundToInt(((Vector3)node.position).z);
    //                grid[x, y].isWater = true;
    //            }
    //            k++;
    //        });
    //        AstarPath.StartPath(path);
    //        AstarPath.BlockUntilCalculated(path);
    //    }
    //}

    void DrawTerrainMesh()
    {
        List<Cell> grass = new List<Cell>();
        List<Cell> hill = new List<Cell>();
        List<Cell> mountain = new List<Cell>();
        List<Cell> water = new List<Cell>();

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Cell cell = grid[x, y];

                if (cell.isWater)
                {
                    water.Add(cell);
                    continue;
                }

                if (cell.isHill)
                {
                    hill.Add(cell);
                    continue;
                }

                if (cell.isMountain)
                {
                    mountain.Add(cell);
                    continue;
                }

                grass.Add(cell);
            }
        }

        SelectSubTerrain("Grass", grass, material.grassMaterial);
        SelectSubTerrain("Hill", hill, material.hillMaterial);
        SelectSubTerrain("Mountain", mountain, material.mountainMaterial);
        SelectSubTerrain("Water", water, material.waterMaterial, waterHeightDisplacement);
    }

    void SelectSubTerrain(string meshName, List<Cell> cells, Material material, float displacement = 0)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        foreach ( var cell in cells )
        {
            PlaceSurfaceVertice(cell.pos.x, cell.pos.y, cell.elevation * elevationScale + displacement, vertices, triangles, uvs);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        MakeSubMeshObject(meshName, mesh, material);
    }

    void PlaceSurfaceVertice(int x, int y, float z, List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
    {
        Vector3 a = new Vector3(x - .5f, z, y + .5f);
        Vector3 b = new Vector3(x + .5f, z, y + .5f);
        Vector3 c = new Vector3(x - .5f, z, y - .5f);
        Vector3 d = new Vector3(x + .5f, z, y - .5f);

        Vector2 uvA = new Vector2(x / (float)size, y / (float)size);
        Vector2 uvB = new Vector2((x + 1) / (float)size, y / (float)size);
        Vector2 uvC = new Vector2(x / (float)size, (y + 1) / (float)size);
        Vector2 uvD = new Vector2((x + 1) / (float)size, (y + 1) / (float)size);

        bool flippedVertices = false;

        if (grid[x, y].elevation > 0)
        {
            float mod = 0;
            if (x > 0 && grid[x - 1, y].elevation < grid[x, y].elevation)
            {
                mod = z - elevation[grid[x, y].elevation] + elevation[grid[x - 1, y].elevation];
                a.y = mod;
                c.y = mod;
            }

            if (y > 0 && grid[x, y - 1].elevation < grid[x, y].elevation)
            {
                mod = z - elevation[grid[x, y].elevation] + elevation[grid[x, y - 1].elevation];
                c.y = mod;
                d.y = mod;
            }

            if (x < size - 1 && grid[x + 1, y].elevation < grid[x, y].elevation)
            {
                mod = z - elevation[grid[x, y].elevation] + elevation[grid[x + 1, y].elevation];
                b.y = mod;
                d.y = mod;
            }

            if (y < size - 1 && grid[x, y + 1].elevation < grid[x, y].elevation)
            {
                mod = z - elevation[grid[x, y].elevation] + elevation[grid[x, y + 1].elevation];
                a.y = mod;
                b.y = mod;
            }

            if (x > 0 && y < size - 1 && grid[x - 1, y + 1].elevation < grid[x, y].elevation
                && grid[x, y + 1].elevation == grid[x, y].elevation
                && grid[x - 1, y].elevation == grid[x, y].elevation)
            {
                a.y = z - elevation[grid[x, y].elevation] + elevation[grid[x - 1, y + 1].elevation];

                flippedVertices = true;
            }

            if (x > 0 && y > 0 && grid[x - 1, y - 1].elevation < grid[x, y].elevation)
            {
                c.y = z - elevation[grid[x, y].elevation] + elevation[grid[x - 1, y - 1].elevation];
            }

            if (x < size - 1 && y > 0 && grid[x + 1, y - 1].elevation < grid[x, y].elevation
                && grid[x, y - 1].elevation == grid[x, y].elevation
                && grid[x + 1, y].elevation == grid[x, y].elevation)
            {
                d.y = z - elevation[grid[x, y].elevation] + elevation[grid[x + 1, y - 1].elevation];

                flippedVertices = true;
            }

            if (x < size - 1 && y < size - 1 && grid[x + 1, y + 1].elevation < grid[x, y].elevation)
            {
                b.y = z - elevation[grid[x, y].elevation] + elevation[grid[x + 1, y + 1].elevation];
            }

            if (x > 0 && y < size - 1 && grid[x, y + 1].elevation < grid[x, y].elevation && grid[x - 1, y].elevation < grid[x, y].elevation
                || x < size - 1 && y > 0 && grid[x, y - 1].elevation < grid[x, y].elevation && grid[x + 1, y].elevation < grid[x, y].elevation)
            {
                flippedVertices = true;
            }
        }

        Vector3[] v = new Vector3[6];
        Vector2[] uv = new Vector2[6];
        if (flippedVertices)
        {
            v = new Vector3[] { a, b, d, a, d, c };
            uv = new Vector2[] { uvA, uvB, uvA, uvB, uvD, uvC };
            for (int k = 0; k < 6; k++)
            {
                vertices.Add(v[k]);
                triangles.Add(triangles.Count);
                uvs.Add(uv[k]);
            }
            return;
        }


        v = new Vector3[] { a, b, c, b, d, c };
        uv = new Vector2[] { uvA, uvB, uvC, uvB, uvD, uvC };
        for (int k = 0; k < 6; k++)
        {
            vertices.Add(v[k]);
            triangles.Add(triangles.Count);
            uvs.Add(uv[k]);
        }
    }

    void DrawEdgeMesh()
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                CheckIfEdgeIsConnectedToWater(x, y, vertices, triangles);
            }
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        MakeSubMeshObject("Edge", mesh, material.dirtMaterial);
    }

    void MakeSubMeshObject(string name, Mesh mesh, Material material)
    {
        GameObject edgeObj = new GameObject(name);
        edgeObj.transform.SetParent(transform);

        MeshFilter meshFilter = edgeObj.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = edgeObj.AddComponent<MeshRenderer>();
        meshRenderer.material = material;
    }

    void CheckIfEdgeIsConnectedToWater(int x, int y, List<Vector3> vertices, List<int> triangles)
    {
        Cell cell = grid[x, y];
        if (cell.isWater)
            return;

        if (x > 0 && grid[x - 1, y].isWater)
        {
            RotateEdge(x, y, qNone, vertices, triangles);
        }

        if (x < size - 1 && grid[x + 1, y].isWater)
        {
            RotateEdge(x, y, qBack, vertices, triangles);
        }

        if (y > 0 && grid[x, y - 1].isWater)
        {
            RotateEdge(x, y, qLeft, vertices, triangles);
        }

        if (y < size - 1 && grid[x, y + 1].isWater)
        {
            RotateEdge(x, y, qRight, vertices, triangles);
        }
    }

    void RotateEdge(int x, int y, Quaternion rotation, List<Vector3> vertices, List<int> triangles)
    {
        Vector3 a = rotation * new Vector3(-.5f, 0, .5f) + new Vector3(x, 0, y);
        Vector3 b = rotation * new Vector3(-.5f, 0, -.5f) + new Vector3(x, 0, y);
        Vector3 c = rotation * new Vector3(-.5f, -1, .5f) + new Vector3(x, 0, y);
        Vector3 d = rotation * new Vector3(-.5f, -1, -.5f) + new Vector3(x, 0, y);

        Vector3[] v = new Vector3[] { a, b, c, b, d, c };
        for (int i = 0; i < 6; i++)
        {
            vertices.Add(v[i]);
            triangles.Add(triangles.Count);
        }
    }

    void GenerateClutter()
    {
        float[,] noiseMap = new float[size, size];
        (float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noiseValue = Mathf.PerlinNoise(x * treeNoiseScale + xOffset, y * treeNoiseScale + yOffset);
                noiseMap[x, y] = noiseValue;
            }
        }

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Cell cell = grid[x, y];
                if (cell.isWater)
                    continue;

                float v = Random.Range(0f, treeDensity);
                if (noiseMap[x, y] > v)
                    continue;

                GameObject prefab = null;

                if (cell.isMountain)
                    prefab = clutter.GetMountainClutter();
                else if (cell.isHill)
                    prefab = clutter.GetHillClutter();
                else 
                    prefab = clutter.GetGrassClutter();


                GameObject obj = Instantiate(prefab, transform);
                obj.transform.position = new Vector3(x, 0, y);
                obj.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
                obj.transform.localScale = Vector3.one * Random.Range(.8f, 1.2f);
            }
        }
    }

    void PlaceClutter()
    {

    }
}
