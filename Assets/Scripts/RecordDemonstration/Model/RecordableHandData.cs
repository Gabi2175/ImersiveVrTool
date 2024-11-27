using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

[Serializable]
public class RecordableHandData
{
    public string anchorID;
    public List<JointData> hmd;
    public HandData leftHand;
    public HandData rightHand;
    public List<long> timestamps;

    public RecordableHandData()
    {
        anchorID = "";
        hmd = new List<JointData>();
        leftHand = new HandData();
        rightHand = new HandData();
        timestamps = new List<long>();
    }

    public void SaveToJson(string filePathName)
    {
        string json = JsonConvert.SerializeObject(this, Formatting.Indented);

        File.WriteAllText(filePathName, json);
    }
}
