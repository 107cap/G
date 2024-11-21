using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WallType
{
    Basic,
    Reflection
}

public enum Pos
{
    Left,
    Right, 
    Top, 
    Bottom
}

public class Wall : MonoBehaviour
{
    public WallType wallType;
    public Pos posType;
    public float refForce;

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

        return pos*refForce;
    }
}
