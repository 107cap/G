using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Vector3 offset;                  // ī�޶� ��ġ

    // ���� ����
    public void LookAt(Transform target)
    {
        this.transform.position = target.position + offset;
    }
}
