using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
// 해상도 고정 카메라 스크립트
public class CameraResolution : MonoBehaviour
{
  void Start()
  {
    Camera camera = GetComponent<Camera>();
    Rect rect = camera.rect;
    float scaleheight = ((float)Screen.width / Screen.height) / ((float)9 / 16);
    float scalewidth = 1f / scaleheight;
 
    // 위 아래 공백 생성 (휴대폰이 날씬한 경우)
    if (scaleheight < 1)
    {
      rect.height = scaleheight;
      rect.y = (1f - scaleheight) / 2f;
    }
    // 좌 우 공백 생성 (휴대폰이 뚱뚱한 경우)
    else
    {
      rect.width = scalewidth;
      rect.x = (1f - scalewidth) / 2f;
    }
    camera.rect = rect;
  }
 
  void OnPreCull() => GL.Clear(true, true, Color.black);
}