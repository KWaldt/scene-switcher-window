using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KristinaWaldt
{
    public class SceneSwitcherWindow : EditorWindow
    {
        private Vector2 _scrollPos;
        private Vector2 _infoScrollPos;
        private bool _showInfo;
        
        [MenuItem("Window/General/Scene Switcher")]
        public static void ShowWindow()
        {
            GetWindow(typeof(SceneSwitcherWindow), false, "Scene Switcher");
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, false, false);

            // TODO
            var activeScene = SceneManager.GetActiveScene();
            bool isActiveSceneAdded = false;
            for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                EditorBuildSettingsScene scene = EditorBuildSettings.scenes[i];

                if (!isActiveSceneAdded && scene.path == activeScene.path)
                {
                    isActiveSceneAdded = true;
                }
                
                if (!scene.enabled)
                    continue;

                string sceneName = Path.GetFileNameWithoutExtension(scene.path);

                GUILayout.BeginHorizontal();
                DrawAddAdditiveButton(scene);
                DrawAddSingleButton(i, sceneName, scene);
                GUILayout.EndHorizontal();
            }
            
            DrawAddCurrentSceneButton(activeScene, isActiveSceneAdded);

            EditorGUILayout.EndScrollView();
            
            DrawInfoFoldout();

            EditorGUILayout.EndVertical();
        }

        private void DrawAddCurrentSceneButton(Scene scene, bool isAdded)
        {
            if (isAdded || Application.isPlaying) return;

            string sceneName = scene.name;
            if (!GUILayout.Button($"Add '{sceneName}' to build settings")) return;
            
            var buildSettingsScenes = EditorBuildSettings.scenes.ToList();
            buildSettingsScenes.Add(new EditorBuildSettingsScene(scene.path, true));

            EditorBuildSettings.scenes = buildSettingsScenes.ToArray();
        }

        private void DrawInfoFoldout()
        {
            _showInfo = EditorGUILayout.Foldout(_showInfo, "Info");
            if (_showInfo)
            {
                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, false, false);

                GUILayout.Label("You can also load scenes during playmode to teleport yourself. Use with caution.",
                    EditorStyles.helpBox);
            
                GUILayout.Label(
                    "The plus loads scenes additively, the button itself replaces all scenes with the selection.",
                    EditorStyles.helpBox);
            
                GUILayout.Label("Only scenes in Build Settings are visible.", EditorStyles.helpBox);
                
                EditorGUILayout.EndScrollView();
            }
        }

        private void DrawAddSingleButton(int i, string sceneName, EditorBuildSettingsScene scene)
        {
            if (!DrawLeftAlignedButton($"{i:D2}: <b>{sceneName}</b>"))
                return;

            if (!Application.isPlaying && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            LoadScene(scene);
        }

        private static void DrawAddAdditiveButton(EditorBuildSettingsScene scene)
        {
            if (!GUILayout.Button("+", GUILayout.Width(20)))
                return;

            LoadScene(scene, true);
        }

        private bool DrawLeftAlignedButton(string text)
        {
            return GUILayout.Button(text,
                new GUIStyle(GUI.skin.GetStyle("Button")) { alignment = TextAnchor.MiddleLeft, richText = true });
        }

        private static void LoadScene(EditorBuildSettingsScene scene, bool additive = false)
        {
            if (Application.isPlaying)
            {
                SceneManager.LoadScene(Path.GetFileNameWithoutExtension(scene.path),
                    additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
            }
            else
            {
                EditorSceneManager.OpenScene(scene.path, additive ? OpenSceneMode.Additive : OpenSceneMode.Single);
            }
        }
    }
}