using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ObjectDetectionController : MonoBehaviour
{
    public RectTransform _canvasRecTransform;
    public List<RectTransform> _objectDetectedUI;
    [SerializeField] private int width = 1000;
    [SerializeField] private int height = 810;

    // Porta onde o servidor UDP irá enviar os dados
    public string ip = "127.0.0.1";
    public int port = 12345;

    public float _movementForce = 0.01f;
    public float _sizeForce = 10f;
    public float _rotationForce = 1f;
    public TextMeshProUGUI _log;

    public Vector2 _temp1;
    public Vector2 _temp2;

    // Lista para armazenar os valores recebidos
    private Dictionary<string, RectData> receivedRectData = new Dictionary<string, RectData>();

    private Socket socket;
    private bool isReceiving = false;

    // Start é chamado antes do primeiro quadro de atualização
    void Start()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        socket.Bind(endPoint);

        receivedRectData.Add("box", new RectData(0, 0, 0, 0));
        receivedRectData.Add("controller", new RectData(0, 0, 0, 0));
        receivedRectData.Add("oculus", new RectData(0, 0, 0, 0));

        // Iniciar a recepção de dados em segundo plano
        isReceiving = true;
        ReceiveData();
    }

    private void Update()
    {
        int i = 0;
        foreach(string key in receivedRectData.Keys)
        {
            var rectData = receivedRectData[key];
            var element = _objectDetectedUI[i];

            //element.sizeDelta = new Vector2(rectData.width * width, rectData.height * height);
            element.anchoredPosition = new Vector2(rectData.x * _canvasRecTransform.rect.width, _canvasRecTransform.rect.height - rectData.y * _canvasRecTransform.rect.height);
            i++;
        }

        if (Input.GetKeyUp(KeyCode.O))
        {
            LeftAxis2DChanged(_temp1);
        }

        if (Input.GetKeyUp(KeyCode.P))
        {
            RightAxis2DChanged(_temp2);
        }
    }

    // Função de recepção de dados UDP
    private async void ReceiveData()
    {
        try
        {
            await Task.Run(() =>
            {
                while (isReceiving)
                {
                    //byte[] dataByte = client.Receive(ref anyIP);
                    byte[] dataByte = new byte[4096];
                    socket.Receive(dataByte);

                    string message = Encoding.UTF8.GetString(dataByte);

                    Debug.Log($"Mensagem recebida: {message}");

                    //_log.text = $"message: {message}";

                    // Processar a mensagem recebida
                    ProcessMessage(message);
                }
            });
        }
        catch (OperationCanceledException)
        {
            Debug.Log("UDP Thread Cancelled");

        }
        catch (Exception error)
        {
            Debug.LogError(error.Message);
        }

        socket.Close();
    }

    // Processa a mensagem recebida
    private void ProcessMessage(string message)
    {
        // Assumindo que a mensagem seja uma string JSON ou separada por vírgulas
        string[] lines = message.Replace(",", ".").Split('\n') ;

        foreach (string line in lines)
        {
            string[] values = line.Split(';');

            if (values.Length == 5)
            {
                // Converter os valores recebidos para floats
                float x = float.Parse(values[0]);
                float y = float.Parse(values[1]);
                float width = float.Parse(values[2]);
                float height = float.Parse(values[3]);
                string objectName = values[4];

                //_log.text = $"{values[0]} {x}";

                var obj = receivedRectData[objectName];
                obj.x = x;
                obj.y = y;
                obj.width = width;
                obj.height = height;
            }
        }
    }

    // Função para parar a recepção de dados
    private void OnApplicationQuit()
    {
        isReceiving = false;
        socket.Close();
    }

    private class RectData
    {
        public float x;
        public float y;
        public float width;
        public float height;
        public bool updated;

        public RectData(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            updated = false;
        }
    }


    private void LeftAxis2DChanged(Vector2 vector)
    {
        Vector2 movement = Vector2.zero;

        movement.x = MathF.Abs(vector.x) > 0.8f ? vector.x / MathF.Abs(vector.x) : 0f;
        movement.y = MathF.Abs(vector.y) > 0.8f ? vector.y / MathF.Abs(vector.y) : 0f;

        if (_canMove)
        {
            _canvasRecTransform.anchoredPosition += movement * _movementForce;
        }

        if (_canMove2)
        {
            _canvasRecTransform.transform.localPosition += movement.y * _movementForce * Vector3.forward;
        }

        _log.text = $"{_canvasRecTransform.anchoredPosition} {_canvasRecTransform.sizeDelta}\n {_canvasRecTransform.transform.eulerAngles} {_canvasRecTransform.transform.localPosition}";
    }

    private void RightAxis2DChanged(Vector2 vector)
    {
        Vector2 movement = Vector2.zero;

        movement.x = MathF.Abs(vector.x) > 0.8f ? vector.x / MathF.Abs(vector.x) : 0f;
        movement.y = MathF.Abs(vector.y) > 0.8f ? vector.y / MathF.Abs(vector.y) : 0f;

        if (_canScale)
        {
            _canvasRecTransform.sizeDelta += movement * _sizeForce;
        }

        if (_canRotate)
        {
            _canvasRecTransform.transform.eulerAngles += movement.x * _rotationForce * Vector3.forward;
        }

        _log.text = $"{_canvasRecTransform.anchoredPosition} {_canvasRecTransform.sizeDelta}\n {_canvasRecTransform.transform.eulerAngles} {_canvasRecTransform.transform.localPosition}";
    }


    public bool _canMove = true;
    public bool _canRotate = false;
    public bool _canScale = true;
    public bool _canMove2 = false;

    public void SetCanMove(bool value) => _canMove = value;
    public void SetCanMove2(bool value) => _canMove2 = value;
    public void SetCanRotate(bool value) => _canRotate = value;
    public void SetCanScale(bool value) => _canScale = value;

    private void OnEnable()
    {
        InputController.Instance.OnLeftAxis2DChanged.AddListener(LeftAxis2DChanged);
        InputController.Instance.OnRightAxis2DChanged.AddListener(RightAxis2DChanged);
    }
}
