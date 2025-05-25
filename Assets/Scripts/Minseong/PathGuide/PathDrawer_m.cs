using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class PathDrawer_m : MonoBehaviour
{
    public static PathDrawer_m Instance { get; private set; }

    [SerializeField] private float repathInterval = 0.3f;   // 조금 짧게 주기

    [SerializeField] private bool useSmooth = false;

    private LineRenderer line;
    private Coroutine repathRoutine;
    private NavMeshPath navPath;

    private Transform startTf;
    private Transform endTf;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        line = GetComponent<LineRenderer>();
        line.positionCount = 0;
        navPath = new NavMeshPath();
    }

    public void DrawPath(Transform fromTf, Transform toTf)
    {
        startTf = fromTf;
        endTf = toTf;

        if (repathRoutine != null) StopCoroutine(repathRoutine);
        repathRoutine = StartCoroutine(RepathLoop());
    }

    private List<Vector3> SmoothPath(Vector3[] corners,
                                 int baseSub = 4,
                                 bool useChaikin = false)
    {
        // ---------- 1) Chaikin 1~2회로 먼저 날카로운 꼭짓점 둥글리기 ----------
        List<Vector3> pts = new List<Vector3>(corners);
        if (useChaikin)
        {
            int chaikinIter = 2;               // 필요시 1 로 줄여도 OK
            for (int n = 0; n < chaikinIter; n++)
            {
                List<Vector3> tmp = new();
                tmp.Add(pts[0]);               // 첫 점 유지
                for (int i = 0; i < pts.Count - 1; i++)
                {
                    Vector3 Q = Vector3.Lerp(pts[i], pts[i + 1], 0.25f);
                    Vector3 R = Vector3.Lerp(pts[i], pts[i + 1], 0.75f);
                    tmp.Add(Q);
                    tmp.Add(R);
                }
                tmp.Add(pts[^1]);              // 마지막 점 유지
                pts = tmp;
            }
        }

        // ---------- 2) Catmull-Rom 보간 ----------
        List<Vector3> outPts = new();
        for (int i = 0; i < pts.Count - 1; i++)
        {
            Vector3 p0 = i == 0 ? pts[i] : pts[i - 1];
            Vector3 p1 = pts[i];
            Vector3 p2 = pts[i + 1];
            Vector3 p3 = (i + 2 < pts.Count) ? pts[i + 2] : p2;

            // 코너 거리로 분할 수 가변 (짧은 구간은 2~3, 긴 구간은 8~10)
            float segLen = Vector3.Distance(p1, p2);
            int sub = Mathf.Clamp(Mathf.RoundToInt(segLen * 2f) + baseSub, 3, 10);

            for (int j = 0; j <= sub; j++)
            {
                float t = j / (float)sub;
                Vector3 point =
                    0.5f * ((2f * p1) +
                            (-p0 + p2) * t +
                            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t * t +
                            (-p0 + 3f * p1 - 3f * p2 + p3) * t * t * t);
                outPts.Add(point + Vector3.up * 0.05f); // 살짝 들어 올려 Z-fighting 방지
            }
        }
        return outPts;
    }

    private IEnumerator RepathLoop()
    {
        while (true)
        {
            if (startTf != null && endTf != null &&
                NavMesh.CalculatePath(startTf.position, endTf.position, NavMesh.AllAreas, navPath) &&
                navPath.status == NavMeshPathStatus.PathComplete &&
                navPath.corners.Length > 1)
            {
                if (useSmooth)
                {
                    var pts = SmoothPath(navPath.corners, baseSub: 4, useChaikin: false);
                    line.positionCount = pts.Count;
                    line.SetPositions(pts.ToArray());
                }
                else
                {
                    // 🔹 직선: NavMesh 코너 그대로
                    line.positionCount = navPath.corners.Length;
                    line.SetPositions(navPath.corners);
                }
            }
            else line.positionCount = 0;

            yield return new WaitForSeconds(repathInterval);
        }
    }
}
