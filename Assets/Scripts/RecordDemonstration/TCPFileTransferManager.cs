using ExitGames.Client.Photon.StructWrapping;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TCPFileTransferManager : Singleton<TCPFileTransferManager>
{
    public enum SendFileResult { Inative, Processing, Success, Failure }

    private string serverIp = "localhost";
    private int port = 12345;

    private SendFileResult _sendFileStatus = SendFileResult.Inative;

    public SendFileResult SendFileStatus => _sendFileStatus;

    public async void SendFile(string filePath, string prefix="")
    {
        _sendFileStatus = SendFileResult.Processing;

        try
        {
            await Task.Run(() => SendFileAsync(filePath, prefix));
        }
        catch (Exception e)
        {
            Debug.LogError($"Erro: {e.Message}");
            _sendFileStatus = SendFileResult.Failure;
        }
    }

    public async void SendCommand(string command)
    {
        try
        {
            await Task.Run(() => SendCommandAsync(command));
        }
        catch (Exception e)
        {
            Debug.LogError($"Erro: {e.Message}");
        }
    }

    private async Task SendCommandAsync(string command)
    {
        using (TcpClient client = new TcpClient(serverIp, port))
        using (NetworkStream stream = client.GetStream())
        {
            Debug.Log($"=====> {command} {serverIp}:{port}");

            byte[] commandBytes = Encoding.UTF8.GetBytes(command);
            Debug.Log($"-> {commandBytes.Length} {command}");
            await stream.WriteAsync(commandBytes, 0, commandBytes.Length);
            Debug.Log($"Comando enviado: {command}");
        }
    }

    private async Task SendFileAsync(string filePath, string prefix="")
    {
        using (TcpClient client = new TcpClient(serverIp, port))
        using (NetworkStream stream = client.GetStream())
        {
            // Enviar o nome do arquivo
            string fileName = $"{prefix}{Path.GetFileName(filePath)}";
            byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);
            await stream.WriteAsync(fileNameBytes, 0, fileNameBytes.Length);
            Debug.Log($"Nome do arquivo enviado: {fileName}");

            await Task.Delay(500);

            // Enviar o conteúdo do arquivo
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[4096];
                int bytesRead;

                Debug.Log($"Enviando arquivo {filePath}...");
                while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await stream.WriteAsync(buffer, 0, bytesRead);
                }
            }

            Debug.Log("Arquivo enviado com sucesso.");
        }

        await Task.Delay(500);

        _sendFileStatus = SendFileResult.Success;
    }

    public void Release()
    {
        _sendFileStatus = SendFileResult.Inative;
    }

    public void SetServerInfo(string ip, int port)
    {
        serverIp = ip;
        this.port = port;
    }

    /*
    private enum NetworkCommand { RecordVideo, TransferFile }

    private class NetworkAction
    {
        public NetworkCommand networkCommand;
        public List<string> arguments;

        public NetworkAction(NetworkCommand command, params string[] args)
        {
            networkCommand = command;
            arguments = args.ToList();
        }
    }
    */
}
