using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
// 해상도 고정 카메라 스크립트
public class CameraResolution : MonoBehaviour
{
  public Transform target;
    void Start()
  {
    Camera camera = GetComponent<Camera>();
    Rect rect = camera.rect;
    float scaleheight = ((float)Screen.width / Screen.height) / ((float)9 / 16);
    float scalewidth = 1f / scaleheight;
 
    if (scaleheight < 1)
    {
      rect.height = scaleheight;
      rect.y = (1f - scaleheight) / 2f;
    }
    else
    {
      rect.width = scalewidth;
      rect.x = (1f - scalewidth) / 2f;
    }
    camera.rect = rect;
  }

    void FixedUpdate()
    {
        Vector3 targetPos = target.position + new Vector3(0, 2, -10);
        transform.position = Vector3.Lerp(transform.position, targetPos, 0.2f);
    }

    void OnPreCull() => GL.Clear(true, true, Color.black);
}