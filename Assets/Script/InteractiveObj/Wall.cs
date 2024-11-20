using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Pos
{
    Left,
    Right, 
    Top, 
    Bottom
}

public class Wall : MonoBehaviour
{
    public Pos posType;
}
