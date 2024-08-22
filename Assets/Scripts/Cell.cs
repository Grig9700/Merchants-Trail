using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class Cell
{
    public Vector3Int pos;

    public Corner[] corners = new Corner[8];
    public Edge[] edges = new Edge[12];

    public Cell(Vector3Int pos, Corner[] corners, Edge[] edges)
    {
        this.pos = pos;
        this.corners = corners;
        this.edges = edges;
    }

    public void Draw(Vector3Int chunkPos)
    {
        Gizmos.DrawSphere(pos + chunkPos, 0.1f);
    }
}