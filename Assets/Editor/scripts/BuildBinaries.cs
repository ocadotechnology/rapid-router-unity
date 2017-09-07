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
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }
}
