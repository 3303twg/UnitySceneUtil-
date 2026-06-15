using UnityEditor;

public static class SceneMenuSettings
{
    private const string Prefix = "SceneMenu_";

    public static bool IsVisible(string scenePath)
    {
        return EditorPrefs.GetBool(Prefix + scenePath, true);
    }

    public static void SetVisible(string scenePath, bool visible)
    {
        EditorPrefs.SetBool(Prefix + scenePath, visible);
    }
}
