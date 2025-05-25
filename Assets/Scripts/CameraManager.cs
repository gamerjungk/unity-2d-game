using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
// 해상도 고정 카메라 스크립트
public class CameraResolution : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 30, 0);
    void Start()
    {
    }

    void FixedUpdate()
    {
        Vector3 targetPos = target.position + offset;
        transform.rotation = Quaternion.Euler(90f, target.eulerAngles.y, 0f);
        transform.position = Vector3.Lerp(transform.position, targetPos, 0.2f);

    }

    void OnPreCull() => GL.Clear(true, true, Color.black);
}