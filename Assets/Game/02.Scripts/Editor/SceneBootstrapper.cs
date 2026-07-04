using Game.App;
using Game.Core;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Game.Editor
{
    public static class SceneBootstrapper
    {
        public static void CreateGameScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            var camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 5f;
            camera.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 1f);
            camera.clearFlags = CameraClearFlags.SolidColor;
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);

            var lifetimeScopeObject = new GameObject("GameSceneLifetimeScope");
            lifetimeScopeObject.AddComponent<GameSceneLifetimeScope>();

            if (!AssetDatabase.IsValidFolder("Assets/Game/01.Scene"))
            {
                AssetDatabase.CreateFolder("Assets/Game", "01.Scene");
            }

            var scenePath = "Assets/Game/01.Scene/GameScene.unity";
            var isSaved = EditorSceneManager.SaveScene(scene, scenePath);

            if (!isSaved)
            {
                GameLogger.LogError($"[SceneBootstrapper] Failed to save scene: {scenePath}");
            }
            else
            {
                GameLogger.Log($"[SceneBootstrapper] Scene created and saved: {scenePath}");
            }
        }
    }
}
