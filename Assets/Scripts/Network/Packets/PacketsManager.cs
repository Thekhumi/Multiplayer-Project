using System.IO;
using System.Net;
using System.Collections.Generic;
using UnityEngine;

public class PacketsManager : Singleton<PacketsManager>, IReceiveData
{
    private Dictionary<uint, System.Action<uint, ushort, Stream>> onPacketReceived = new Dictionary<uint, System.Action<uint, ushort, Stream>>();
    private uint currentPacketId = 0;

    protected override void Initialize()
    {
        base.Initialize();
    }
    
    public void Start()
    {
        NetworkManager.Instance.onReceiveEvent += OnReceiveData;
    }

    public void AddListener(uint ownerId, System.Action<uint, ushort, Stream> callback)
    {
        if (!onPacketReceived.ContainsKey(ownerId))
            onPacketReceived.Add(ownerId, callback);
    }

    public void RemoveListener(uint ownerId)
    {
        if (onPacketReceived.ContainsKey(ownerId))
            onPacketReceived.Remove(ownerId);
    }

    public void SendPacket(ISerializablePacket packet, uint objectId, bool reliable = false)
    {
        byte[] bytes = Serialize(packet, objectId);

        Debug.Log($"Sending bytes count: " + bytes.Length);

        if (NetworkManager.Instance.isServer)
            Broadcast(bytes);
        else
            NetworkManager.Instance.SendToServer(bytes);
    }

    private void Broadcast(byte[] bytes)
    {
        Debug.Log($"Broadcasting to: {ConnectionManager.Instance.clients.Count}");
        using (var iterator = ConnectionManager.Instance.clients.GetEnumerator())
        {
            while (iterator.MoveNext())
            {
                NetworkManager.Instance.SendToClient(bytes, iterator.Current.Value.ipEndPoint);
            }
        }
    }

    private byte[] Serialize(ISerializablePacket packet, uint objectId)
    {
        PacketHeader header = new PacketHeader();
        MemoryStream stream = new MemoryStream();

        header.id = currentPacketId++;
        header.packetType = packet.packetType;
        header.senderId = ConnectionManager.Instance.clientId;
        header.objectId = objectId;
        header.userPacketType = packet.userPacketType;

        header.Serialize(stream);
        packet.Serialize(stream);

        stream.Close();

        return stream.ToArray();
    }

    public void OnReceiveData(byte[] data, IPEndPoint ipEndpoint)
    {
        Debug.Log($"Receiving: " + data);

        PacketHeader header = new PacketHeader();
        MemoryStream stream = new MemoryStream(data);

        header.Deserialize(stream);

        InvokeCallback(header.objectId, header.id, header.userPacketType, stream);

        stream.Close();
    }

    void InvokeCallback(uint objectId, uint id, ushort type, Stream stream)
    {
        if (onPacketReceived.ContainsKey(objectId))
            onPacketReceived[objectId].Invoke(id, type, stream);
    }
}
