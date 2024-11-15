using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPacket : Packet<Vector3> 
{
    public PlayerPacket()
    {
        type = PacketType.PLAYER;
        SetValue(Vector3.zero);
    }
}
