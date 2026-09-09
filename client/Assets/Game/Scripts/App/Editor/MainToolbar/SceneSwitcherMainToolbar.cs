#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;

namespace DuckDoku.App
{
    [InitializeOnLoad]
    public static class SceneSwitcherMainToolbar
    {
        private const string ScenesFolder = "Assets/Game/Scenes";
        private const string ElementName = "DuckDoku/Scene Switcher";

        static SceneSwitcherMainToolbar()
        {
            EditorSceneManager.activeSceneChangedInEditMode += (_, _) => MainToolbar.Refresh(ElementName);
        }

        [MainToolbarElement(ElementName, defaultDockPosition = MainToolbarDockPosition.Middle)]
        private static IEnumerable<MainToolbarElement> CreateSceneSwitcher()
        {
            yield return new MainToolbarDropdown(
                new MainToolbarContent(GetCurrentSceneLabel(), "Switch the currently open scene"),
                ShowSceneMenu);
        }

        private static string GetCurrentSceneLabel()
        {
            return EditorSceneManager.GetActiveScene().name;
        }

        private static void ShowSceneMenu(Rect dropDownRect)
        {
            var menu = new GenericMenu();
            var activeScenePath = EditorSceneManager.GetActiveScene().path;

            foreach (var scenePath in GetScenePaths())
            {
                var sceneName = Path.GetFileNameWithoutExtension(scenePath);
                var isActive = scenePath == activeScenePath;

                menu.AddItem(new GUIContent(sceneName), isActive, () => OpenScene(scenePath));
            }

            menu.DropDown(dropDownRect);
        }

        private static string[] GetScenePaths()
        {
            return AssetDatabase.FindAssets("t:Scene", new[] { ScenesFolder })
                .Select(AssetDatabase.GUIDToAssetPath)
                .OrderBy(path => path)
                .ToArray();
        }

        private static void OpenScene(string scenePath)
        {
            if (EditorSceneManager.GetActiveScene().path == scenePath)
            {
                return;
            }

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        }
    }
}
#endif
