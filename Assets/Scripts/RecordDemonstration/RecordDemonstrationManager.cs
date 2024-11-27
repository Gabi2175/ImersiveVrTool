using Meta.WitAi.Json;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Plan;


public class RecordDemonstrationManager : MonoBehaviour
{
    [SerializeField] private Image _imgRecordingStatus;
    [SerializeField] private Button _btnStartRecording;
    [SerializeField] private Button _btnStopRecording;

    [SerializeField] private Toggle _toggleRecordVideo;
    [SerializeField] private Toggle _toggleRecordHands;
    [SerializeField] private Toggle _toggleRecordAudio;
    [SerializeField] private Toggle _toggleSendToServer;

    [SerializeField] private HandTrackingDisplayManager _handTrackingDisplayManager;
    [SerializeField] private HandTrackingRecorderManager _handTrackingRecorderManager;
    [SerializeField] private AudioRecorderManager _audioRecorderManager;

    [SerializeField] private TextMeshProUGUI _textLogStatus;
    [SerializeField] private TextMeshProUGUI _textLog;
    [SerializeField] private TMP_InputField _InputFieldIP;
    [SerializeField] private TMP_InputField _InputFieldReceivingIP;
    [SerializeField] private TMP_InputField _InputFieldPort;
    [SerializeField] private TMP_InputField _InputFieldReceiverPort;

    [SerializeField] private Image _imgSyncStatus;

    [SerializeField] private int _waitTime = 4;

    [SerializeField] private TaskManager taskManager;

    private string prefix_file_name = "";

    public void StartRecording()
    {
        StartCoroutine(StartRecordingCoroutine());
    }

    public void SyncClock()
    {
        _imgSyncStatus.color = Color.yellow;

        ClockManager.Instance.SyncToServer((result) =>
        {
            _imgSyncStatus.color = result ? Color.green : Color.red;
        });
    }

    private IEnumerator StartRecordingCoroutine()
    {
        _btnStartRecording.gameObject.SetActive(false);
        _btnStopRecording.gameObject.SetActive(true);
        _btnStartRecording.interactable = false;

        TCPFileTransferManager.Instance.SetServerInfo(_InputFieldIP.text, int.Parse(_InputFieldPort.text));

        _imgRecordingStatus.color = Color.green;

        prefix_file_name = DateTime.Now.ToString("dd-MM-yyyy-HH_mm_ss");

        if (_toggleRecordVideo.isOn)
        {
            TCPFileTransferManager.Instance.SendCommand($"init {prefix_file_name}_video_demonstration.mp4 {_waitTime}");

            _textLogStatus.text = $"Status: Esperando {_waitTime} segundos para comecar ...";

            yield return new WaitForSeconds(_waitTime);
        }


        if (_toggleRecordHands.isOn)
            _handTrackingRecorderManager.StartRecording($"{prefix_file_name}_handtracking_demonstration.htd");

        if (_toggleRecordAudio.isOn)
            _audioRecorderManager.StartRecording($"{prefix_file_name}_audio_demonstration");


        _textLogStatus.text = "Status: gravando ...";
        _btnStartRecording.interactable = true;
    }

    public void StopRecording()
    {
        _imgRecordingStatus.color = Color.grey;

        List<string> filesToSend = new List<string>();

        if (_toggleRecordVideo.isOn)
            TCPFileTransferManager.Instance.SendCommand($"stop");

        if (_toggleRecordHands.isOn)
        {
            string handDataFile = _handTrackingRecorderManager.StopRecording();
            string handData2DFile = _handTrackingRecorderManager.Save2DCoords();

            filesToSend.Add(handDataFile);
            filesToSend.Add(handData2DFile);
        }

        if (_toggleRecordAudio.isOn)
        {
            string audioDataFile = _audioRecorderManager.StopRecording();

            if (audioDataFile != "")
                filesToSend.Add(audioDataFile);
        }


        if (_toggleSendToServer.isOn)
        {
            if (!_toggleRecordAudio.isOn)
                StartCoroutine(SendResults(filesToSend, null));
            else
                StartCoroutine(SendResults(filesToSend, CreateTrainingSession));
        }

        _btnStartRecording.gameObject.SetActive(true);
        _btnStopRecording.gameObject.SetActive(false);
    }

    private IEnumerator SendResults(List<string> filesToSend, Action onFinished)
    {
        _btnStartRecording.interactable = false;
        _btnStopRecording.interactable = false;

        var tcpManager = TCPFileTransferManager.Instance;

        while (filesToSend.Count > 0)
        {
            string file = filesToSend[0];

            _textLogStatus.text = $"Status: Enviando {file} ...";

            filesToSend.RemoveAt(0);

            tcpManager.SendFile(file, "transfer ");

            yield return new WaitUntil(() => tcpManager.SendFileStatus != TCPFileTransferManager.SendFileResult.Processing);

            Debug.Log(tcpManager.SendFileStatus.ToString());

            if (tcpManager.SendFileStatus == TCPFileTransferManager.SendFileResult.Success)
            {
                Debug.Log($"Arquivo {file} enviado com sucesso!");
            }
            else if (tcpManager.SendFileStatus == TCPFileTransferManager.SendFileResult.Failure)
            {
                _textLogStatus.text = $"Status: erro ao enviar o arquivo {file}";
                Debug.Log($"erro ao enviar o arquivo {file}");
            }

            tcpManager.Release();

            yield return new WaitForSeconds(0.5f);
        }

        _textLogStatus.text = "Status: Arquivos enviados!";

        _btnStartRecording.interactable = true;
        _btnStopRecording.interactable = true;

        onFinished?.Invoke();
    }

    public void GetSteps()
    {
        StartCoroutine(GetTrainingSteps());
    }

    public void CreateTrainingSession()
    {
        TCPFileTransferManager.Instance.SendCommand($"create {prefix_file_name}");

        StartCoroutine(GetTrainingSteps());
    }

    public IEnumerator GetTrainingSteps()
    {
        _textLog.text += $"\nEsperando servidor montar as etapas ...";

        string ip = _InputFieldReceivingIP.text;
        int port = int.Parse(_InputFieldReceiverPort.text);

        var server = TCPServerManager.Instance;

        server.SetServerInfo(ip, port);

        server.StartServer();

        //Debug.Log(TCPFileTransferManager.Instance.SendFileStatus);
        yield return new WaitUntil(() => server.SendFileStatus != TCPFileTransferManager.SendFileResult.Processing);

        //Debug.Log(TCPFileTransferManager.Instance.SendFileStatus);
        _textLogStatus.text = $"Status: Session data recebido!";

        server.StopServer();

        CreateSteps(server.result);
    }

    private void CreateSteps(string sessionDataResponseJson)
    {
        Debug.Log($"[RecordDemonstrationManager][CreateSteps] Tratando session data response ...");
        Plan plan = taskManager.GetPlan();

        Debug.Log(sessionDataResponseJson);
        var sessionDataResponse = JsonUtility.FromJson<SessionDataResponse>(sessionDataResponseJson);

        for (int i = 0; i < sessionDataResponse._steps.Count; i++)
        {
            var step = sessionDataResponse._steps[i];

            Task task = new Task($"Step {i + 1}");

            foreach (var augmentedObject in step._augmentedObjects)
            {
                TaskElement taskElement;

                if (augmentedObject._objType == TaskElement.ObjType.CustomTextPainel)
                {
                    taskElement = new TaskElement($"{augmentedObject._objName}.ptx", TaskElement.ObjType.CustomTextPainel);
                    taskElement.scale = new Vector3(470, 300, 1);
                }
                else
                {
                    taskElement = new TaskElement(augmentedObject._objName, augmentedObject._objType);
                    taskElement.scale = new Vector3(1, 1, 1);
                }

                SceneElement sceneElement = new SceneElement();

                sceneElement.positon = ListToVector3(augmentedObject._position);
                sceneElement.eulerAngles = ListToVector3(augmentedObject._rotation);
                sceneElement.taskElements.Add(taskElement);

                task.sceneElements.Add(sceneElement);
            }

            string handDataPath = Path.Combine(Application.persistentDataPath, sessionDataResponse._handDataFileName);
            var handDemonstration = new HandDemonstrationData(handDataPath, step._startTimeStep, step._endTimeStep);

            task._handDemonstrations.Add(handDemonstration);

            plan.AddTaskInTheEnd(task);
        }

        taskManager.UpdateTask(null, plan.CurrentTask);
        Debug.Log($"[RecordDemonstrationManager][CreateSteps] Session data tratado com sucesso!");
    }

    private Vector3 ListToVector3(List<float> list)
    {
        if (list.Count != 3) return Vector3.zero;

        return new Vector3(list[0], list[1], list[2]);
    }
}
