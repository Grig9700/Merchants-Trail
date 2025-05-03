using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class CustomTileData : MonoBehaviour, IMouseInterractable
{
    #region Pathing
    public float travelWeight = 1;

    [HideInInspector]
    public float G, H = 0f;
    [HideInInspector]
    public float F => G + H;
    [HideInInspector]
    public CustomTileData Parent;
    #endregion

    public Vector3Int pos { private set; get; }
    public List<CustomTileData> neighbours { get; private set; }

    public void SetPos(Vector3Int pos)
    {
        this.pos = pos;
    }
    public void SetNeighbours(IEnumerable<CustomTileData> neighbours)
    {
        this.neighbours = neighbours.ToList();
    }

    public IEnumerable<CustomTileData> Interract(CustomTileData data = null)
    {
        Debug.Log($"Hello, you clicked me {name} at {pos} in the neighbourhood of {neighbours.Count}");

        if (data == null)
            return null;

        var path = AStar.FindPath(data, this);

        foreach (var tile in path)
        {
            var outline = tile.gameObject.GetOrAddComponent<Outline>();
        }

        return path;
    }
}
