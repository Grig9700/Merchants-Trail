using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Corner
{
    public Vector3Int pos;
    public int density;

    public Corner(Vector3Int pos, int density)
    {
        this.pos = pos;
        this.density = density;
    }

    public void Draw(Vector3Int chunkPos, int maxDensity, int minDensity)
    {
        Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(maxDensity, minDensity, density));
        //Debug.Log($"inversed lerp : {Mathf.InverseLerp(maxDensity, minDensity, density)} ::: density : {density}");

        Gizmos.DrawSphere(pos + chunkPos, 0.1f);
    }
}
