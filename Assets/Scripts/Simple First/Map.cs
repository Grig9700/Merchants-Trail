using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using UnityEngine.WSA;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace SimpleFirst
{
    public class Map : MonoBehaviour
    {
        [SerializeField]
        private uint mapWidth = 7;
        [SerializeField]
        private uint mapDepth = 7;


        [SerializeField]
        private int tileLayer = 7;
        [SerializeField]
        private CustomTile temp;
        private Tilemap tMap;
        private TilemapRenderer tileRenderer;
        private Dictionary<Vector3Int, CustomTileData> map;

        void Start()
        {
            tMap = this.GetComponentInChildren<Tilemap>();
            tileRenderer = this.GetComponentInChildren<TilemapRenderer>();
            map = PaintMap(tMap, temp);
            Neigbours(tMap);
        }

        private Dictionary<Vector3Int, CustomTileData> PaintMap(Tilemap tilemap, CustomTile tile)
        {
            for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapDepth; j++)
                {
                    var position = new Vector3Int(i, j, 0);
                    tilemap.SetTile(position, tile);
                }
            }

            GameObject tileParent = null;

            for (int i = 0, count = transform.childCount; i < count; i++)
            {
                var child = transform.GetChild(i).gameObject;

                if (child.layer != tileLayer)
                    continue;

                tileParent = child;
            }

            Dictionary<Vector3Int, CustomTileData> map = new Dictionary<Vector3Int, CustomTileData>();
            var tiles = tileParent.GetComponentsInChildren<CustomTileData>();

            foreach (var t in tiles)
            {
                Vector3Int cellPosition = tilemap.WorldToCell(t.transform.position);

                map.Add(cellPosition, t);
            }

            return map;
        }

        private void GetNeighbours(Tilemap tilemap, Vector3Int pos)
        {
            CustomTileData tile;

            map.TryGetValue(pos, out tile);

            if (tile == null)
                return;

            var positions = GetNeighbourPositions.Neighbors(pos);

            List<CustomTileData> neighbours = new List<CustomTileData>();
            foreach (var position in positions)
            {
                CustomTileData neighbour;

                map.TryGetValue(position, out neighbour);

                if (neighbour == null)
                    continue;

                neighbours.Add(neighbour);
            }

            tile.SetNeighbours(neighbours);
            tile.SetPos(pos);
        }

        private void Neigbours(Tilemap map)
        {
            for (int i = map.cellBounds.x; i < map.cellBounds.max.x + 1; i++)
            {
                for (int j = map.cellBounds.y; j < map.cellBounds.max.y + 1; j++)
                {
                    var pos = new Vector3Int(i, j, 0);
                    var tile = (CustomTile)map.GetTile(pos);
                    if (tile == null)
                        continue;

                    GetNeighbours(map, pos);

                }
            }
        }
    }
}
