using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

public class Chunk
{
    public Vector3Int pos;
    public Cell[,,] cells;
    public Corner[,,] corners;

    [HideInInspector]
    public Mesh mesh;

    public Chunk(Vector3Int pos, Vector3Int[,,] chunkSize, int densityMin, int densityMax)
    {
        this.pos = pos;
        corners = new Corner[chunkSize.GetLength(0), chunkSize.GetLength(1), chunkSize.GetLength(2)];

        for (int z = 0; z < chunkSize.GetLength(2); z++)
        {
            for (int y = 0; y < chunkSize.GetLength(1); y++)
            {
                for(int x = 0; x < chunkSize.GetLength(0); x++)
                {
                    corners[x, y, z] = new Corner(new Vector3Int(x, y, z), Random.Range(densityMin, densityMax));
                }
            }
        }

        cells = new Cell[chunkSize.GetLength(0) - 1, chunkSize.GetLength(1) - 1, chunkSize.GetLength(2) - 1];
        for (int z = 0; z < corners.GetLength(2) - 1; z++)
        {
            for (int y = 0; y < corners.GetLength(1) - 1; y++)
            {
                for (int x = 0; x < corners.GetLength(0) - 1; x++)
                {
                    Corner[] cellCorners = new Corner[8];
                    cellCorners[0] = corners[x, y, z];
                    cellCorners[1] = corners[x, y, z + 1];
                    cellCorners[2] = corners[x, y + 1, z];
                    cellCorners[3] = corners[x, y + 1, z + 1];
                    cellCorners[4] = corners[x + 1, y, z];
                    cellCorners[5] = corners[x + 1, y, z + 1];
                    cellCorners[6] = corners[x + 1, y + 1, z];
                    cellCorners[7] = corners[x + 1, y + 1, z + 1];

                    cells[x, y, z] = new Cell(new Vector3Int(x, y, z), cellCorners, new Edge[12]);
                }
            }
        }
    }

    public void Draw(float surfaceDensity)
    {
        //foreach (var cell in cells)
        //{
        //    cell.Draw(pos);
        //}

        foreach (var corner in corners)
        {
            if (corner.density < surfaceDensity)
                continue;

            corner.Draw(pos, 16, -32);
        }



        foreach (Cell cell in cells)
        {
            int cellIndex = 0;
            for (int i = 0; i < cell.corners.Length; i++)
            {
                if (cell.corners[i].density >= surfaceDensity)
                    continue;

                cellIndex |= 1 << i;
            }

            var triangulation = TriangulationTables.GetTriangulation(cellIndex).Where(x => x != -1).ToArray();
            var triangleCount = triangulation.Length / 3;

            var vertices = new Vector3[triangleCount * 3];
            var meshTriangles = new int[triangleCount * 3];

            for (int i = 0; i < triangleCount; i++)
            {
                vertices[i] = new Vector3(triangulation[i * 3], triangulation[i * 3 + 1], triangulation[i * 3 + 2]);
            }
            
            mesh.vertices = vertices;
            mesh.triangles = meshTriangles;

            mesh.RecalculateNormals();
        }
    }
}
