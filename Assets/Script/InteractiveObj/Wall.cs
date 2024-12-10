using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum WallType
{
    Basic,
    Reflection,
    Red,
    Blue,
    Green,
    Yellow,
    Purple
}

public enum Pos
{
    Left,
    Right, 
    Top, 
    Bottom,
    Basic
}

public class Wall : MonoBehaviour
{
    MeshRenderer m_MeshRenderer;
    Material m_Material;

    public WallType wallType;
    public Pos posType;

    public float refForce = 10;
    public float rotationSpeed;

    public bool isTurn;

    void Awake()
    {
        m_MeshRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        m_Material = m_MeshRenderer.material;

        switch (wallType)
        {
            case WallType.Basic:
                m_Material.color = new Color32(224, 211, 136, 255);
                break;
            case WallType.Reflection:
                m_Material.color = new Color32(214, 157, 214, 255);
                break;
            case WallType.Red:
                m_Material.color = new Color32(230, 160, 160, 255);
                break;
            case WallType.Blue:
                m_Material.color = new Color32(160, 160, 230, 255);
                break;
            case WallType.Green:
                m_Material.color = new Color32(160, 230, 160, 255);
                break;
            case WallType.Yellow:
                m_Material.color = new Color32(230, 230,160, 255);
                break;
            case WallType.Purple:
                m_Material.color = new Color32(230, 160, 230, 255);
                break;
        }
    }

    void Update()
    {
        TurnWall();
    }

    public Vector3 ReflectionPos()
    {
        Vector3 pos=Vector3.zero;

        switch (posType)
        {
            case Pos.Left:
                pos = Vector3.right;
                break;
            case Pos.Right:
                pos = Vector3.left;
                break;
            case Pos.Top:
                pos = Vector3.back;
                break;
            case Pos.Bottom:
                pos = Vector3.forward;
                break;
        }

        return pos * refForce;
    }

    void TurnWall()
    {
        if (isTurn)
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }
    }
}
