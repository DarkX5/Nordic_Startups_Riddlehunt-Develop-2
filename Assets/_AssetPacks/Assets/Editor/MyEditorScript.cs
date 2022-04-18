using UnityEditor;

namespace Editor
{
    class MyEditorScript
    {
        static void PerformBuild ()
        {
            string[] scenes = { "Assets/Scenes/MyScene.unity" };
            BuildPipeline.BuildPlayer(scenes, "./Builds/MyBuild", BuildTarget.Android, BuildOptions.None);
        }
    }
}