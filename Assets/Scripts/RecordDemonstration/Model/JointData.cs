using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class JointData
{
    public Vector3Data position;
    public Vector3Data rotation;

    public JointData()
    {
        position = new Vector3Data();
        rotation = new Vector3Data();
    }

    public JointData(Vector3 position, Vector3 rotation)
    {
        this.position = new Vector3Data(position);
        this.rotation = new Vector3Data(rotation);
    }

    public JointData(Vector3Data position, Vector3Data rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
}
