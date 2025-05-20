using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class UIAnchorHelper
{
    // Cmd + Option + Shift + A (Mac 기준)
    [MenuItem("Tools/UI/Match Anchors To Current Rect %#&a")]
    public static void MatchAnchors()
    {
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Match Anchors To Current Rect");

        // 현재 프리팹 모드에서 작업 중인지 확인
        PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();

        foreach (Object obj in Selection.objects)
        {
            // 씬 또는 프리팹 내부에서 선택된 오브젝트 처리
            GameObject go = obj as GameObject;
            if (go == null) continue;

            // 프리팹 모드인 경우, 현재 PrefabStage 안의 오브젝트만 처리
            if (stage != null && !go.transform.IsChildOf(stage.prefabContentsRoot.transform))
                continue;

            RectTransform rect = go.GetComponent<RectTransform>();
            if (rect == null || rect.parent == null) continue;

            RectTransform parent = rect.parent as RectTransform;
            if (parent == null || parent.rect.width == 0 || parent.rect.height == 0) continue;

            Vector2 newMin = new Vector2(
                rect.anchorMin.x + rect.offsetMin.x / parent.rect.width,
                rect.anchorMin.y + rect.offsetMin.y / parent.rect.height
            );
            Vector2 newMax = new Vector2(
                rect.anchorMax.x + rect.offsetMax.x / parent.rect.width,
                rect.anchorMax.y + rect.offsetMax.y / parent.rect.height
            );

            Undo.RecordObject(rect, "Match Anchor");
            rect.anchorMin = newMin;
            rect.anchorMax = newMax;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
        }

        Undo.CollapseUndoOperations(undoGroup);
        Debug.Log("✅ Anchors matched to current RectTransform! (Grouped Undo)");
    }
}
