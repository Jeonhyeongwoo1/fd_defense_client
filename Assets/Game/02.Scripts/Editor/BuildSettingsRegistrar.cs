using Game.Core;
using UnityEditor;

namespace Game.Editor
{
    public static class BuildSettingsRegistrar
    {
        private const string OutGameScenePath = "Assets/Game/01.Scene/OutGameScene.unity";
        private const string GameScenePath = "Assets/Game/01.Scene/GameScene.unity";

        public static void RegisterScenes()
        {
            var scenes = new[]
            {
                new EditorBuildSettingsScene(OutGameScenePath, true),
                new EditorBuildSettingsScene(GameScenePath, true)
            };

            EditorBuildSettings.scenes = scenes;

            GameLogger.Log($"[BuildSettingsRegistrar] Registered build scenes: {OutGameScenePath} (index 0), {GameScenePath} (index 1)");
        }

        public static void ConfigureForPortrait()
        {
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;
            PlayerSettings.defaultScreenWidth = 1080;
            PlayerSettings.defaultScreenHeight = 1920;
            AssetDatabase.SaveAssets();
            GameLogger.Log("[BuildSettingsRegistrar] Portrait orientation configured.");
        }
    }
}
