using System.IO;

public struct ConnectionRequestData
{
    public long clientSalt;
}

public class ConnectionRequestPacket : NetworkPacket<ConnectionRequestData>
{
    public ConnectionRequestPacket() : base(PacketType.ConnectionRequest, 0)
    {
    }

    protected override void OnDeserialize(Stream stream)
    {
        BinaryReader br = new BinaryReader(stream);
        payload.clientSalt = br.ReadInt64();
    }

    protected override void OnSerialize(Stream stream)
    {
        BinaryWriter bw = new BinaryWriter(stream);
        bw.Write(payload.clientSalt);
    }
}