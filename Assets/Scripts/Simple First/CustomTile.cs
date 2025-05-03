using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Tilemaps;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using System.Linq;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "TerrainTile", menuName = "Resources/tiles")]
public class CustomTile : TileBase
{
    public GameObject tilePrefab;
    public Sprite m_DefaultSprite;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.gameObject = tilePrefab;
        tileData.sprite = m_DefaultSprite;
        tileData.transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(-1, 1, 1)); // example: x-mirror the sprite
        tileData.flags = TileFlags.LockTransform;
    }

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        TileBase tileBase = tilemap.GetTile(position);
        Matrix4x4 matrix = tilemap.GetTransformMatrix(position);
        return base.StartUp(position, tilemap, go);
    }
}
