// Assets/Editor/CalcCityBounds.cs
using UnityEditor;
using UnityEngine;

public class CalcCityBounds : MonoBehaviour
{
    [MenuItem("Tools/📏  Calculate City Bounds")]
    static void CalcBounds()
    {
        const string roadTag = "RoadNode";                     // ◀‥ 태그 이름
        var gos = GameObject.FindGameObjectsWithTag(roadTag);  // 태그로 모두 찾기

        if (gos.Length == 0)
        {
            Debug.LogWarning($"❌ Tag \"{roadTag}\" 가 달린 오브젝트가 없습니다!");
            return;
        }

        // 맨 첫 오브젝트로 초기화
        Bounds city = new Bounds(gos[0].transform.position, Vector3.zero);

        foreach (var go in gos)
        {
            // 메시에 렌더러가 붙어 있다면 그것의 bounds 사용
            var r = go.GetComponentInChildren<Renderer>();
            if (r != null) city.Encapsulate(r.bounds);
            else city.Encapsulate(new Bounds(go.transform.position, Vector3.one));
        }

        Debug.Log($"▶ CITY SIZE  :  X = {city.size.x:F1} m ,  Z = {city.size.z:F1} m");
    }
}
