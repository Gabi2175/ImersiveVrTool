using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class HandTrackingDisplayManager : MonoBehaviour
{
    [SerializeField] private HandTrackingRecorderManager _handTrackingRecorderManager;

    [SerializeField] private Transform _hmd;
    [SerializeField] private List<Transform> _leftHandJoints;
    [SerializeField] private List<Transform> _rightHandJoints;
    [SerializeField] private Transform _refLeft;
    [SerializeField] private Transform _refRight;

    [SerializeField] private Image _imgPlayHandDataStatus;

    private RecordableHandData _recordableHandData;

    private float _fps = 24f;

    private int _idxFinder = 19;

    private Coroutine _playDataCoroutine;

    private List<Vector2> leftHand2D;
    private List<Vector2> rightHand2D;
    //[SerializeField] private Transform _cameraParent;
    [SerializeField] private Camera _auxCamera;

    public void StartPlayData()
    {
        string filePath = Path.Combine(Application.persistentDataPath, _handTrackingRecorderManager.CurrentDataFile);

        if (!File.Exists(filePath)){
            _imgPlayHandDataStatus.color = Color.red;
            return;
        }

        string json = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
        _recordableHandData = JsonConvert.DeserializeObject<RecordableHandData>(json);

        Debug.Log($"Start Play data");

        leftHand2D = new List<Vector2>();
        rightHand2D = new List<Vector2>();

        _playDataCoroutine = StartCoroutine(PlayData(_fps));

        _imgPlayHandDataStatus.color = Color.green;
    }

    public void StopPlay()
    {
        StopCoroutine(_playDataCoroutine);
    }


    private IEnumerator PlayData(float fps)
    {
        float secondToWait = 1 / _fps;

        int counter = 0;
        int maxCount = _recordableHandData.hmd.Count;

        _refLeft.GetComponent<TrackObject>().enabled = false;
        _refRight.GetComponent<TrackObject>().enabled = false;

        while (counter < maxCount)
        {
            Debug.Log($"IDX => {counter}");
            UpdateHMD(_recordableHandData.hmd[counter]);
            UpdateJoints(_recordableHandData.leftHand.jointsData[counter]._itens, _leftHandJoints, _refLeft);
            UpdateJoints(_recordableHandData.rightHand.jointsData[counter]._itens, _rightHandJoints, _refRight);

            Save2DCoords(ref leftHand2D, _recordableHandData.hmd[counter], _refLeft, _green);
            Save2DCoords(ref rightHand2D, _recordableHandData.hmd[counter], _refRight, _blue);

            
            if (counter == -1)
            {
                Debug.Log($"==>  {counter}  {_refLeft.position} || {_refRight.position}");

                yield return new WaitForSeconds(2);
            }
            

            counter++;

            yield return new WaitForSeconds(secondToWait);
        }

        _imgPlayHandDataStatus.color = Color.gray;
        SaveRecord();
    }

    private void SaveRecord()
    {
        string filePath = Path.Combine(Application.persistentDataPath, _handTrackingRecorderManager.CurrentDataFile + "2d");

        using (StreamWriter writetext = new StreamWriter(filePath))
        {
            for (int i = 0; i < leftHand2D.Count; i++)
            {
                string line = $"{_recordableHandData.timestamps[i]};{leftHand2D[i].x};{leftHand2D[i].y};{rightHand2D[i].x};{rightHand2D[i].y}";

                writetext.WriteLine(line);
            }
        }

        Debug.Log("Arquivo 2d salvo com sucesso!");
    }

    public RectTransform _green;
    public RectTransform _blue;

    private void Save2DCoords(ref List<Vector2> handData, JointData hmdInfo, Transform pointToRecord, RectTransform temp)
    {
        //_cameraParent.transform.localPosition = hmdInfo.position.ToVector3();
        //_cameraParent.transform.localEulerAngles = hmdInfo.rotation.ToVector3();

        Vector3 point = pointToRecord.position;

        Vector2 pos2d = _auxCamera.WorldToScreenPoint(point);
        temp.anchoredPosition = pos2d;

        string message = $"{_auxCamera.pixelWidth}/{_auxCamera.pixelHeight}  {point}  Pos={pos2d}";
        pos2d.x = pos2d.x / _auxCamera.pixelWidth;
        pos2d.y = pos2d.y / _auxCamera.pixelHeight;

        message += $"   {pos2d}";
        Debug.Log(message);

        handData.Add(pos2d);
    }

    private void UpdateHMD(JointData hmdInfo)
    {
        _hmd.localPosition = hmdInfo.position.ToVector3();
        _hmd.localEulerAngles = hmdInfo.rotation.ToVector3();
    }

    private void UpdateJoints(List<JointData> jointsData, List<Transform> joints, Transform refPoint)
    {
        for (int i = 0; i < jointsData.Count; i++)
        {
            joints[i].localPosition = jointsData[i].position.ToVector3();
            joints[i].localEulerAngles = jointsData[i].rotation.ToVector3();
        }

        refPoint.position = joints[_idxFinder].position;
        refPoint.rotation = joints[_idxFinder].rotation;
    }
}
