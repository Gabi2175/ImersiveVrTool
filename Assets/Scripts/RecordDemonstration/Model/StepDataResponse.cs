using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StepDataResponse
{
    public string _description;
    public float _startTimeStep;
    public float _endTimeStep;

    public List<AugmentedDataResponse> _augmentedObjects;

    public StepDataResponse(string description, float startTimeStep, float endTimeStep, List<AugmentedDataResponse> augmentedObjects)
    {
        _description = description;
        _startTimeStep = startTimeStep;
        _endTimeStep = endTimeStep;
        _augmentedObjects = augmentedObjects;
    }

}
