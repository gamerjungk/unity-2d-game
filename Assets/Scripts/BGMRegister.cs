using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMRegister : MonoBehaviour
{
    private AudioSource audioSource;

    void Awake()
    {
        // 오브젝트가 씬 전환 때마다 살아있으면 안 되므로 DontDestroyOnLoad 안 씀
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 반복 재생 설정
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void Start()
    {
        if (GameSettingsManager.Instance != null)
        {
            GameSettingsManager.Instance.SetBGMSource(audioSource);
        }

        audioSource.Play(); // 씬 진입 시 재생
    }

    void OnSceneUnloaded(Scene scene)
    {
        Destroy(gameObject); // 씬이 전환되면 자기 자신 삭제 → BGM 종료
    }

    void OnDestroy()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }
}
