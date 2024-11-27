using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Plan;

[Serializable]
public class AugmentedDataResponse
{
    public string _objName;
    public TaskElement.ObjType _objType;
    public List<float> _position;
    public List<float> _rotation;

    public AugmentedDataResponse(string objName, TaskElement.ObjType objType, List<float> position, List<float> rotation)
    {
        _objName = objName;
        _objType = objType;
        _position = position;
        _rotation = rotation;
    }

}
