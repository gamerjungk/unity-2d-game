using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;
using TMPro;
public class LoadSceneManager : MonoBehaviour
{

    // 싱글턴 인스턴스
    public static LoadSceneManager Instance { get { return instance; } }

    public CanvasGroup Fade_img; // 페이드 인/아웃용 이미지 
    public TextMeshProUGUI Loading_text;  // 로딩 퍼센트 텍스트 표시
    public Slider ProgressBar; // // 로딩 진행 상황 표시 슬라이더
    private static LoadSceneManager instance;
    float fadeDuration = 2;

    void Start()
    {
        //인스턴스 중복 방지
        if (instance != null)
        {
            DestroyImmediate(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        // ProgressBar & Text 비활성화 (처음엔 안 보이게)
        ProgressBar.gameObject.SetActive(false);
        Loading_text.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        // 씬 로드 이벤트 해제 (메모리 누수 방지)
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 화면 어둡게(1) → 투명(0) 전환하면서 UI 상호작용 허용
        Fade_img.DOFade(0, fadeDuration)
        .OnComplete(() => Fade_img.blocksRaycasts = false);
    }

    public void ChangeScene(string sceneName)
    {
        // 투명 → 어둡게 전환하면서 터치 막고, 로딩 시작
        Fade_img.DOFade(1, fadeDuration)
        .OnStart(() => Fade_img.blocksRaycasts = true)
        .OnComplete(() => StartCoroutine(LoadScene(sceneName)));
    }

    // 실제 비동기 씬 로딩 처리 코루틴
    IEnumerator LoadScene(string sceneName)
    {
        //  ProgressBar & Text 활성화
        ProgressBar.gameObject.SetActive(true);
        Loading_text.gameObject.SetActive(true);

        ProgressBar.value = 0;
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;

        float past_time = 0;
        float percentage = 0;

        while (!async.isDone)
        {
            yield return null;
            past_time += Time.deltaTime;
            // 로딩이 거의 끝났을 때(90% 이상), 부드럽게 100%로 증가
            if (percentage >= 90)
            {
                percentage = Mathf.Lerp(percentage, 100, past_time);
                if (percentage >= 99.5f)
                {
                    async.allowSceneActivation = true;
                }
            }
            else
            {
                // 일반 로딩 상태에서는 실제 progress 기반으로 증가
                percentage = Mathf.Lerp(percentage, async.progress * 100f, past_time);
                if (percentage >= 90) past_time = 0;
            }

            //  ProgressBar 업데이트
            ProgressBar.value = percentage / 100f;
            Loading_text.text = percentage.ToString("0") + "%";
        }

        //  씬 전환 후 ProgressBar 숨기기
        ProgressBar.gameObject.SetActive(false);
        Loading_text.gameObject.SetActive(false);
    }
}

//게임을 할때 게임을 불러오는 과정에서 시간이 걸리는데 이때 그냥 씬을 바꾸는것보다는 씬을 바꾸는 중에 게임에 컨셉에 맞게 배달하는 사람이 로딩바를 채우는 로딩 씬이 좋다고 생각함