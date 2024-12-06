using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpeedType
{
    Up,
    Down
}

public class SpeedObject : MonoBehaviour
{
    MeshRenderer m_render;
    Material m_material;

    public SpeedType speedType;
    public float force;

    void Awake()
    {
        m_render = GetComponent<MeshRenderer>();
        m_material = m_render.material;
    }

    void Start()
    {
        ColorSetting();
    }

    public float SpeedControl(float curSpeed)
    {
        float upSpeed=curSpeed;

        upSpeed *= force;

        return upSpeed;
    }

    void ColorSetting()
    {
        Color curColor = m_material.color;

        if (speedType == SpeedType.Up)
        {
            curColor = Color.blue;
        }
        else if (speedType == SpeedType.Down)
        {
            curColor = Color.red;
        }

        curColor.a = 0.3f;

        m_material.color = curColor;
    }
}
