using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class GameTile : Tile
{
    [SerializeField] public bool isPathable;
    //public Sprite m_Preview;

#if UNITY_EDITOR
    // The following is a helper that adds a menu item to create a RoadTile Asset
    [MenuItem("Assets/Create/GameTile")]
    public static void CreateGameTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Game Tile", "New Game Tile", "Asset", "Save Game Tile", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<GameTile>(), path);
    }
#endif

}
