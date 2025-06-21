using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

// 해당 cs가 붙은 오브젝트에 LineRenderer 컴포넌트가 반드시 필요
[RequireComponent(typeof(LineRenderer))]
public class PathDrawer_m : MonoBehaviour
{
    // 싱글턴 인스턴스 프로퍼티
    public static PathDrawer_m Instance { get; private set; }

    // 경로 갱신 주기를 설정 (초 단위)
    [SerializeField] private float repathInterval = 0.3f;

    // 부드러운 곡선 경로 사용 여부  
    [SerializeField] private bool useSmooth = false;

    private LineRenderer line; // 경로 시각화를 위한 LineRenderer 참조  
    private Coroutine repathRoutine; // 경로 갱신 코루틴 참조  
    private NavMeshPath navPath; // NavMesh.CalculatePath 결과를 저장할 객체  

    private Transform startTf; // 경로 시작 지점 Transform  
    private Transform endTf; // 경로 목표 지점 Transform

    void Awake()
    {
        // 싱글턴이 아직 설정되지 않았으면 자신을 인스턴스로 설정
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; } // 이미 존재하면 중복 방지 차원에서 파괴

        line = GetComponent<LineRenderer>(); // LineRenderer 컴포넌트 가져오기  
        line.positionCount = 0; // 초기에는 점 개수를 0으로 설정  
        navPath = new NavMeshPath(); // NavMeshPath 객체 생성  
    }

    // 경로 그리기 요청 함수
    public void DrawPath(Transform fromTf, Transform toTf)
    {
        startTf = fromTf; // 시작 지점 설정
        endTf = toTf; // 목표 지점 설정

        // 이미 코루틴 실행 중이면 중단하여 중복 실행 방지
        if (repathRoutine != null) StopCoroutine(repathRoutine);

        // 갱신 코루틴 시작
        repathRoutine = StartCoroutine(RepathLoop());
    }

    private List<Vector3> SmoothPath(Vector3[] corners,
                                 int baseSub = 4,
                                 bool useChaikin = false)
    {
        // ---------- 1) Chaikin 1~2회로 먼저 날카로운 꼭짓점 둥글리기 ----------
        List<Vector3> pts = new List<Vector3>(corners); // 코너 배열을 리스트로 복사
        if (useChaikin)
        {
            int chaikinIter = 2; // chaikin 반복 횟수 설정
            for (int n = 0; n < chaikinIter; n++)
            {
                List<Vector3> tmp = new(); // 임시 리스트 생성
                tmp.Add(pts[0]); // 첫 점 유지
                for (int i = 0; i < pts.Count - 1; i++)
                {
                    Vector3 Q = Vector3.Lerp(pts[i], pts[i + 1], 0.25f); // 구간 25% 지점
                    Vector3 R = Vector3.Lerp(pts[i], pts[i + 1], 0.75f); // 구간 75% 지점
                    tmp.Add(Q);
                    tmp.Add(R);
                }
                tmp.Add(pts[^1]); // 마지막 점 유지
                pts = tmp; // 부드러워진 리스트로 교체
            }
        }

        // ---------- 2) Catmull-Rom 보간 ----------
        List<Vector3> outPts = new();
        for (int i = 0; i < pts.Count - 1; i++)
        {
            Vector3 p0 = i == 0 ? pts[i] : pts[i - 1]; // 이전 점(없으면 현재 점)
            Vector3 p1 = pts[i]; // 현재 점
            Vector3 p2 = pts[i + 1]; // 다음 점
            Vector3 p3 = (i + 2 < pts.Count) ? pts[i + 2] : p2; // 그다음 점(없으면 p2)

            // 코너 거리로 분할 수 가변 (짧은 구간은 2~3, 긴 구간은 8~10)
            float segLen = Vector3.Distance(p1, p2); // 구간 길이 계산
            int sub = Mathf.Clamp(Mathf.RoundToInt(segLen * 2f) + baseSub, 3, 10); // 분할 개수 결정

            // 분할만큼 보간
            for (int j = 0; j <= sub; j++)
            {
                // 보간 파라미처
                float t = j / (float)sub;
                // Catmull-Rom 공식
                Vector3 point =
                    0.5f * ((2f * p1) +
                            (-p0 + p2) * t +
                            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t * t +
                            (-p0 + 3f * p1 - 3f * p2 + p3) * t * t * t);
                outPts.Add(point + Vector3.up * 0.05f); // Z-fighting 방지를 위해 살짝 올림
            }
        }
        return outPts; // 부드러운 경로 점 리스트 반환
    }

    // 경로를 주기적으로 재계산하는 코루틴
    private IEnumerator RepathLoop()
    {
        // 무한 루프
        while (true)
        {
            // 경로 계산해서 완전 경로인지 충분한 코너가 있는지 확인
            if (startTf != null && endTf != null &&
                NavMesh.CalculatePath(startTf.position, endTf.position, NavMesh.AllAreas, navPath) &&
                navPath.status == NavMeshPathStatus.PathComplete &&
                navPath.corners.Length > 1)
            {
                // 부드러운 점으로 라인 설정
                if (useSmooth)
                {
                    var pts = SmoothPath(navPath.corners, baseSub: 4, useChaikin: false);
                    line.positionCount = pts.Count;
                    line.SetPositions(pts.ToArray());
                }
                else
                {
                    // 직선: NavMesh 코너 그대로
                    line.positionCount = navPath.corners.Length;
                    line.SetPositions(navPath.corners);
                }
            }
            else line.positionCount = 0; // 유효 경로 없으면 라인 제거

            yield return new WaitForSeconds(repathInterval); // 주기만큼 대기
        }
    }
}
