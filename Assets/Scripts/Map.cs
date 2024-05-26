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
    public float waterLevel = .4f;
    public float waterHeightDisplacement = -.1f;
    public float hillLevel = .6f;
    public float hillHeightDisplacement = .4f;
    public float mountainLevel = .8f;
    public float mountainHeightDisplacement = .8f;

    [Header("Clutter")]
    public float treeNoiseScale = .05f;
    public float treeDensity = .5f;
    public float riverNoiseScale = .06f;
    public int rivers = 5;
    public int size = 100;

    Cell[,] grid;

    Quaternion qRight = Quaternion.AngleAxis(90, Vector3.up);
    Quaternion qLeft = Quaternion.AngleAxis(-90, Vector3.up);
    Quaternion qNone = Quaternion.AngleAxis(0, Vector3.up);
    Quaternion qBack = Quaternion.AngleAxis(180, Vector3.up);

    void Start()
    {
        if (IsSeeded)
            Random.InitState(seed);

        InitializeGrid();
        SmoothGrid();

        //GenerateRivers(grid);
        DrawTerrainMesh();
        DrawEdgeMesh();
        GenerateClutter();
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

        float[,] falloffMap = new float[size, size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float xv = x / (float)size * 2 - 1;
                float yv = y / (float)size * 2 - 1;
                float v = Mathf.Max(Mathf.Abs(xv), Mathf.Abs(yv));
                falloffMap[x, y] = Mathf.Pow(v, 3f) / (Mathf.Pow(v, 3f) + Mathf.Pow(2.2f - 2.2f * v, 3f));
            }
        }

        grid = new Cell[size, size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noiseValue = noiseMap[x, y];
                noiseValue -= falloffMap[x, y];
                bool isWater = noiseValue < waterLevel;
                bool isHill = hillLevel < noiseValue && noiseValue < mountainLevel;
                bool isMountain = noiseValue > mountainLevel;

                float height = isHill ? hillHeightDisplacement : isMountain ? mountainHeightDisplacement : 0;

                Cell cell = new Cell(new Vector2Int(x, y), isWater, isHill, isMountain, height);
                grid[x, y] = cell;
            }
        }
    }

    void SmoothGrid()
    {
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if (grid[x, y].height == 0)
                    continue;

                int myHeight = 0;

                if (x > 0 && grid[x - 1, y].height >= grid[x, y].height)
                    myHeight++;

                if (y > 0 && grid[x, y - 1].height >= grid[x, y].height)
                    myHeight++;

                if (x < size - 1 && grid[x + 1, y].height >= grid[x, y].height)
                    myHeight++;

                if (y < size - 1 && grid[x, y + 1].height >= grid[x, y].height)
                    myHeight++;

                if (y > 0 && grid[x, y - 1].height < grid[x, y].height && y < size - 1 && grid[x, y + 1].height < grid[x, y].height
                    || x > 0 && grid[x - 1, y].height < grid[x, y].height && x < size - 1 && grid[x + 1, y].height < grid[x, y].height)
                    myHeight -= 5;

                if (myHeight > 1)
                    continue;

                if (grid[x, y].isHill)
                {
                    grid[x, y] = new Cell(new Vector2Int(x, y), false, false, false, 0);
                    continue;
                }

                if (grid[x, y].isMountain)
                {
                    grid[x, y] = new Cell(new Vector2Int(x, y), false, true, false, hillHeightDisplacement);
                    continue;
                }
            }
        }
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

        SelectSubTerrain("Grass", grass, 0, material.grassMaterial);
        SelectSubTerrain("Hill", hill, hillHeightDisplacement, material.hillMaterial);
        SelectSubTerrain("Mountain", mountain, mountainHeightDisplacement, material.mountainMaterial);
        SelectSubTerrain("Water", water, waterHeightDisplacement, material.waterMaterial);
    }

    void SelectSubTerrain(string meshName, List<Cell> cells, float displacement, Material material)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        foreach ( var cell in cells )
        {
            PlaceSurfaceVertice(cell.pos.x, cell.pos.y, displacement, vertices, triangles, uvs);
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

        if (grid[x, y].height > 0)
        {
            if (x > 0 && grid[x - 1, y].height < grid[x, y].height)
            {
                a.y = z - grid[x, y].height + grid[x - 1, y].height;
                c.y = z - grid[x, y].height + grid[x - 1, y].height;
            }

            if (y > 0 && grid[x, y - 1].height < grid[x, y].height)
            {
                c.y = z - grid[x, y].height + grid[x, y - 1].height;
                d.y = z - grid[x, y].height + grid[x, y - 1].height;
            }

            if (x < size - 1 && grid[x + 1, y].height < grid[x, y].height)
            {
                b.y = z - grid[x, y].height + grid[x + 1, y].height;
                d.y = z - grid[x, y].height + grid[x + 1, y].height;
            }

            if (y < size - 1 && grid[x, y + 1].height < grid[x, y].height)
            {
                a.y = z - grid[x, y].height + grid[x, y + 1].height;
                b.y = z - grid[x, y].height + grid[x, y + 1].height;
            }

            if (x > 0 && y < size - 1 && grid[x - 1, y + 1].height < grid[x, y].height)
            {
                a.y = z - grid[x, y].height + grid[x - 1, y + 1].height;
            }

            if (x > 0 && y > 0 && grid[x - 1, y - 1].height < grid[x, y].height)
            {
                c.y = z - grid[x, y].height + grid[x - 1, y - 1].height;
            }

            if (x < size - 1 && y > 0 && grid[x + 1, y - 1].height < grid[x, y].height)
            {
                d.y = z - grid[x, y].height + grid[x + 1, y - 1].height;
            }

            if (x < size - 1 && y < size - 1 && grid[x + 1, y + 1].height < grid[x, y].height)
            {
                b.y = z - grid[x, y].height + grid[x + 1, y + 1].height;
            }

            if (grid[x, y + 1].height < grid[x, y].height && grid[x - 1, y].height < grid[x, y].height
                || grid[x, y - 1].height < grid[x, y].height && grid[x + 1, y].height < grid[x, y].height)
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
                if (cell.isWater || cell.isMountain)
                    continue;

                float v = Random.Range(0f, treeDensity);
                if (noiseMap[x, y] > v)
                    continue;

                //GameObject prefab = clutter.treePrefabs[Random.Range(0, clutter.treePrefabs.Length)];
                //GameObject obj = Instantiate(prefab, transform);
                //obj.transform.position = new Vector3(x, 0, y);
                //obj.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
                //obj.transform.localScale = Vector3.one * Random.Range(.8f, 1.2f);
            }
        }
    }

    void PlaceClutter()
    {

    }
}
