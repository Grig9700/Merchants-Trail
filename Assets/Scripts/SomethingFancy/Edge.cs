using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Edge
{
    public Corner p1;
    public Corner p2;
    public Vector3Int interpolatedPoint;

    public Edge(Corner p1, Corner p2)
    {
        this.p1 = p1;
        this.p2 = p2;
    }
    public void Draw(Vector3Int chunkPos)
    {
        Gizmos.DrawLine(p1.pos + chunkPos, p2.pos + chunkPos);
    }
}
