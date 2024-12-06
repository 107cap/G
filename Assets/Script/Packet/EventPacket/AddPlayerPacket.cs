using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPlayerPacket : EventPacket
{
    public int[] ClientNums;
    public string nickName;
    public string[] ClientNames;
}
