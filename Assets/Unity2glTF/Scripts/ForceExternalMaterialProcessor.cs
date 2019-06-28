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
    void OnPostprocessModel(GameObject go)
    {
        Debug.Log("go");
        string ext = Path.GetExtension(assetPath);
        string texturePath = assetPath.Replace(ext, ".fbm");
        Renderer[] renderers = go.transform.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (!renderer.sharedMaterial.mainTexture)
            {
                string[] temp = AssetDatabase.FindAssets(renderer.sharedMaterial.name, new string[] { texturePath });
                if (temp.Length == 1)
                {
                    renderer.sharedMaterial.mainTexture = AssetDatabase.LoadAssetAtPath<Texture>(AssetDatabase.GUIDToAssetPath(temp[0]));
                }
            }
        }
    }
    void OnPostprocessMaterial(Material mat)
    {
        Debug.Log("mat");
    }
    void OnPostprocessTexture(Texture2D texture)
    {
        Debug.Log("tex");
        string name = Path.GetFileNameWithoutExtension(assetPath);
        string ext = Path.GetExtension(assetPath);
        string path = assetPath.Replace(".fbm/" + name + ext, "");
        int index = path.LastIndexOf("/");
        path = path.Substring(0, index+1);
        path += "Materials/" + name + ".mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if(mat && !mat.mainTexture)
        {
            mat.mainTexture = texture;
        }
    }
}
#endif