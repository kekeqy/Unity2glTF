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
using System.Collections.Generic;

public class ForceExternalMaterialProcessor : AssetPostprocessor
{
    //用来保存贴图的路径
    static List<string> list = new List<string>();
    void OnPreprocessModel()
    {
        list.Clear();
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
        list.Add(assetPath);

    }
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        //资源释放顺序Win：贴图-->材质-->物体，Linux：材质-->物体-->贴图
        //Linux存在材质无贴图的问题
        //以下代码为彻底解决Linux材质无贴图问题。
        if (importedAssets.Length > 0)
        {
            foreach (string texturePath in list)
            {
                string name = Path.GetFileNameWithoutExtension(texturePath);
                string ext = Path.GetExtension(texturePath);
                string matPath = texturePath.Replace("/" + name + ext, "");
                int index = matPath.LastIndexOf("/");
                matPath = matPath.Substring(0, index + 1);
                matPath += "Materials/" + name + ".mat";
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
                if (mat && !mat.mainTexture)
                {
                    mat.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
                }
            }
        }
    }
}
#endif