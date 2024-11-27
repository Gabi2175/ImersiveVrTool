using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTrackingHelper : MonoBehaviour
{
    [SerializeField] private Transform _hmdToTrack;
    [SerializeField] private Transform _hmd;

    [SerializeField] private HandVisual _leftHandJoints;
    [SerializeField] private HandVisual _rightHandJoints;

    [SerializeField] private List<Transform> _leftHandTransforms;
    [SerializeField] private List<Transform> _rightHandTransforms;

    private bool isActive = false;

    public Transform HMD => _hmd;
    public List<Transform> LeftHandTransforms => _leftHandTransforms;
    public List<Transform> RightHandTransforms => _rightHandTransforms;

    public void Update()
    {
        if (isActive == true)
        {
            UpdateHMD();
            UpdateHand(_rightHandTransforms, _rightHandJoints.Joints);
            UpdateHand(_leftHandTransforms, _leftHandJoints.Joints);
        }
    }

    private void UpdateHMD()
    {
        _hmd.position = _hmdToTrack.position;
        _hmd.rotation = _hmdToTrack.rotation;
    }

    public void UpdateHand(List<Transform> handTransforms, IList<Transform> joints)
    {
        for (int i = 0; i < joints.Count; i++)
        {
            handTransforms[i].position = joints[i].position;
            handTransforms[i].rotation = joints[i].rotation;
        }
    }

    public void OnStartHandTracking(bool isTracking)
    {
        isActive = isTracking;
    }
}
