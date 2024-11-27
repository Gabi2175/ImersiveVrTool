using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class AudioRecorderManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    private string _fileName;
    private AudioClip _audioClip;

    private float _startTime;

    public TextMeshProUGUI _log;

    public void StartRecording(string fileName)
    {
        _fileName = fileName;
        _startTime = Time.realtimeSinceStartup;

        int timeInMinutes = 10;
        string microfoneDeviceName = "";

        if (Microphone.devices.Length == 0)
        {
            _log.text += $"\nNenhum microfone foi encontrado!";
            return;
        }

        microfoneDeviceName = Microphone.devices[0];
        int minFreq, maxFreq;
        Microphone.GetDeviceCaps(microfoneDeviceName, out minFreq, out maxFreq);


        _audioClip = Microphone.Start(microfoneDeviceName, true, timeInMinutes * 60, minFreq);
        audioSource.clip = _audioClip;
        audioSource.Play();

        _log.text += $"\nStart Audio Record with device {microfoneDeviceName} {Microphone.devices.Length} {minFreq}";
    }

    public string StopRecording()
    {
        audioSource.Stop();

        float trueAudioTime = Time.realtimeSinceStartup - _startTime;
        AudioClip formattedAudioClip = CopyAudioClip(_audioClip, 0, trueAudioTime);

        if (formattedAudioClip == null)
        {
            _log.text += "\nErro ao formatar audioclip";
            return "";
        }

        string filePath = Path.Combine(Application.persistentDataPath, _fileName);
        SavWav.SaveWav(filePath, formattedAudioClip);

        return filePath + ".wav";
    }

    //ChatGPT
    public AudioClip CopyAudioClip(AudioClip audioClip, float startTime, float endTime)
    {
        // Obtenha as amostras de áudio do clip de origem
        float[] sourceData = new float[audioClip.samples * audioClip.channels];
        audioClip.GetData(sourceData, 0);

        // Calcule os índices de início e fim com base no tempo
        int startSample = Mathf.RoundToInt(startTime * audioClip.frequency);
        int endSample = Mathf.RoundToInt(endTime * audioClip.frequency);

        // Verifique se os índices são válidos
        if (startSample < 0 || endSample > sourceData.Length || startSample >= endSample)
        {
            _log.text += "\nIntervalo de amostras inválido.";
            return null;
        }

        // Calcule o número de amostras para copiar
        int sampleCount = endSample - startSample;

        // Crie o novo clip
        AudioClip newClip = AudioClip.Create("NewAudioClip", sampleCount, audioClip.channels, audioClip.frequency, false);

        // Copie as amostras do intervalo
        float[] destinationData = new float[sampleCount * audioClip.channels];
        Array.Copy(sourceData, startSample * audioClip.channels, destinationData, 0, destinationData.Length);

        // Defina os dados do novo clip
        newClip.SetData(destinationData, 0);

        return newClip;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
