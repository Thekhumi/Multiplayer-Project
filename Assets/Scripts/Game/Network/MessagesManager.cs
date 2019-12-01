using UnityEngine;

public class MessagesManager : Singleton<MessagesManager>
{
    protected override void Initialize()
    {
        base.Initialize();
    }

    public void SendString(string message, uint objectId)
    {
        Debug.Log($"Sending: " + message);

        StringMessagePacket packet = new StringMessagePacket();

        packet.payload = message;

        PacketsManager.Instance.SendPacket(packet, objectId);
    }
}
