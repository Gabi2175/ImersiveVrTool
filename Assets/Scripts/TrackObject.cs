using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackObject : MonoBehaviour
{
    [SerializeField] private Transform _objectToTrack;
    [SerializeField] private Transform _tranformToUpdate;

    // Update is called once per frame
    void Update()
    {
        _tranformToUpdate.position = _objectToTrack.position;
        _tranformToUpdate.rotation = _objectToTrack.rotation;
    }
}
