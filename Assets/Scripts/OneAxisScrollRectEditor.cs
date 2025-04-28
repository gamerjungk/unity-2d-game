#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(OneAxisScrollRect))]
public class OneAxisScrollRectEditor : ScrollRectEditor
{
    SerializedProperty modeProp;

    protected override void OnEnable()
    {
        base.OnEnable();
        modeProp = serializedObject.FindProperty("mode");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Custom Options", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(modeProp);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
