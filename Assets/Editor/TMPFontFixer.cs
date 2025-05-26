#if UNITY_EDITOR
using UnityEditor;
using TMPro;
using UnityEngine;

public static class TMPFontFixer
{
    [InitializeOnLoadMethod]
    static void OnEditorLoad()
    {
        FixFonts();
    }

    [MenuItem("Tools/Fix TMP Fonts (Set to Static)")]
    public static void FixFonts()
    {
        TMP_FontAsset[] fonts = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();

        foreach (var font in fonts)
        {
            if (font != null && font.atlasPopulationMode == AtlasPopulationMode.Dynamic)
            {
                Debug.Log($"🛠️ TMP 폰트 '{font.name}' → Static으로 수정됨");
                font.atlasPopulationMode = AtlasPopulationMode.Static;
                EditorUtility.SetDirty(font);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif