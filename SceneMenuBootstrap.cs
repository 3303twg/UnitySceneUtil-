using UnityEditor;

[InitializeOnLoad]
public static class SceneMenuBootstrap
{
    static SceneMenuBootstrap()
    {
        SceneMenuGenerator.EnsureGenerated();
        EditorApplication.delayCall += SceneMenuGenerator.Generate;
    }
}
