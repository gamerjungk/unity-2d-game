using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UIManager : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Image[] uiImages;          // ui 객체 배열
    public Text turnText;             // 현재 몇턴인지 보여주는 텍스트
    public float maxSteerAngle = 180f, wheelAngle = 0f, lastWheelAngle = 0f; // 최대 회전 각도 , 현재 핸들 각도
    public float pressTime = 0f;
    private bool isAccel = false, isBrake = false, isDragging;

    void Start()
    {
        uiImages = GetComponentsInChildren<Image>();    // 0: accel    1: brake    2: handle   3: turnBar
    }

    void Update()
    {
        turnText.text = string.Format("cur\nTurn: " + GameManager.inst.turnManager.curTurn);
        uiImages[2].rectTransform.localEulerAngles = new Vector3(0, 0, wheelAngle);     // 실시간으로 핸들 회전
        GameManager.inst.player.Handling(wheelAngle);                                   
        if(isAccel) {
            pressTime += Time.deltaTime;
            GameManager.inst.player.Accelerate(pressTime);
        } else if(isBrake) {
            pressTime += Time.deltaTime;
            GameManager.inst.player.Brake();
        }
        if (!isDragging && GameManager.inst.turnManager.isMidTurn)
        {
            // 터치에서 손을 뗀 후 핸들 복귀 현재 핸들 각에 따라 복귀 속도 변화
            wheelAngle = Mathf.Lerp(wheelAngle, 0f, Time.deltaTime * Mathf.Clamp(Mathf.Abs(wheelAngle), 0.05f, 0.5f));
            uiImages[2].rectTransform.localEulerAngles = new Vector3(0, 0, wheelAngle);
        }
    }

    public void OnPointerDown(PointerEventData eventData)   // 터치 누를 때 작동하는 함수
    {
        if(IsPointerOn(eventData, uiImages[0])) {
            isAccel = true;
            pressTime = 0f;
            GameManager.inst.turnManager.midTurn();
        }
        if(IsPointerOn(eventData, uiImages[1])) {
            isBrake = true;
            pressTime = 0f;
            GameManager.inst.turnManager.midTurn();
        }
        if(IsPointerOn(eventData, uiImages[2])) {
            isDragging = true;
            lastWheelAngle = GetAngle(eventData.position);
        }
    }

    public void OnDrag(PointerEventData eventData)       // 터치해서 드래그할 때 작동하는 함수 현재는 핸들 ui를 드래그하면 그에 맞게 핸들이랑 player 회전
    {
        if(IsPointerOn(eventData, uiImages[2])) {
        float newAngle = GetAngle(eventData.position);   
        float deltaAngle = Mathf.DeltaAngle(lastWheelAngle, newAngle);
        wheelAngle = Mathf.Clamp(wheelAngle + deltaAngle, -maxSteerAngle, maxSteerAngle);
        uiImages[2].rectTransform.localEulerAngles = new Vector3(0, 0, wheelAngle);                 // 핸들 회전
        lastWheelAngle = newAngle;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        if(IsPointerOn(eventData, uiImages[0])) {
            isAccel = false;
        }
        if(IsPointerOn(eventData, uiImages[1])) {
            isBrake = false;
        }
    }

    private float GetAngle(Vector2 touchPos)        // 터치한 곳과 핸들사이 각도 계산
    {
        Vector2 dir = touchPos - (Vector2)uiImages[2].rectTransform.position;   
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

        private bool IsPointerOn(PointerEventData eventData, Image targetUI)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(targetUI.rectTransform, eventData.position);
    }
}
