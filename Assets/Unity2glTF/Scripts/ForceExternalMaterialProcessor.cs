#if UNITY_EDITOR
// An asset postprocessor that sets the material setting of a model to "Use External Materials (Legacy)".
//
// It only processes an asset if it's a new one, that didn't exist in the project yet.
// Duplicating an asset inside Unity does not count as new asset in  this case.
// It counts as new asset if the .meta file is missing.
//
// Save this file as: Assets/Editor/ForceExternalMaterialProcessor.cs
//
// Download latest version at: https://bitbucket.org/snippets/pschraut/Eea64L
//
using UnityEngine;
using UnityEditor;
using System.IO;

public class ForceExternalMaterialProcessor : AssetPostprocessor
{
    void OnPreprocessModel()
    {
#if UNITY_2018_1_OR_NEWER
        var importSettingsMissing = assetImporter.importSettingsMissing;
#else
        var importSettingsMissing = !File.Exists(AssetDatabase.GetTextMetaFilePathFromAssetPath(assetPath));
#endif
        if (!importSettingsMissing)
            return; // Asset imported already, do not process.

        var modelImporter = assetImporter as ModelImporter;
        modelImporter.materialLocation = ModelImporterMaterialLocation.External;
    }
    void OnPostprocessTexture(Texture2D texture)
    {
        //资源释放顺序Win：贴图-->材质-->物体，Linux：材质-->物体-->贴图
        //以下代码为解决Linux材质无贴图问题
        string name = Path.GetFileNameWithoutExtension(assetPath);
        foreach (var kv in assetImporter.GetExternalObjectMap())
        {
            if (kv.Key.name == name && kv.Key.type == typeof(Material))
            {
                Material mat = kv.Value as Material;
                if (!mat.mainTexture)
                {
                    mat.mainTexture = texture;
                }
                break;
            }
        }
    }
}
#endif