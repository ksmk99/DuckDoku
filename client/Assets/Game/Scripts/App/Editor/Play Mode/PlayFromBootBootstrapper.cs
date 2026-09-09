#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DuckDoku.App
{
    [InitializeOnLoad]
    public static class PlayFromBootBootstrapper
    {
        private const string BootScenePath = "Assets/Game/Scenes/Boot.unity";
        private const string PrefsEnabledKey = "DuckDoku.PlayFromBoot.Enabled";
        private const string SessionScenePathKey = "DuckDoku.PlayFromBoot.ScenePathToRestore";
        private const string MenuPath = "Tools/Play From Boot";

        private static bool IsEnabled
        {
            get => EditorPrefs.GetBool(PrefsEnabledKey, true);
            set => EditorPrefs.SetBool(PrefsEnabledKey, value);
        }

        static PlayFromBootBootstrapper()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        [MenuItem(MenuPath, true)]
        private static bool ValidateToggleEnabled()
        {
            Menu.SetChecked(MenuPath, IsEnabled);
            return true;
        }

        [MenuItem(MenuPath)]
        private static void ToggleEnabled()
        {
            IsEnabled = !IsEnabled;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (!IsEnabled || state != PlayModeStateChange.ExitingEditMode)
            {
                return;
            }

            var activeScene = EditorSceneManager.GetActiveScene();

            if (string.IsNullOrEmpty(activeScene.path) || activeScene.path == BootScenePath)
            {
                EditorSceneManager.playModeStartScene = null;
                SessionState.EraseString(SessionScenePathKey);
                return;
            }

            var bootSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(BootScenePath);

            if (bootSceneAsset == null)
            {
                Debug.LogError($"PlayFromBootBootstrapper: Boot scene not found at path: {BootScenePath}. Play mode override skipped.");
                return;
            }

            SessionState.SetString(SessionScenePathKey, activeScene.path);
            EditorSceneManager.playModeStartScene = bootSceneAsset;
        }
    }
}
#endif