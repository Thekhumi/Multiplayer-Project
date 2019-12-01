using System.Collections.Generic;
using System.Net;
using UnityEngine;
using System.IO;

public struct Client
{
    public enum State
    {
        PendingConnection,
        Connected
    }

    public float lastMsgTimeStamp;
    public long id;
    public IPEndPoint ipEndPoint;

    public Client(IPEndPoint ipEndPoint, long id, float timeStamp)
    {
        this.lastMsgTimeStamp = timeStamp;
        this.id = id;
        this.ipEndPoint = ipEndPoint;
    }
}

public class ConnectionManager : Singleton<ConnectionManager>
{
    public readonly Dictionary<long, Client> clients = new Dictionary<long, Client>();
    private readonly Dictionary<IPEndPoint, long> ipToId = new Dictionary<IPEndPoint, long>();
    private System.Action<bool> onConnect;
    private System.Random rand = new System.Random();

    public enum State
    {
        Disconnected,
        Connecting,
        Connected
    }
    
    public State state
    {
        get; private set;
    }

    // This id should be generated during first handshake
    public long clientId
    {
        get; private set;
    }

    protected override void Initialize()
    {
        base.Initialize();
        state = State.Disconnected;
        NetworkManager.Instance.onReceiveEvent += OnReceiveData;
    }

    public void ConnectToServer(IPAddress ip, int port, System.Action<bool> onConnectCallback)
    {
        if (!NetworkManager.Instance.StartClient(ip, port))
        {
            if (onConnectCallback != null)
                onConnectCallback(false);

            return;
        }

        if (onConnectCallback != null)
            onConnect += onConnectCallback;
        
        state = State.Connecting;

        SendConnectionRequest();
    }

    public bool StartServer(int port)
    {
        return NetworkManager.Instance.StartServer(port);
    }

    void AddClient(IPEndPoint ip)
    {
        if (!ipToId.ContainsKey(ip))
        {
            Debug.Log("Adding client: " + ip.Address);

            long id = clientId;
            ipToId[ip] = clientId;

            clients.Add(clientId, new Client(ip, id, Time.realtimeSinceStartup));

            clientId++;
        }
    }

    void RemoveClient(IPEndPoint ip)
    {
        if (ipToId.ContainsKey(ip))
        {
            Debug.Log("Removing client: " + ip.Address);
            clients.Remove(ipToId[ip]);
        }
    }

    private void SendToServer<T>(NetworkPacket<T> packet)
    {
        MemoryStream stream = new MemoryStream();
        BasePacketHeader header = new BasePacketHeader();
        header.protocolId = 0;
        header.packetType = packet.packetType;
        
        header.Serialize(stream);
        packet.Serialize(stream);
        
        stream.Close();
        NetworkManager.Instance.SendToServer(stream.ToArray());
    }

    private void SendConnectionRequest()
    {
        ConnectionRequestPacket request = new ConnectionRequestPacket();
        request.payload.clientSalt = LongRandom(0, long.MaxValue);
        SendToServer(request);
    }

    private void SendChallengeRequest()
    {
         
    }

    private void SendChallengeResponse()
    {
        
    }

    private void OnReceiveData(byte[] bytes, IPEndPoint ipEndpoint)
    {

    }

    long LongRandom(long min, long max) 
    {
        byte[] buf = new byte[8];
        rand.NextBytes(buf);
        long longRand = System.BitConverter.ToInt64(buf, 0);

        return (System.Math.Abs(longRand % (max - min)) + min);
    }
}