using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Security.Cryptography;

namespace Uinty2glTF
{
    public class ExportWindow : EditorWindow
    {
        [MenuItem("工具/glTF导出工具")]
        public static void Init()
        {
            ExportWindow window = (ExportWindow)EditorWindow.GetWindow(typeof(ExportWindow), true, "glTF导出工具", true);
            window.autoRepaintOnSceneChange = true;
            window.minSize = new Vector2(300, 420);
            window.maxSize = new Vector2(300, 420);
            window.maximized = false;
            window._selectedCount = Selection.objects.Length;
            window.Show(true);
            window.Focus();
        }

        private const int _spaceSize = 20;
        private const string _selectedCountFormat = "选中物体数量：{0}个";
        private int _selectedCount = 0;
        private GUIStyle _msgStytle;
        private readonly GUILayoutOption _buttonHeight = GUILayout.Height(30);
        private readonly string _errMsgFormat = "<color=#FF0000>{0}</color>";
        private string _errMsg = null;
        private readonly string _successMsgFormat = "<color=#00FF00>{0}</color>";
        private string _successMsg = null;
        private bool _error = false;
        private bool _glb = true;
        private GameObject _exporterGo;
        private SceneToGlTFWiz _exporter;
        public void OnEnable()
        {
            if (_exporterGo == null)
            {
                _exporterGo = new GameObject("Exporter");
                _exporter = _exporterGo.AddComponent<SceneToGlTFWiz>();
                _exporterGo.hideFlags = HideFlags.HideAndDontSave;
            }
        }

        public void OnDisable()
        {
            if (_exporterGo != null)
            {
                DestroyImmediate(_exporterGo);
                _exporterGo = null;
            }

        }
        public void OnSelectionChange()
        {
            _selectedCount = Selection.objects.Length;
            _errMsg = null;
            _error = false;
            _successMsg = null;
            Repaint();
        }
        public void OnGUI()
        {
            if (_msgStytle == null) _msgStytle = new GUIStyle(EditorStyles.label) { richText = true };

            GUILayout.BeginVertical();
            GUILayout.Space(_spaceSize);
            GUILayout.Label(string.Format(_selectedCountFormat, _selectedCount));
            GUILayout.Space(_spaceSize);
            _glb = GUILayout.Toggle(_glb, "生成.glb文件");
            GUILayout.Space(_spaceSize);
            if (GUILayout.Button("导出选中物体", _buttonHeight))
            {
                _errMsg = null;
                _error = false;
                _successMsg = null;
                if (_selectedCount == 0)
                {
                    _error = true;
                    _errMsg = "请在层级列表面板选中一个或多个物体进行导出！";
                    return;
                }
                Export();
            }
            GUILayout.Space(_spaceSize * 2);
            if (_error)
            {
                if (_errMsg != null)
                {
                    GUILayout.Label(string.Format(_errMsgFormat, _errMsg), _msgStytle);
                }
                else
                {
                    GUILayout.Label("");
                }
            }
            else
            {
                if (_successMsg != null)
                {
                    GUILayout.Label(string.Format(_successMsgFormat, _successMsg), _msgStytle);
                }
                else
                {
                    GUILayout.Label("");
                }
            }
            GUILayout.Space(_spaceSize * 2);
            if (GUILayout.Button("清理场景物体", _buttonHeight))
            {
                ClearScene();
            }
            GUILayout.Space(_spaceSize);
            if (GUILayout.Button("清理资源文件", _buttonHeight))
            {
                ClearResources();
            }
            GUILayout.Space(_spaceSize);
            if (GUILayout.Button("打开项目文件夹", _buttonHeight))
            {
                OpenProjectDir();
            }
            GUILayout.EndVertical();
        }
        private void Export()
        {
            InitExportPath();
            string mExportPath = Directory.GetParent(Application.dataPath).FullName + "/export";
            RNGCryptoServiceProvider csp = new RNGCryptoServiceProvider();
            byte[] byteCsp = new byte[4];
            csp.GetBytes(byteCsp);
            string mParamName = System.BitConverter.ToString(byteCsp).Replace("-", null);
            string exportFileName = Path.Combine(mExportPath, mParamName + ".gltf");
            var callBack = new System.Action<bool, string>((bool state, string msg) =>
               {
                   _error = !state;
                   if (state) _successMsg = msg;
                   else _errMsg = msg;
               });
            _exporter.ExportCoroutine(exportFileName, null, _glb, true, true, false, callBack);
        }
        private void InitExportPath()
        {
            string exprotPath = Directory.GetParent(Application.dataPath).FullName + "/export";
            if (Directory.Exists(exprotPath))
            {
                DirectoryHelper.DeleteFolder2(exprotPath);
            }
            if (!Directory.Exists(exprotPath))
            {
                Directory.CreateDirectory(exprotPath);
            }
        }
        private void ClearScene()
        {
            GameObject[] objs = SceneManager.GetActiveScene().GetRootGameObjects();
            for (int i = 0; i < objs.Length; i++) Object.DestroyImmediate(objs[i]);
            EditorSceneManager.MarkAllScenesDirty();
            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        private void OpenProjectDir()
        {
            string root = Directory.GetParent(Application.dataPath).FullName;
            System.Diagnostics.Process.Start(root);
        }
        private void ClearResources()
        {
            string scene = SceneManager.GetActiveScene().path;
            string[] array = AssetDatabase.GetAllAssetPaths();
            for (int i = 0; i < array.Length; i++)
            {
                string path = array[i];
                if (!path.StartsWith("Assets") || path.StartsWith("Assets/Unity2glTF") || path == scene || path == "Assets") continue;
                AssetDatabase.DeleteAsset(path);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}