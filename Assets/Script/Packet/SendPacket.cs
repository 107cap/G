using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendPacket : IPacket
{
    public PacketType type=>PacketType.Send;            // 패킷 타입 초기화
    public MoveDirFlags moveFlag;                       // MoveDirFlags 변수

    // 패킷 생성
    public SendPacket(MoveDirFlags moveDirFlags)
    {
        this.moveFlag = moveDirFlags;
    }

    // 패킷 문자열로 직렬화
    public string Serialize()
    {
        return $"{ (int)moveFlag}";
    }

    // 패킷 역직렬화
    public void DeSerialize(string data)
    {
        var parts = data.Split(',');
        moveFlag = (MoveDirFlags)int.Parse(parts[0]);
    }
}
