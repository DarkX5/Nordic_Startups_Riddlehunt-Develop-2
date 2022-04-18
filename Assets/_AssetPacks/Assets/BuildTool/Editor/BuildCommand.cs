using System;
using UnityEditor;

namespace BuildTool.Editor
{
    public class BuildCommand{

        public static readonly string[] scenes = { "Assets/Scenes/AppScenes/Startup.unity" };

        public static void BuildiOS()
        {
            Console.Write("build for iOS");

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = scenes;
            buildPlayerOptions.locationPathName = "./Builds/RiddlehuntAlpha.ipa";
            buildPlayerOptions.target = BuildTarget.iOS;
            buildPlayerOptions.options = BuildOptions.None;
            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }

        public static void BuildAndroid()
        {
            Console.Write("build for Android");
        
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = scenes;
            buildPlayerOptions.locationPathName = "./Builds/RiddlehuntAlpha.apk";
            buildPlayerOptions.target = BuildTarget.Android;
            buildPlayerOptions.options = BuildOptions.None;
            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }
    }
}