using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HandData
{
    public List<ListWrapper<JointData>> jointsData;

    public HandData()
    {
        jointsData = new List<ListWrapper<JointData>>();
    }

    public HandData(List<JointData> pivotData, List<ListWrapper<JointData>> jointsData)
    {
        this.jointsData = jointsData;
    }
}
