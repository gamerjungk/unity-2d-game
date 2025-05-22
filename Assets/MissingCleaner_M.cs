#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class MissingScriptCleaner
{
    [MenuItem("Tools/Remove Missing Scripts in Project")]
    private static void RemoveMissingScripts()
    {
        int count = 0;

        // ① 씬에 열려 있는 모든 GameObject
        foreach (GameObject go in Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            count += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);

        // ② 에셋(프리팹·SO 등) 전체
        string[] guids = AssetDatabase.FindAssets("t:Prefab t:ScriptableObject");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var obj = AssetDatabase.LoadAssetAtPath<Object>(path);
            if (obj is GameObject go)
                count += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"🗑️  Missing Script 컴포넌트 {count}개 제거 완료");
    }
}
#endif
