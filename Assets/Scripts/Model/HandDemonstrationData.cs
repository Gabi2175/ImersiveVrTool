using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HandDemonstrationData
{
    public string _handDataFilePath;
    public float _startTime;
    public float _endTime;

    public HandDemonstrationData(string handDataFilePath, float startTime, float endTime)
    {
        _handDataFilePath = handDataFilePath;
        _startTime = startTime;
        _endTime = endTime;
    }
}
