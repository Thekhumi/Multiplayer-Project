using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System;

public class NetworkManager : MonoBehaviourSingleton<NetworkManager>, IReceiveData
{
    public IPAddress ipAddress
    {
        get; private set;
    }

    public int port
    {
        get; private set;
    }

    public bool isServer
    {
        get; private set;
    }

    public int timeOut = 30;

    public Action<byte[], IPEndPoint> onReceiveEvent;

    private UdpConnection connection;

    public bool StartServer(int port)
    {
        try
        {
            isServer = true;
            this.port = port;
            connection = new UdpConnection(port, this);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }

        return true;
    }

    public bool StartClient(IPAddress ip, int port)
    {
        try
        {
            isServer = false;

            this.port = port;
            this.ipAddress = ip;

            connection = new UdpConnection(ip, port, this);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }

        return true;
    }

    public void OnReceiveData(byte[] data, IPEndPoint ip)
    {
        if (onReceiveEvent != null)
            onReceiveEvent.Invoke(data, ip);
    }

    public void SendToServer(byte[] data)
    {
        Debug.Log("Sending to server");
        connection.Send(data);
    }

    public void SendToClient(byte[] data, IPEndPoint iPEndPoint)
    {
        Debug.Log("Sending to client");
        connection.Send(data, iPEndPoint);
    }

    void Update()
    {
        // We flush the data in main thread
        if (connection != null)
            connection.FlushReceiveData();
    }
}
