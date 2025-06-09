using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UIManager : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Sprite[] stickSprites;
    public Image[] uiImages;          // ui 객체 배열
    public Text turnText;             // 현재 몇턴인지 보여주는 텍스트
    public float maxSteerAngle = 180f, wheelAngle = 0f, lastWheelAngle = 0f, pressTime = 0f; // 최대 회전 각도 , 현재 핸들 각도
    public int gearState = 4;       // 1: P     2: R    3: N    4: D
    private bool isAccel = false, isBrake = false, isHandling = false, isGear = false;
    public float wheelDelta = 0f;
    public float prevWheelAngle = 0f;  
    void Start()
    {
        uiImages = GetComponentsInChildren<Image>();    // 1: accel    2: brake    3: handle   4: gear  5: gearStick     6: fuel     7: fuelStick     8: turnBar
    }

    void Update()
    {
        
        turnText.text = string.Format("cur\nTurn: " + GameManager.inst.turnManager.curTurn);
        uiImages[3].rectTransform.localEulerAngles = new Vector3(0, 0, wheelAngle);  
        
        if (isHandling) //이제 이전 턴 핸들의 앵글을 저장하고 현재 각도와 비교 연산
        {
            
            wheelDelta = wheelAngle - prevWheelAngle;
            GameManager.inst.player.Handling(wheelDelta); //변화량만큼 Angle을 이동
            prevWheelAngle = wheelAngle; //매턴이 끝날 때는 핸들링 함수가 마지막으로 호출된 각도를 유지함
        }
        // 이전 내용(핸들링을 할 때 현재 핸들의 Angle을 가져오기)
        // if (isHandling)
        // {
        //     GameManager.inst.player.Handling(wheelAngle);
        // }
        uiImages[7].rectTransform.rotation = Quaternion.Euler(0, 0, 70f - 2 * GameManager.fuel);
        if(isAccel) {
            pressTime += Time.deltaTime;
            if (gearState == 4) GameManager.inst.player.Accelerate(pressTime);
            else if (gearState == 3) GameManager.inst.player.Accelerate(0);
            else if (gearState == 2) GameManager.inst.player.Accelerate(-pressTime);
            else if (gearState == 1) GameManager.inst.player.Accelerate(0);
        } else if(isBrake) {
            pressTime += Time.deltaTime;
            GameManager.inst.player.Brake();
        }
        if (!isHandling && GameManager.inst.turnManager.isMidTurn)
        {
            // 터치에서 손을 뗀 후 핸들 복귀 현재 핸들 각에 따라 복귀 속도 변화
            wheelAngle = Mathf.Lerp(wheelAngle, 0f, Time.deltaTime * Mathf.Clamp(Mathf.Abs(wheelAngle), 0.05f, 0.5f));
            uiImages[3].rectTransform.localEulerAngles = new Vector3(0, 0, wheelAngle);
        }
        if (isGear)         // 기어 조작
        {
            uiImages[4].transform.localScale = new Vector3(1, 1, 1);
            uiImages[5].transform.localScale = new Vector3(1, 1, 1);
            if (uiImages[5].transform.position.y >= 170f)
            {
                uiImages[5].sprite = stickSprites[0];
                gearState = 1;
            }
            else if (uiImages[5].transform.position.y >= 160f)
            {
                uiImages[5].sprite = stickSprites[1];
                gearState = 2;
            }
            else if (uiImages[5].transform.position.y >= 150f)
            {
                uiImages[5].sprite = stickSprites[2];
                gearState = 3;
            }
            else if (uiImages[5].transform.position.y < 150f)
            {
                uiImages[5].sprite = stickSprites[3];
                gearState = 4;
            }
        }
        else
        {
            uiImages[4].transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            uiImages[5].transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            if (gearState == 1) uiImages[5].rectTransform.position = new Vector3(uiImages[5].rectTransform.position.x, 170f, 0);
            else if (gearState == 2) uiImages[5].rectTransform.position = uiImages[5].rectTransform.position = new Vector3(uiImages[5].rectTransform.position.x, 160f, 0);
            else if (gearState == 3) uiImages[5].rectTransform.position = uiImages[5].rectTransform.position = new Vector3(uiImages[5].rectTransform.position.x, 150f, 0);
            else if (gearState == 4) uiImages[5].rectTransform.position = uiImages[5].rectTransform.position = new Vector3(uiImages[5].rectTransform.position.x, 140f, 0);
        }
    }

    public void OnPointerDown(PointerEventData eventData)   // 터치 누를 때 작동하는 함수
    {
        if (IsPointerOn(eventData, uiImages[1]))
        {
            isAccel = true;
            pressTime = 0f;
            GameManager.inst.turnManager.midTurn();
        }
        if (IsPointerOn(eventData, uiImages[2]))
        {
            isBrake = true;
            pressTime = 0f;
            GameManager.inst.turnManager.midTurn();
        }
        if (IsPointerOn(eventData, uiImages[3]))
        {
            isHandling = true;
            lastWheelAngle = GetAngle(eventData.position);
            prevWheelAngle = wheelAngle;
            GameManager.inst.turnManager.midTurn();
        }
        if (IsPointerOn(eventData, uiImages[4]))
        {
            isGear = true;
        }
    }

    public void OnDrag(PointerEventData eventData)       // 터치해서 드래그할 때 작동하는 함수 현재는 핸들 ui를 드래그하면 그에 맞게 핸들이랑 player 회전
    {
        if(IsPointerOn(eventData, uiImages[3])) {
        float newAngle = GetAngle(eventData.position);   
        float deltaAngle = Mathf.DeltaAngle(lastWheelAngle, newAngle);

        wheelAngle = Mathf.Clamp(wheelAngle + deltaAngle, -maxSteerAngle, maxSteerAngle);
        uiImages[3].rectTransform.localEulerAngles = new Vector3(0, 0, wheelAngle);                 // 핸들 회전
        lastWheelAngle = newAngle;
        } 
        if(IsPointerOn(eventData, uiImages[5]))
        {
            uiImages[5].rectTransform.position = new Vector3(uiImages[5].rectTransform.position.x, Mathf.Clamp(eventData.position.y, 145f, 175f), 0);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHandling = false;
        isGear = false;
        if(IsPointerOn(eventData, uiImages[1])) {
            isAccel = false;
        }
        if(IsPointerOn(eventData, uiImages[2])) {
            isBrake = false;
        }
    }

    private float GetAngle(Vector2 touchPos)        // 터치한 곳과 핸들사이 각도 계산
    {
        Vector2 dir = touchPos - (Vector2)uiImages[3].rectTransform.position;   
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

        private bool IsPointerOn(PointerEventData eventData, Image targetUI)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(targetUI.rectTransform, eventData.position);
    }
}
