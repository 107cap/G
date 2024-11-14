using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceivePacket : IPacket
{
    public PacketType type => PacketType.Receive;       // ��Ŷ Ÿ�� �ʱ�ȭ
    public MoveDirFlags moveFlag;                       // MoveDirFlags ����

    // ��Ŷ ����
    public ReceivePacket(MoveDirFlags moveDirFlags)
    {
        this.moveFlag = moveDirFlags;
    }

    // ��Ŷ ���ڿ��� ����ȭ
    public string Serialize()
    {
        return $"{(int)moveFlag}";
    }

    // ��Ŷ ������ȭ
    public void DeSerialize(string data)
    {
        var parts = data.Split(',');
        moveFlag = (MoveDirFlags)int.Parse(parts[0]);
    }
}
