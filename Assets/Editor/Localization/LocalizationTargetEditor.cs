using UnityEditor;
using UnityEngine;
using System;
using System.Linq;

[CustomEditor(typeof(LocalizationTarget))]
public class LocalizationTargetEditor : Editor
{
    private string[] sortedKeys;

    private void OnEnable()
    {
        // 알파벳순으로 정렬
        sortedKeys = LocalizationKeys.Keys.OrderBy(k => k).ToArray();
    }

    public override void OnInspectorGUI()
    {
        var targetScript = (LocalizationTarget)target;

        EditorGUI.BeginChangeCheck();

        // 현재 key가 정렬된 리스트에서 몇 번째에 있는지 찾기
        int selectedIndex = Mathf.Max(0, Array.IndexOf(sortedKeys, targetScript.key));
        selectedIndex = EditorGUILayout.Popup("Key", selectedIndex, sortedKeys);
        string newKey = sortedKeys[selectedIndex];

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(targetScript, "Change Localization Key");
            targetScript.key = newKey;
            EditorUtility.SetDirty(targetScript);
        }
    }
}
