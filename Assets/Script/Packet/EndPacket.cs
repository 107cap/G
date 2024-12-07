using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPacket : IPacket
{
    public PacketType Type { get => PacketType.END; }
    public int clientNum;

    public int ClientNum
    {
        get => clientNum;
        set => clientNum = value;
    }

    public bool isEnd = false;
    public void SetisEnd(bool _isEnd) => isEnd = _isEnd;
    public EndPacket() { }

}
