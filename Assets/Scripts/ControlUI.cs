using UnityEngine;
using UnityEngine.EventSystems;

public class ControlUI : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform accel, brake, handle;
    void Start()
    {
        accel = GetComponent<RectTransform>()[0];
    }
    public float maxSteerAngle = 200f; // 최대 회전 각도
    public float returnSpeed = 5f; // 핸들 복귀 속도
    private float wheelAngle = 0f; // 현재 핸들 각도
    private float lastWheelAngle = 0f;
    private bool isDragging = false;

    void Update()
    {
        if (!isDragging)
        {
            // 터치에서 손을 뗀 후 핸들 복귀
            wheelAngle = Mathf.Lerp(wheelAngle, 0f, Time.deltaTime * returnSpeed);
            handle.localEulerAngles = new Vector3(0, 0, -wheelAngle);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        lastWheelAngle = GetAngle(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        float newAngle = GetAngle(eventData.position);
        float deltaAngle = Mathf.DeltaAngle(lastWheelAngle, newAngle);
        wheelAngle = Mathf.Clamp(wheelAngle + deltaAngle, -maxSteerAngle, maxSteerAngle);
        handle.localEulerAngles = new Vector3(0, 0, -wheelAngle);
        lastWheelAngle = newAngle;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }

    private float GetAngle(Vector2 touchPos)
    {
        Vector2 dir = touchPos - (Vector2)handle.position;
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

    public float GetSteerInput()
    {
        return wheelAngle / maxSteerAngle; // -1 ~ 1 값 반환
    }
}
