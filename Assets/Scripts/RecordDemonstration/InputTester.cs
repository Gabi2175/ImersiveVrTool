using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTester : MonoBehaviour
{
    public RecordDemonstrationManager recordDemonstrationManager;
    public HandTrackingDisplayManager displayHandManager;
    public AudioRecorderManager audioRecorderManager;



    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            recordDemonstrationManager.StartRecording();

        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            recordDemonstrationManager.StopRecording();
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            displayHandManager.StartPlayData();
        }
        else if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            audioRecorderManager.StartRecording("audioteste.mp3");
        }
        else if (Input.GetKeyUp(KeyCode.Alpha5))
        {
            audioRecorderManager.StopRecording();
        }
        else if (Input.GetKeyUp(KeyCode.Alpha6))
        {
            TCPFileTransferManager.Instance.SetServerInfo("127.0.0.1", 5000);
            TCPFileTransferManager.Instance.SendFile("D:/Pastas_Pessoais/ViniciusChrisosthemos/Github/ImmersiveModelingToolDev/Server/1_sem-estrutura_video_demonstration.mp4", "transfer ");
        }
        else if (Input.GetKeyUp(KeyCode.Alpha7))
        {
            TCPServerManager.Instance.SetServerInfo("127.0.0.1", 12345);
            TCPServerManager.Instance.StartServer();
        }
        else if (Input.GetKeyUp(KeyCode.Alpha8))
        {
            StartCoroutine(recordDemonstrationManager.GetTrainingSteps());
        }
    }
}
