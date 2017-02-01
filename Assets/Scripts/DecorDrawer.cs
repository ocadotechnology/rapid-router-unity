using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


public class DecorDrawer : MonoBehaviour
{
    [Inject]
    BoardTranslator translator;

    [Inject]
    Installer.Settings.DecorationTiles tiles;

    public GameObject[] SetupDecorations(LevelDecor[] decorations)
    {
        GameObject[] decorationObjects = new GameObject[decorations.Length];
        for (int currentIndex = 0; currentIndex < decorations.Length; currentIndex++)
        {
            LevelDecor decoration = decorations[currentIndex];
            GameObject tile;
            Dictionary<String, GameObject> nameToTile = new Dictionary<String, GameObject>();
            nameToTile.Add("tree1", tiles.tree1Tile);
            nameToTile.Add("tree2", tiles.tree2Tile);
            nameToTile.Add("bush", tiles.bushTile);
            nameToTile.Add("pond", tiles.pondTile);
            tile = null;
            if (nameToTile.TryGetValue(decoration.decorName, out tile))
            {
                float row = translator.translateRow(decoration.x / 100f, false);
                float column = translator.translateColumn(decoration.y / 100f, false);
                decorationObjects[currentIndex] = Instantiate(tile,
                                    new Vector3(row, column, 0f),
                                    Quaternion.identity) as GameObject;
            } else {
                decorationObjects[currentIndex] = new GameObject();
            }
        }
        return decorationObjects;
    }
}