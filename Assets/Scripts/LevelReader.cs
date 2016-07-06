using System;
using System.Collections.Generic;
// using FullSerializer;
using Newtonsoft.Json;
using UnityEngine;

// public class LevelConverter: fsDirectConverter<Level> {

//     // private static readonly fsSerializer Serializer = new fsSerializer();

//     public override object CreateInstance(fsData data, Type storageType) {
//         return new Level();
//     }

//     protected override fsResult DoSerialize(Level model, Dictionary<string, fsData> serialized) {
//         return fsResult.Success;
//     }

//     protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref Level model) {
//         var result = fsResult.Success;
        
//         DeserializeStringData(data, "path", ref result, ref model.path);
//         if (result.Failed) return result;

//         DeserializeStringData(data, "origin", ref result, ref model.origin);
//         if (result.Failed) return result;

//         DeserializeStringData(data, "destinations", ref result, ref model.destinations);
//         if (result.Failed) return result;

//         return result;
//     }

//     private void DeserializeStringData<T>(Dictionary<string, fsData> data, string propertyName, ref fsResult result, ref T model) where T : class {
//         fsData pathData;
//         if ((result += CheckKey(data, "path", out pathData)).Failed) return;
//         if ((result += CheckType(pathData, fsDataType.String)).Failed) return;
//         object deserialisedPath = null;
//         Serializer.TryDeserialize(pathData, typeof(T), ref deserialisedPath).AssertSuccessWithoutWarnings();
//         model = deserialisedPath as T; 
//     }
// }

public class LevelReader {

    public static Level ReadLevelFromFile(int level) {
        TextAsset levelTextAsset = (TextAsset)Resources.Load("Levels/" + level, typeof(TextAsset));
        string levelText = levelTextAsset.text;
        Level levelObj = JsonConvert.DeserializeObject<Level>(levelText);
        return levelObj;
    }

}