using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventPacket : IPacket
{
    public PacketType Type { get => PacketType.EVENT; }
    public int clientNum;
    public int ClientNum
    {
        get => clientNum;
        set => clientNum = value;
    }

    public EventType eventType;

    public void EventType(EventType _eventType)
    {
        eventType = _eventType; 
    }
}
    //T IPacket.GetValue<T>()
    //{
    //    throw new System.NotImplementedException();
    //}

    //void IPacket.SetValue<T>(T value)
    //{
    //    throw new System.NotImplementedException();
    //}
