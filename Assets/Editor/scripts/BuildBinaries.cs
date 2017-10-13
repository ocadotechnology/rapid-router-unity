using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildBinaries : MonoBehaviour {

    [MenuItem("Build/iPhone")]
    public static void BuildForIPhone()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] {"Assets/Scenes/GameScene.unity"};
        buildPlayerOptions.locationPathName = "iPhoneBuild";
        buildPlayerOptions.target = BuildTarget.iPhone;
        buildPlayerOptions.options = BuildOptions.None;
        
        Console.WriteLine ("locationPathName: " + buildPlayerOptions.locationPathName);
        Console.WriteLine ("target: " + buildPlayerOptions.target);
        Console.WriteLine ("targetGroup: " + buildPlayerOptions.targetGroup);
        Console.WriteLine ("scenes: " + buildPlayerOptions.scenes);
        Console.WriteLine ("assetBundleManifestPath: " + buildPlayerOptions.assetBundleManifestPath);
        
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }
}
