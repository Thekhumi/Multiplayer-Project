using System.IO;

public enum GamePacketType
{
    Message,
}

public abstract class GameNetworkPacket<T> : NetworkPacket<T>
{
    public GameNetworkPacket(GamePacketType type) : base(PacketType.User, (ushort)type)
    {
    }
}

public class StringMessagePacket : GameNetworkPacket<string>
{
    public StringMessagePacket() : base(GamePacketType.Message)
    {
    }

    protected override void OnDeserialize(Stream stream)
    {
        BinaryReader br = new BinaryReader(stream);
        payload = br.ReadString();
    }

    protected override void OnSerialize(Stream stream)
    {
        BinaryWriter bw = new BinaryWriter(stream);
        bw.Write(payload);
    }
}