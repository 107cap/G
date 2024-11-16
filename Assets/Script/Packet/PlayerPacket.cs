using System;
using UnityEngine;

public class PlayerPacket : IPacket 
{
    public PacketType type { get => PacketType.PLAYER; }
    float x;
    float y;
    float z;

    public (float, float, float) GetPosition() => (x, y, z);

    public void SetPosition((float, float, float) value)
    {
        (x, y, z) = value;
    }
}
