using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Vector3Data
{
    public float x;
    public float y;
    public float z;

    public Vector3Data()
    {
        x = y = z = 0.0f;
    }

    public Vector3Data(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3Data(Vector3 vector3) : this(vector3.x, vector3.y, vector3.z) { }


    public Vector3 ToVector3() => new Vector3(x, y, z);
}
