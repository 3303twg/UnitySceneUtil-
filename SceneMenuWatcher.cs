using System.Linq;
using UnityEditor;

public class SceneMenuWatcher : AssetPostprocessor
{
    static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        bool hasSceneChange =
            importedAssets.Any(x => x.EndsWith(".unity")) ||
            deletedAssets.Any(x => x.EndsWith(".unity")) ||
            movedAssets.Any(x => x.EndsWith(".unity")) ||
            movedFromAssetPaths.Any(x => x.EndsWith(".unity"));

        if (!hasSceneChange)
            return;

        EditorApplication.delayCall += SceneMenuGenerator.Generate;
    }
}
