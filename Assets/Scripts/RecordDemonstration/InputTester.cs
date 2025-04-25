using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputTester : MonoBehaviour
{
    public RecordDemonstrationManager recordDemonstrationManager;
    public HandTrackingDisplayManager displayHandManager;
    public AudioRecorderManager audioRecorderManager;



    private void Update()
    {
        if (Keyboard.current[Key.Digit1].wasReleasedThisFrame)
        {
            recordDemonstrationManager.StartRecording();

        }
        else if (Keyboard.current[Key.Digit2].wasReleasedThisFrame)
        {
            recordDemonstrationManager.StopRecording();
        }
        else if (Keyboard.current[Key.Digit3].wasReleasedThisFrame)
        {
            displayHandManager.StartPlayData();
        }
        else if (Keyboard.current[Key.Digit4].wasReleasedThisFrame)
        {
            audioRecorderManager.StartRecording("audioteste.mp3");
        }
        else if (Keyboard.current[Key.Digit5].wasReleasedThisFrame)
        {
            audioRecorderManager.StopRecording();
        }
        else if (Keyboard.current[Key.Digit6].wasReleasedThisFrame)
        {
            TCPFileTransferManager.Instance.SetServerInfo("127.0.0.1", 5000);
            TCPFileTransferManager.Instance.SendFile("D:/Pastas_Pessoais/ViniciusChrisosthemos/Github/ImmersiveModelingToolDev/Server/1_sem-estrutura_video_demonstration.mp4", "transfer ");
        }
        else if (Keyboard.current[Key.Digit7].wasReleasedThisFrame)
        {
            TCPServerManager.Instance.SetServerInfo("127.0.0.1", 12345);
            TCPServerManager.Instance.StartServer();
        }
        else if (Keyboard.current[Key.Digit8].wasReleasedThisFrame)
        {
            StartCoroutine(recordDemonstrationManager.GetTrainingSteps());
        }
    }
}
