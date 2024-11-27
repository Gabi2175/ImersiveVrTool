using Newtonsoft.Json;
using Photon.Pun.Demo.SlotRacer.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClockManager : Singleton<ClockManager>
{
    [SerializeField] private TMP_InputField _inputFieldIPToSync;
    [SerializeField] private TMP_InputField _inputFieldPortToSync;

    [SerializeField] private TMP_InputField _inputFieldIP;
    [SerializeField] private TMP_InputField _inputFieldPort;

    public TextMeshProUGUI _log;
    private long _pivot = 0;
    private long _offset = 0;

    private void Start()
    {
        _pivot = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
    }

    void Update()
    {
        _log.text = Time.ToString();
    }

    public void SyncToServer(Action<bool> callback)
    {
        TCPFileTransferManager.Instance.SetServerInfo(_inputFieldIPToSync.text, int.Parse(_inputFieldPortToSync.text));
        TCPFileTransferManager.Instance.SendCommand("sync");
        StartCoroutine(SyncToServerCoroutine(callback));
    }

    private IEnumerator SyncToServerCoroutine(Action<bool> callback)
    {
        var server = TCPServerManager.Instance;
        server.SetServerInfo(_inputFieldIP.text, int.Parse(_inputFieldPort.text));
        server.StartServer();

        yield return new WaitWhile(() => server.SendFileStatus == TCPFileTransferManager.SendFileResult.Processing);

        if (server.SendFileStatus == TCPFileTransferManager.SendFileResult.Success)
        {
            _offset = long.Parse(server.result);
            _pivot = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        
            callback?.Invoke(true);
        }
        else
        {
            callback?.Invoke(false);
        }

        server.StopServer();
    }

    public long Time => new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - _pivot + _offset;
}
