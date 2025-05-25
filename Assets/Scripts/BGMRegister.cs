using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMRegister : MonoBehaviour
{
    public float skipSeconds = 0f;
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

        Debug.Log($"🎧 BGMRegister: audioSource.clip = {(audioSource.clip != null ? audioSource.clip.name : "null")}");
        Debug.Log($"🎧 skipSeconds = {skipSeconds}");

        if (skipSeconds > 0f && audioSource.clip != null)
        {
            Debug.Log($"🎵 스킵 적용: {skipSeconds}초");
            audioSource.Stop(); // 강제로 초기화
            audioSource.time = Mathf.Min(skipSeconds, audioSource.clip.length - 0.01f);
        }

        audioSource.Play();
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
