using System.IO;

public interface ISerializablePacket
{
    PacketType packetType { get; set; }
    ushort userPacketType { get; set; }

    void Serialize(Stream stream);
    void Deserialize(Stream stream);
}