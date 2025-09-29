using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq; // 🔸 LINQ 사용 위해 필요

public class EscapeButton : MonoBehaviour
{
    [Header("▶ 플레이어 참조")]
    public Transform playerTransform;
    public Rigidbody playerRigidbody;

    [Header("▶ 초기 위치 설정")]
    public Vector3 defaultPosition = new Vector3(-362.43f, 0.09770536f, -357.1601f);
    public Vector3 defaultRotation = Vector3.zero;

    [Header("▶ UI 버튼 참조")]
    public Button resetButton;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(DelayedFind());
    }

    private IEnumerator DelayedFind()
    {
        yield return new WaitForSeconds(0.1f); // UI가 생성될 시간을 확보

        TryFindReferences();

        if (resetButton != null)
        {
            resetButton.onClick.RemoveAllListeners(); // 중복 방지
            resetButton.onClick.AddListener(ResetPlayerPosition);
        }
        else
        {
            Debug.LogError("❌ resetButton(EscapeButton) 참조 실패!");
        }
    }

    public void ResetPlayerPosition()
    {
        if (playerTransform == null) return;

        playerTransform.position = defaultPosition;
        playerTransform.rotation = Quaternion.Euler(defaultRotation);

        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;

            playerRigidbody.isKinematic = true;
            playerRigidbody.Sleep();
            playerRigidbody.WakeUp();
            playerRigidbody.isKinematic = false;
        }

        Debug.Log($"[Reset] Player position reset to {defaultPosition}");
    }

    private void TryFindReferences()
    {
        // 🔹 비활성화 포함 EscapeButton 찾기
        if (resetButton == null)
        {
            resetButton = Resources.FindObjectsOfTypeAll<Button>()
                .FirstOrDefault(btn => btn.name == "EscapeButton");

            if (resetButton == null)
                Debug.LogWarning("⚠️ EscapeButton(Button) 오브젝트를 찾을 수 없습니다. 이름 확인 필요!");
        }

        if (playerTransform == null || playerRigidbody == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
                playerRigidbody = playerObj.GetComponent<Rigidbody>();
            }
            else
            {
                Debug.LogWarning("⚠️ Player 태그를 가진 오브젝트를 찾을 수 없습니다.");
            }
        }
    }
}
