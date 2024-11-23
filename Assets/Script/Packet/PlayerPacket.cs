using System;
using UnityEngine;

public class PlayerPacket : IPacket 
{
    public PacketType Type { get => PacketType.PLAYER; }
    public int clientNum;
    public int ClientNum
    {
        get => clientNum;
        set => clientNum = value;
    }
    public long timestamp;

    public float x;
    public float y;
    public float z;

    public PlayerPacket() { }
    
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
