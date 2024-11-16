using System;
using UnityEngine;

public class PlayerPacket : IPacket 
{
    public PacketType type { get => PacketType.PLAYER; }
    float x;
    float y;
    float z;

    public (float, float, float) GetPosition() => (x, y, z);
    public Vector3 GetPosition2Vec3() => new Vector3(x, y, z);

    public void SetPosition((float, float, float) value)
    {
        (x, y, z) = value;
    }

    public void SetPosition(Vector3 value)
    {
        (x, y, z) = (value.x, value.y, value.z);
    }
}
