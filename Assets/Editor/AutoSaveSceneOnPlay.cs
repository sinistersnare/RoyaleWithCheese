#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class AutoSaveSceneOnPlay: ScriptableObject {
	static AutoSaveSceneOnPlay() {
        EditorApplication.playModeStateChanged += OnStateChanged;
	}

    private static void OnStateChanged(PlayModeStateChange state) {
        Scene currentScene = SceneManager.GetActiveScene();
        if (state == PlayModeStateChange.ExitingEditMode) {
            Debug.Log("Auto-Saving scene before entering Play mode: " + currentScene.name);
            bool saveOK = EditorSceneManager.SaveScene(currentScene, currentScene.path);
            if (!saveOK) Debug.Log("SAVE FAILED");
            AssetDatabase.SaveAssets();
        }
    }
}
#endif