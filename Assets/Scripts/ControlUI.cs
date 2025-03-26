using UnityEngine;
using UnityEngine.EventSystems;

public class ControlUI : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform[] uiImages;
    public float maxSteerAngle = 50f; // 최대 회전 각도
    private float wheelAngle = 0f; // 현재 핸들 각도
    private float lastWheelAngle = 0f;
    void Start()
    {
        uiImages = GetComponentsInChildren<RectTransform>();    // 0: uiTab     1: accel    2: brake    3: handle
    }

    void Update()
    {
    }

    public void OnPointerDown(PointerEventData eventData)   // 터치 누를 때 작동하는 함수
    {
        lastWheelAngle = GetAngle(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)  // 터치해서 드래그할 때 작동하는 함수 현재는 핸들 ui를 드래그하면 그에 맞게 핸들이랑 player 회전
    {
        float newAngle = GetAngle(eventData.position);
        float deltaAngle = Mathf.DeltaAngle(lastWheelAngle, newAngle);
        wheelAngle = Mathf.Clamp(wheelAngle + deltaAngle, -maxSteerAngle, maxSteerAngle);
        uiImages[3].localEulerAngles = new Vector3(0, 0, wheelAngle);
        GameManager.inst.player.transform.localEulerAngles = new Vector3(0, 0, wheelAngle + 90);
        lastWheelAngle = newAngle;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }

    private float GetAngle(Vector2 touchPos)
    {
        Vector2 dir = touchPos - (Vector2)uiImages[3].position;
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

}
