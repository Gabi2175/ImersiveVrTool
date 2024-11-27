using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.Text;
using TMPro;

public class ServerUDPManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _txtLog;
    [SerializeField] private TMP_InputField _InputFieldIP;
    [SerializeField] private TMP_InputField _InputFieldPort;

    private Socket _socket;

    void Start()
    {
        ConnectedToServer();
    }

    public void SendMessageToServer(string message)
    {
        byte[] buffer = Encoding.ASCII.GetBytes(message);
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(_InputFieldIP.text), int.Parse(_InputFieldPort.text));
        _socket.SendTo(buffer, endPoint);
        _txtLog.text += $"\nSend message {message}";
    }

    public void DisconnectFromServer()
    {
        if (_socket == null) return;

        _socket.Disconnect(true);
        _txtLog.text += $"\nDisconnect from server";
    }

    public void ConnectedToServer()
    {
        try
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            _txtLog.text += $"\nConnected with {_InputFieldIP.text}:{_InputFieldPort.text}";
        }
        catch (Exception error)
        {
            Debug.Log(error);
            _txtLog.text += $"\nError while connecting to {_InputFieldIP.text}:{_InputFieldPort.text}";
            _txtLog.text += $"\n{error.StackTrace}";
        }
    }
}
