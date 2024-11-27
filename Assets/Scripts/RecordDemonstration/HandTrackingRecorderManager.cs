using Meta.WitAi;
using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class HandTrackingRecorderManager : MonoBehaviour
{
    [SerializeField] private HandTrackingHelper _handTrackingHelper;

    [SerializeField] private SpatialAnchorManager _spatialAnchorManager;
    [SerializeField] private TextMeshProUGUI _log;

    [SerializeField] private Transform _refLeft;
    [SerializeField] private Transform _refRight;

    private List<Vector2> leftHand2D;
    private List<Vector2> rightHand2D;
    //[SerializeField] private Transform _cameraParent;
    [SerializeField] private Camera _auxCamera;


    private RecordableHandData _recordableHandData;

    private bool _isRecording = false;
    private bool _recordIsFinished = false;
    public string _fileName;

    private Coroutine _recordDataCoroutine;
    private long _initTime;

    public UnityEvent OnStartHandRecording;
    public UnityEvent OnEndHandRecording;

    public void StartRecording(string filename)
    {
        Debug.Log("Start Recording");

        _fileName = filename;
        _recordableHandData = new RecordableHandData();

        _isRecording = true;
        _recordIsFinished = false;
        leftHand2D = new List<Vector2>();
        rightHand2D = new List<Vector2>();

        _spatialAnchorManager.CreateAnchor(Vector3.zero, Quaternion.identity);

        _recordDataCoroutine = StartCoroutine(GetJointsData());
        OnStartHandRecording?.Invoke();
    }

    private IEnumerator GetJointsData(float fps=24)
    {
        float secondsToWait = 1 / fps;

        float accumTime = secondsToWait;


        _refLeft.GetComponent<TrackObject>().enabled = true;
        _refRight.GetComponent<TrackObject>().enabled = true;

        while (_isRecording)
        {
            if (accumTime > secondsToWait)
            {
                _recordableHandData.timestamps.Add(ClockManager.Instance.Time);
                _recordableHandData.hmd.Add(new JointData(_handTrackingHelper.HMD.localPosition, _handTrackingHelper.HMD.localEulerAngles));

                GetHandData(_handTrackingHelper.LeftHandTransforms, ref _recordableHandData.leftHand, ref leftHand2D, _refLeft);
                GetHandData(_handTrackingHelper.RightHandTransforms, ref _recordableHandData.rightHand, ref rightHand2D, _refRight);

                accumTime -= secondsToWait;
            }
            else
            {
                accumTime += Time.deltaTime;

                yield return null;
            }
        }

        _recordIsFinished = true;
        Debug.Log("Stop getting joinsdata");
    }

    public string StopRecording()
    {
        Debug.Log("[HandTrackingRecorderManager] Stop Recording");

        StopCoroutine(_recordDataCoroutine);

        //_fileName = $"{_fileName.Substring(0, _fileName.Length - 4)}_{_initTime}.htd";
        string filePathName = Path.Combine(Application.persistentDataPath, _fileName);

        _recordableHandData.anchorID = "temp";
        _recordableHandData.SaveToJson(filePathName);


        Debug.Log($"[HandTrackingRecorderManager]    file saved in {filePathName}");
        OnEndHandRecording?.Invoke();

        return filePathName;

    }


    public Transform _TEMPHMD;
    public Transform _TEMPFAKEHMD;

    private void GetHandData(List<Transform> handTransforms, ref HandData handData, ref List<Vector2> vector2D, Transform refPoint)
    {
        List<JointData> jointsData = new List<JointData>();

        for (int i = 1; i < handTransforms.Count; i++)
        {
            jointsData.Add(new JointData(handTransforms[i].localPosition, handTransforms[i].localEulerAngles));
        }

        handData.jointsData.Add(new ListWrapper<JointData>(jointsData));

        //_cameraParent.localPosition = _handTrackingHelper.HMD.localPosition;
        //_cameraParent.localEulerAngles = _handTrackingHelper.HMD.localEulerAngles;

        _TEMPFAKEHMD.position = _TEMPHMD.position;
        _TEMPFAKEHMD.rotation = _TEMPHMD.rotation;

        // Gera ponto 2D usando a camera
        Vector3 point = refPoint.position;
        Vector2 pos2d = _auxCamera.WorldToScreenPoint(point);

        pos2d.x = pos2d.x / _auxCamera.pixelWidth;
        pos2d.y = pos2d.y / _auxCamera.pixelHeight;

        vector2D.Add(pos2d);
    }

    public string Save2DCoords()
    {
        string filePath = Path.Combine(Application.persistentDataPath, _fileName + "2d");

        using (StreamWriter writetext = new StreamWriter(filePath))
        {
            for (int i = 0; i < leftHand2D.Count; i++)
            {
                string line = $"{_recordableHandData.timestamps[i]};{leftHand2D[i].x};{leftHand2D[i].y};{rightHand2D[i].x};{rightHand2D[i].y}";

                writetext.WriteLine(line);
            }
        }

        Debug.Log("[HandTrackingRecorderManager]    Arquivo 2d salvo com sucesso!");

        return filePath;
    }

    public string CurrentDataFile => _fileName;
}
