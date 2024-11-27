using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;
using static TCPFileTransferManager;
using System;

public class TCPServerManager : Singleton<TCPServerManager>
{
    private TcpListener listener;
    private Thread listenerThread;
    private bool isRunning = false;
    
    // Configurações do servidor
    public string ipAddress = "127.0.0.1"; // IP do servidor
    public int port = 12345; // Porta do servidor

    public string result = "";

    private SendFileResult _sendFileStatus = SendFileResult.Inative;

    public SendFileResult SendFileStatus => _sendFileStatus;

    public Action<bool> OnServerStatusChange;

    // Inicia o servidor TCP
    public void StartServer()
    {
        _sendFileStatus = SendFileResult.Processing;
        listenerThread = new Thread(new ThreadStart(RunServer));
        listenerThread.IsBackground = true; // Torna a thread em segundo plano
        listenerThread.Start();
        Debug.Log("Servidor iniciado.");

        IsRunning = true;

    }
    
    // Executa o servidor em uma thread separada
    private void RunServer()
    {
        Debug.Log($"Iniciando servidor em {ipAddress}:{port}");
        listener = new TcpListener(IPAddress.Parse(ipAddress), port);
        listener.Start();
        Debug.Log($"Servidor ouvindo em {ipAddress}:{port}...");

        while (IsRunning)
        {
            try
            {
                TcpClient client = listener.AcceptTcpClient();
                Debug.Log("Conexão estabelecida com um cliente.");
                HandleClient(client);
            }
            catch (SocketException e)
            {
                Debug.LogError($"Erro ao aceitar conexão: {e.Message}");
                _sendFileStatus = SendFileResult.Failure;
            }
        }
    }
    
    // Lida com o cliente conectado
    private void HandleClient(TcpClient client)
    {
        using (client)
        using (NetworkStream stream = client.GetStream())
        {
            // Receber o nome do arquivo

            byte[] contentBuffer = new byte[16384];
            int bytesRead = stream.Read(contentBuffer, 0, contentBuffer.Length);
            result = Encoding.UTF8.GetString(contentBuffer, 0, bytesRead);

            Debug.Log($"Mensagem recebida na rede: \n{result}");

            _sendFileStatus = SendFileResult.Success;
        }
    }

    private void OnApplicationQuit()
    {
        StopServer();
    }

    // Para parar o servidor
    public void StopServer()
    {
        IsRunning = false;
        OnServerStatusChange?.Invoke(isRunning);
        listener.Stop();
        listenerThread.Abort();
        Debug.Log("Servidor encerrado.");
    }

    public void SetServerInfo(string ip, int port)
    {
        this.ipAddress = ip;
        this.port = port;
    }

    public void Release()
    {
        _sendFileStatus = SendFileResult.Inative;
    }

    public void ResultReaded() => _sendFileStatus = SendFileResult.Processing;
    public bool IsRunning { get => isRunning; private set { isRunning = value; OnServerStatusChange?.Invoke(isRunning); } }
}
