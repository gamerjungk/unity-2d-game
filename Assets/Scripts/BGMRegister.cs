using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMRegister : MonoBehaviour
{
    public float skipSeconds = 0f;  // BGM 재생 시 처음 몇 초를 건너뛸지 설정하는 값
    private AudioSource audioSource;    // BGM 재생에 사용할 AudioSource 컴포넌트 참조
    
    void Awake()
    {
        // 오브젝트가 씬 전환 때마다 살아있으면 안 되므로 DontDestroyOnLoad 안 씀
        audioSource = GetComponent<AudioSource>();  // 현재 오브젝트에 AudioSource가 있는지 확인
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();   // 없으면 새로 추가
        }

        // 반복 재생 설정
        audioSource.loop = true;    // 배경음은 반복 재생되도록 설정
        audioSource.playOnAwake = false;    // 자동 재생 방지 (Start에서 직접 실행)

        SceneManager.sceneUnloaded += OnSceneUnloaded;  // 씬이 언로드될 때 실행할 콜백 등록
    }

    void Start()
    {   
        // GameSettingsManager에 현재 AudioSource를 등록 (볼륨 등 설정을 위함)
        if (GameSettingsManager.Instance != null)
        {
            GameSettingsManager.Instance.SetBGMSource(audioSource);
        }

        // 현재 오디오 클립 정보 디버그 출력(디버깅용)
        Debug.Log($"🎧 BGMRegister: audioSource.clip = {(audioSource.clip != null ? audioSource.clip.name : "null")}");
        Debug.Log($"🎧 skipSeconds = {skipSeconds}");

        
        if (skipSeconds > 0f && audioSource.clip != null)
        {
            Debug.Log($"🎵 스킵 적용: {skipSeconds}초");
            audioSource.Stop(); // 재생 중이면 초기화
            audioSource.time = Mathf.Min(skipSeconds, audioSource.clip.length - 0.01f); // clip 길이를 초과하지 않도록 범위 제한 후 위치 설정
        }

        audioSource.Play(); // 최종적으로 BGM 재생 시작
    }

    void OnSceneUnloaded(Scene scene)
    {
        Destroy(gameObject); // 씬이 전환되면 자기 자신 삭제 → BGM 종료
    }

    void OnDestroy()
    {   // 씬 언로드 이벤트 등록 해제 → 메모리 누수 방지
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }
}
