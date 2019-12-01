using System.IO;

public enum PacketType
{
    ConnectionRequest,
    Challenge,
    ChallengeResponse,
    User,
}

public class BasePacketHeader : ISerializablePacket
{
    public ushort protocolId;
    public PacketType packetType { get; set; }
    public ushort userPacketType { get; set; }

    public void Serialize(Stream stream)
    {
        BinaryWriter bw = new BinaryWriter(stream);

        bw.Write(protocolId);
        bw.Write((byte)packetType);
        bw.Write(userPacketType);

        OnSerialize(stream);
    }

    public void Deserialize(Stream stream)
    {
        BinaryReader br = new BinaryReader(stream);
        protocolId = br.ReadUInt16();
        packetType = (PacketType)br.ReadByte();
        userPacketType = br.ReadUInt16();
        
        OnDeserialize(stream);
    }

    
    protected virtual void OnSerialize(Stream stream)
    {
    }

    protected virtual void OnDeserialize(Stream stream)
    {
    }
}

public class PacketHeader : BasePacketHeader
{
    public long senderId;
    public uint id;
    public uint objectId;
    
    protected override void OnSerialize(Stream stream)
    {
        BinaryWriter bw = new BinaryWriter(stream);

        bw.Write(id);
        bw.Write(senderId);
        bw.Write(objectId);
    }

    protected override void OnDeserialize(Stream stream)
    {
        BinaryReader br = new BinaryReader(stream);
        id = br.ReadUInt32();
        senderId = br.ReadInt64();
        senderId = br.ReadUInt32();
    }

}

// P = Payload type
public abstract class NetworkPacket<P> : ISerializablePacket
{
    public ushort id;
    public PacketType packetType { get; set; }
    public ushort userPacketType { get; set; }
    public P payload;

    public NetworkPacket(PacketType packetType, ushort userType)
    {
        this.packetType = packetType;
        this.userPacketType = userType;
    }

    public virtual void Serialize(Stream stream)
    {
        OnSerialize(stream);
    }

    public virtual void Deserialize(Stream stream)
    {
        OnDeserialize(stream);
    }


    protected abstract void OnSerialize(Stream stream);
    protected abstract void OnDeserialize(Stream stream);
}
