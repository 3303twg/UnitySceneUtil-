using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class SceneMenuGenerator
{
    private const string GeneratorFileName = "SceneMenuGenerator.cs";
    private const string GeneratedFileName = "GeneratedSceneMenu.cs";

    public static string OutputDirectory
    {
        get
        {
            string[] guids = AssetDatabase.FindAssets("SceneMenuGenerator t:MonoScript");

            foreach (string guid in guids)
            {
                string scriptPath = AssetDatabase.GUIDToAssetPath(guid).Replace('\\', '/');

                if (Path.GetFileName(scriptPath) == GeneratorFileName)
                    return Path.GetDirectoryName(scriptPath).Replace('\\', '/');
            }

            return "Assets/Script/Editor/SceneMenu";
        }
    }

    public static string GeneratedFilePath =>
        $"{OutputDirectory}/{GeneratedFileName}";

    public static void EnsureGenerated()
    {
        string directory = OutputDirectory;
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        if (File.Exists(GeneratedFilePath))
            return;

        WriteMinimalStub();
    }

    private static void WriteMinimalStub()
    {
        const string stub = @"using UnityEditor;

public static class GeneratedSceneMenu
{
    [MenuItem(""Scenes/Settings..."", false, 9999)]
    public static void OpenSettings()
    {
        SceneMenuSettingsWindow.ShowWindow();
    }
}
";

        File.WriteAllText(GeneratedFilePath, stub);
        AssetDatabase.Refresh();
    }

    public static void Generate()
    {
        string[] guids = AssetDatabase.FindAssets("t:Scene");

        var scenePaths = guids
            .Select(AssetDatabase.GUIDToAssetPath)
            .OrderBy(path => Path.GetFileNameWithoutExtension(path))
            .ToList();

        StringBuilder sb = new();

        sb.AppendLine("using UnityEditor;");
        sb.AppendLine("using UnityEditor.SceneManagement;");
        sb.AppendLine();
        sb.AppendLine("public static class GeneratedSceneMenu");
        sb.AppendLine("{");

        int menuPriority = 0;
        int methodIndex = 0;

        foreach (string scenePathRaw in scenePaths)
        {
            if (!SceneMenuSettings.IsVisible(scenePathRaw))
                continue;

            string scenePath = scenePathRaw.Replace("\\", "/");
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);

            sb.AppendLine($@"
    [MenuItem(""Scenes/{sceneName}"", false, {menuPriority++})]
    public static void OpenScene_{methodIndex++}()
    {{
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {{
            EditorSceneManager.OpenScene(""{scenePath}"");
        }}
    }}
");
        }

        sb.AppendLine(@"
    [MenuItem(""Scenes/Settings..."", false, 9999)]
    public static void OpenSettings()
    {
        SceneMenuSettingsWindow.ShowWindow();
    }
");

        sb.AppendLine("}");

        string newCode = sb.ToString();
        string directory = OutputDirectory;

        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        if (File.Exists(GeneratedFilePath))
        {
            string oldCode = File.ReadAllText(GeneratedFilePath);

            if (oldCode == newCode)
                return;
        }

        File.WriteAllText(GeneratedFilePath, newCode);

        Debug.Log("[SceneMenu] 갱신 완료");
        AssetDatabase.Refresh();
    }
}
