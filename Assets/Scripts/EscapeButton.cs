using UnityEngine;
using UnityEngine.UI;

public class EscapeButton : MonoBehaviour
{
    [Header("▶ 플레이어 참조")]
    public Transform playerTransform;
    // 차량이면 Rigidbody도 리셋해주는 게 좋습니다
    public Rigidbody playerRigidbody;

    [Header("▶ 초기 위치 설정")]
    public Vector3 defaultPosition = new Vector3(-362.43f, 0.09770536f, -357.1601f);
    public Vector3 defaultRotation = Vector3.zero; // 회전까지 초기화하고 싶으면 설정

    [Header("▶ UI 버튼 참조")]
    public Button resetButton;

    private void Start()
    {
        if (resetButton != null)
            resetButton.onClick.AddListener(ResetPlayerPosition);
    }
    public void ResetPlayerPosition()
    {
        if (playerTransform == null) return;

        // 위치 및 회전 초기화
        playerTransform.position = defaultPosition;
        playerTransform.rotation = Quaternion.Euler(defaultRotation);

        if (playerRigidbody != null)
        {
            // 🔧 물리 충돌 방지용: 리셋 후 안정화
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;

            // 🔧 물리엔진 간섭 제거 → 다시 활성화
            playerRigidbody.isKinematic = true;
            playerRigidbody.Sleep(); // 완전히 정지
            playerRigidbody.WakeUp(); // 다시 깨움
            playerRigidbody.isKinematic = false;
        }

        Debug.Log($"[Reset] Player position reset to {defaultPosition}");
    }

}
