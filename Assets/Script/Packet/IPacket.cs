using System;
public enum PacketType
{
    None=0,
    Send=1,
    Receive=2,
    Error=3
}

public interface IPacket
{
    PacketType type { get; }
    string Serialize();
    void DeSerialize(string data);
}