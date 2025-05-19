using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;

public class LoadSceneManager : MonoBehaviour
{
    public static LoadSceneManager Instance { get { return instance; } }

    public CanvasGroup Fade_img;
    public Text Loading_text;
    public Slider ProgressBar; // 🎯 ProgressBar
    private static LoadSceneManager instance;
    float fadeDuration = 2;

    void Start()
    {
        if (instance != null)
        {
            DestroyImmediate(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        // 🎯 ProgressBar & Text 비활성화 (처음엔 안 보이게)
        ProgressBar.gameObject.SetActive(false);
        Loading_text.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Fade_img.DOFade(0, fadeDuration)
        .OnComplete(() => Fade_img.blocksRaycasts = false);
    }

    public void ChangeScene(string sceneName)
    {
        Fade_img.DOFade(1, fadeDuration)
        .OnStart(() => Fade_img.blocksRaycasts = true)
        .OnComplete(() => StartCoroutine(LoadScene(sceneName)));
    }

    IEnumerator LoadScene(string sceneName)
    {
        // 🎯 ProgressBar & Text 활성화
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
                percentage = Mathf.Lerp(percentage, async.progress * 100f, past_time);
                if (percentage >= 90) past_time = 0;
            }

            // 🎯 ProgressBar 업데이트
            ProgressBar.value = percentage / 100f;
            Loading_text.text = percentage.ToString("0") + "%";
        }

        // 🎯 씬 전환 후 ProgressBar 숨기기
        ProgressBar.gameObject.SetActive(false);
        Loading_text.gameObject.SetActive(false);
    }
}