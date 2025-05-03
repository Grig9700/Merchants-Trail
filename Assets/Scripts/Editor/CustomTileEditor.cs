using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

//[CustomEditor(typeof(CustomTile))]
//public class CustomTileEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();
//    }

//    public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
//    {
//        Tile tile = (Tile)target;

//        if (tile.sprite == null)
//            return base.RenderStaticPreview(assetPath, subAssets, width, height);

//        Texture2D newIcon = new Texture2D(width, height);
//        Texture2D spritePreview = AssetPreview.GetAssetPreview(tile.sprite);
//        EditorUtility.CopySerialized(spritePreview, newIcon);
//        EditorUtility.SetDirty(tile);
//        return newIcon;
//    }
//}
