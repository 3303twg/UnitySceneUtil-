using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SceneMenuSettingsWindow : EditorWindow
{
    private Vector2 _scroll;

    private bool _isDragging;
    private bool _dragValue;

    private readonly HashSet<string> _dragVisited = new();

    public static void ShowWindow()
    {
        GetWindow<SceneMenuSettingsWindow>("Scene Menu Settings");
    }

    private void OnGUI()
    {
        GUILayout.Label("Visible Scenes", EditorStyles.boldLabel);

        _scroll = EditorGUILayout.BeginScrollView(_scroll);

        string[] guids = AssetDatabase.FindAssets("t:Scene");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string sceneName = Path.GetFileNameWithoutExtension(path);

            bool current = SceneMenuSettings.IsVisible(path);

            Rect rect = EditorGUILayout.GetControlRect(false, 20);

            Event e = Event.current;

            switch (e.type)
            {
                case EventType.MouseDown:
                    {
                        if (rect.Contains(e.mousePosition))
                        {
                            _isDragging = true;
                            _dragVisited.Clear();

                            _dragValue = !current;

                            SceneMenuSettings.SetVisible(path, _dragValue);
                            _dragVisited.Add(path);

                            Repaint();
                            e.Use();
                        }

                        break;
                    }

                case EventType.MouseDrag:
                    {
                        if (_isDragging &&
                            rect.Contains(e.mousePosition) &&
                            !_dragVisited.Contains(path))
                        {
                            SceneMenuSettings.SetVisible(path, _dragValue);
                            _dragVisited.Add(path);

                            Repaint();
                        }

                        break;
                    }

                case EventType.MouseUp:
                    {
                        if (_isDragging)
                        {
                            _isDragging = false;
                            _dragVisited.Clear();

                            Repaint();
                        }

                        break;
                    }
            }

            EditorGUI.ToggleLeft(
                rect,
                sceneName,
                SceneMenuSettings.IsVisible(path));
        }

        EditorGUILayout.EndScrollView();

        if (_isDragging)
        {
            Repaint();
        }

        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Show All"))
        {
            foreach (string guid in AssetDatabase.FindAssets("t:Scene"))
            {
                SceneMenuSettings.SetVisible(
                    AssetDatabase.GUIDToAssetPath(guid),
                    true);
            }

            Repaint();
        }

        if (GUILayout.Button("Hide All"))
        {
            foreach (string guid in AssetDatabase.FindAssets("t:Scene"))
            {
                SceneMenuSettings.SetVisible(
                    AssetDatabase.GUIDToAssetPath(guid),
                    false);
            }

            Repaint();
        }

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Apply"))
        {
            SceneMenuGenerator.Generate();
        }

        if (GUILayout.Button("Close"))
        {
            Close();
        }

        EditorGUILayout.EndHorizontal();
    }
}
