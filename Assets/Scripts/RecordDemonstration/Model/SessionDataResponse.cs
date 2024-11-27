using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SessionDataResponse
{
    public string _audioFileName;
    public string _handDataFileName;
    public string _videoFileName;
    public List<StepDataResponse> _steps;

    public SessionDataResponse(string audioFileName, string handDataFileName, string videoFileName, List<StepDataResponse> steps)
    {
        _audioFileName = audioFileName;
        _handDataFileName = handDataFileName;
        _videoFileName = videoFileName;
        _steps = steps;
    }

}
