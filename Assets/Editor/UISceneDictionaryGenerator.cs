using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class UITypeDictionaryGenerator : EditorWindow
{
    [MenuItem("Tools/Generate UI Type Dictionaries")]
    public static void ShowWindow()
    {
        GetWindow<UITypeDictionaryGenerator>("UI Type Dict Generator");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Generate UI Dictionaries and Copy to Clipboard"))
        {
            GenerateAndCopyDictionaries();
        }
    }

    private void GenerateAndCopyDictionaries()
    {
        string result = "";

        // 1. UI_Scene Dictionary 생성
        var sceneTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && typeof(UI_Scene).IsAssignableFrom(t))
            .ToList();

        result += "// 자동 생성된 UI_Scene Dictionary\n";
        result += "new Dictionary<string, Action>\n{\n";
        foreach (var type in sceneTypes)
        {
            result += $"    {{ UITypeCache<{type.Name}>.Name, () => UIManager.Instance.ChangeSceneUI<{type.Name}>() }},\n";
        }
        result += "};\n\n";

        // 2. UI_Popup Dictionary 생성
        var popupTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && typeof(UI_Popup).IsAssignableFrom(t))
            .ToList();

        result += "// 자동 생성된 UI_Popup Dictionary\n";
        result += "new Dictionary<string, Action>\n{\n";
        foreach (var type in popupTypes)
        {
            result += $"    {{ UITypeCache<{type.Name}>.Name, () => UIManager.Instance.ShowPopup<{type.Name}>() }},\n";
        }
        result += "};";

        // 콘솔 출력
        Debug.Log(result);

        // 클립보드에 자동 복사
        GUIUtility.systemCopyBuffer = result;
        Debug.Log("[UI Type Dictionaries] Copied to clipboard!");
    }
}