using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class LevelReader {

    public static Level ReadLevelFromFile(int level) {
        TextAsset levelTextAsset = (TextAsset)Resources.Load("Levels/" + level, typeof(TextAsset));
        string levelText = levelTextAsset.text;
        Level levelObj = JsonConvert.DeserializeObject<Level>(levelText);
        return levelObj;
    }

}