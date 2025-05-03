using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AStar
{
    private static float Heuristic(CustomTileData a, CustomTileData b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }

    public static List<CustomTileData> FindPath(CustomTileData start, CustomTileData goal)
    {
        var open = new List<CustomTileData>();
        var closed = new HashSet<Vector3>();

        start.G = 0;
        start.H = Heuristic(start, goal);
        open.Add(start);

        while (open.Count > 0)
        {
            var current = open[0];
            open.RemoveAt(0);

            // If goal is reached, reconstruct the path
            if (current.pos == goal.pos)
            {
                var path = new List<CustomTileData>();
                var temp = current;
                while (temp.pos != start.pos)
                {
                    path.Add(temp);
                    temp = temp.Parent;
                }
                path.Add(start);
                path.Reverse();
                return path;
            }

            closed.Add(current.pos);

            // Explore neighbors
            foreach (var neighbour in current.neighbours)
            {
                if (closed.Contains(neighbour.pos))
                    continue;

                float tentativeG = current.G + 1; // 1 for each move

                if (open.Contains(neighbour) && tentativeG >= neighbour.G)
                    continue;

                neighbour.G = tentativeG;
                neighbour.H = Heuristic(neighbour, goal);
                neighbour.Parent = current;

                // Add or update the neighbor in the open list
                if (open.Contains(neighbour))
                {
                    open.Remove(neighbour);
                }
                open.Add(neighbour);
            }

            open.Sort((x, y) => x.F.CompareTo(y.F));
        }

        return new List<CustomTileData>(); // No path found
    }
}
