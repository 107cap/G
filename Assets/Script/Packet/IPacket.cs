
public interface IPacket
{
    public PacketType Type { get; }
    public int ClientNum { get; set; }
    //public T GetValue<T>();
    //public void SetValue<T>(T value);
}
