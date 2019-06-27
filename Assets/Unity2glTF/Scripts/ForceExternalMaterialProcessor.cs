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

public class ForceExternalMaterialProcessor : AssetPostprocessor
{
    void OnPreprocessModel()
    {
#if UNITY_2018_1_OR_NEWER
        var importSettingsMissing = assetImporter.importSettingsMissing;
#else
        var importSettingsMissing = !System.IO.File.Exists(AssetDatabase.GetTextMetaFilePathFromAssetPath(assetPath));
#endif
        if (!importSettingsMissing)
            return; // Asset imported already, do not process.

        var modelImporter = assetImporter as ModelImporter;
        modelImporter.materialLocation = ModelImporterMaterialLocation.External;
    }
}
#endif