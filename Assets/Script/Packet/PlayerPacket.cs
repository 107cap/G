using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPacket : Packet<(float, float, float)> 
{
    public PlayerPacket()
    {
        type = PacketType.PLAYER;
        SetValue((0, 0, 0));
    }
}
