using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Vector3 offset;                  // 카메라 위치

    // 시점 설정
    public void LookAt(Transform target)
    {
        this.transform.position = target.position + offset;
    }
}
