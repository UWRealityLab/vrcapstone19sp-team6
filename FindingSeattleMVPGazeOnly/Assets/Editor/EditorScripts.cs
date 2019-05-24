using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

class EditorScripts : EditorWindow
{

    [MenuItem("Play/Execute starting scene _%h")]
    public static void RunMainScene()
    {
        string currentSceneName = EditorApplication.currentScene;
        File.WriteAllText(".lastScene", currentSceneName);
        EditorApplication.OpenScene("Assets/Scenes/FS_INITIALIZE.unity");
        EditorApplication.isPlaying = true;
    }

    [MenuItem("Play/Reload editing scene _%g")]
    public static void ReturnToLastScene()
    {
        string lastScene = File.ReadAllText(".lastScene");
        EditorApplication.OpenScene(lastScene);
    }
}