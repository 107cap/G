using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyPacket : IPacket
{
    public PacketType Type { get => PacketType.READY; }
    public int clientNum;
    public int ClientNum
    {
        get => clientNum;
        set => clientNum = value;
    }

    public bool isReady = false;
    public ReadyPacket() { }
    public bool GetIsReady() => isReady;
    public void SetIsReady(bool _isReady) => isReady = _isReady;
}
