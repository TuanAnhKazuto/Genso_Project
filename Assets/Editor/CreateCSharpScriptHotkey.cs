#if UNITY_EDITOR
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public static class CreateCSharpScriptHotkey
{
    private const string DefaultScriptName = "NewScript";

    // Hotkey:
    // Windows/Linux: Ctrl + Shift + C
    // macOS: Cmd + Shift + C
    [MenuItem("Tools/Create C# Script %#&c")]
    private static void CreateCSharpScript()
    {
        string folderPath = GetSelectedFolderPath();

        string assetPath = EditorUtility.SaveFilePanelInProject(
            "Create C# Script",
            DefaultScriptName,
            "cs",
            "Nhập tên script C# cần tạo.",
            folderPath
        );

        if (string.IsNullOrWhiteSpace(assetPath))
            return;

        string className = Path.GetFileNameWithoutExtension(assetPath);

        if (!IsValidClassName(className))
        {
            EditorUtility.DisplayDialog(
                "Tên script không hợp lệ",
                "Tên file cũng sẽ là tên class.\n\n" +
                "Chỉ nên dùng chữ, số, dấu gạch dưới và không bắt đầu bằng số.\n\n" +
                "Ví dụ hợp lệ: PlayerMovement, EnemyAI, GameManager",
                "OK"
            );

            return;
        }

        string fullPath = Path.GetFullPath(assetPath);

        if (File.Exists(fullPath))
        {
            bool overwrite = EditorUtility.DisplayDialog(
                "File đã tồn tại",
                $"{assetPath} đã tồn tại. Bạn có muốn ghi đè không?",
                "Ghi đè",
                "Hủy"
            );

            if (!overwrite)
                return;
        }

        string scriptContent = CreateScriptTemplate(className);

        File.WriteAllText(fullPath, scriptContent, new UTF8Encoding(false));

        AssetDatabase.ImportAsset(assetPath);
        AssetDatabase.Refresh();

        MonoScript scriptAsset = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);

        Selection.activeObject = scriptAsset;
        EditorGUIUtility.PingObject(scriptAsset);
    }

    private static string GetSelectedFolderPath()
    {
        if (Selection.activeObject == null)
            return "Assets/Scripts";

        string selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);

        if (string.IsNullOrEmpty(selectedPath))
            return "Assets/Scripts";

        if (AssetDatabase.IsValidFolder(selectedPath))
            return selectedPath;

        string directory = Path.GetDirectoryName(selectedPath);

        if (string.IsNullOrEmpty(directory))
            return "Assets/Scripts";

        return directory.Replace("\\", "/");
    }

    private static bool IsValidClassName(string className)
    {
        return Regex.IsMatch(className, @"^[A-Za-z_][A-Za-z0-9_]*$");
    }

    private static string CreateScriptTemplate(string className)
    {
        return
$@"using UnityEngine;

public class {className} : MonoBehaviour
{{
    
}}
";
    }
}
#endif