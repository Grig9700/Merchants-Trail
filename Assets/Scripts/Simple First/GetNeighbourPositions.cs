using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetNeighbourPositions
{
    static Vector3Int
    LEFT = new Vector3Int(-1, 0, 0),
    RIGHT = new Vector3Int(1, 0, 0),
    DOWN = new Vector3Int(0, -1, 0),
    DOWNLEFT = new Vector3Int(-1, -1, 0),
    DOWNRIGHT = new Vector3Int(1, -1, 0),
    UP = new Vector3Int(0, 1, 0),
    UPLEFT = new Vector3Int(-1, 1, 0),
    UPRIGHT = new Vector3Int(1, 1, 0);

    static Vector3Int[] directions_when_y_is_even =
          { LEFT, RIGHT, DOWN, DOWNLEFT, UP, UPLEFT };
    static Vector3Int[] directions_when_y_is_odd =
          { LEFT, RIGHT, DOWN, DOWNRIGHT, UP, UPRIGHT };

    public static IEnumerable<Vector3Int> Neighbors(Vector3Int node)
    {
        Vector3Int[] directions = (node.y % 2) == 0 ?
             directions_when_y_is_even :
             directions_when_y_is_odd;
        foreach (var direction in directions)
        {
            Vector3Int neighborPos = node + direction;
            yield return neighborPos;
        }
    }
}
