using UnityEngine.UI;
using System.IO;
using UnityEngine;

public class ChatScreen : MonoBehaviourSingleton<ChatScreen>
{
    public Text messages;
    public InputField inputMessage;

    protected override void Initialize()
    {
        base.Initialize();
        this.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        PacketsManager.Instance.AddListener(0, OnReceivePacket);
    }

    void OnDisable()
    {
        PacketsManager.Instance.RemoveListener(0);
    }

    void OnReceivePacket(uint id, ushort type, Stream stream)
    {
        Debug.Log($"OnReceivePacket: " + type);

        if (type == (ushort)GamePacketType.Message)
        {
            StringMessagePacket packet = new StringMessagePacket();
            packet.Deserialize(stream);

            Debug.Log($"Payload: " + packet.payload);

            if (NetworkManager.Instance.isServer)
            {
                MessagesManager.Instance.SendString(packet.payload, 0);
            }

            messages.text += packet.payload + System.Environment.NewLine;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (inputMessage && inputMessage.text != "")
            {
                if (NetworkManager.IsAvailable() && NetworkManager.Instance.isServer)
                {
                    messages.text += inputMessage.text + System.Environment.NewLine;
                }

                MessagesManager.Instance.SendString(inputMessage.text, 0);

                inputMessage.ActivateInputField();
                inputMessage.Select();
                inputMessage.text = "";
            }
        }
    }

}
