using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    public FadeController fadeController;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadSceneWithFade(string sceneName)
    {
        fadeController.RegisterCallback(() =>
        {
            SceneManager.LoadScene(sceneName); // 페이드 아웃 끝나고 씬 전환
        });

        fadeController.FadeOut(); // 먼저 페이드 아웃
    }
}
