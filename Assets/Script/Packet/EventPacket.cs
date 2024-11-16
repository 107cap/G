using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventPacket : IPacket
{
    public PacketType type { get => PacketType.EVENT; }

    //T IPacket.GetValue<T>()
    //{
    //    throw new System.NotImplementedException();
    //}

    //void IPacket.SetValue<T>(T value)
    //{
    //    throw new System.NotImplementedException();
    //}
}
