using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using UnityEngine;
using System.IO;

public class HandDemonstrationController : MonoBehaviour
{
    [SerializeField] private List<Transform> _leftHandJoints;
    [SerializeField] private List<Transform> _rightHandJoints;
    [SerializeField] private float _timeBetweenReplay = 2f;
    [SerializeField] private int _fps = 24;

    private RecordableHandData _handData;
    private Coroutine _playHandDataCoroutine;
    private int _startIndex = 0;
    private int _endIndex = 0;
    private string _handDataFilePath;

    public void PlayHandData(string handDataFilePath, float start = 0, float end = 0)
    {
        _handDataFilePath = handDataFilePath;

        if (File.Exists(handDataFilePath))
        {
            string jsonContent = File.ReadAllText(handDataFilePath);
            _handData = JsonUtility.FromJson<RecordableHandData>(jsonContent);

            _startIndex = (int) (_handData.hmd.Count * start);
            _endIndex = (int) (end == 0 ? _handData.hmd.Count : _handData.hmd.Count * end);

            _playHandDataCoroutine = StartCoroutine(PlayHandDataCoroutine());
        }
        else
        {
            Debug.Log($"[HandDemonstrationController][PlayHandData] Arquivo {handDataFilePath} nao encontrado");
        }
    }

    private IEnumerator PlayHandDataCoroutine()
    {
        float timeBetweenFrames = 1 / (float)_fps;

        while (true)
        {
            for (int i = _startIndex; i < _endIndex; i++)
            {
                UpdateJoints(_leftHandJoints, _handData.leftHand.jointsData[i]._itens);
                UpdateJoints(_rightHandJoints, _handData.rightHand.jointsData[i]._itens);

                yield return new WaitForSeconds(timeBetweenFrames);
            }

            yield return new WaitForSeconds(_timeBetweenReplay);
        }
    }

    private void UpdateJoints(List<Transform> joints, List<JointData> joinData)
    {
        for (int i = 0; i < joinData.Count; i++)
        {
            joints[i].localPosition = joinData[i].position.ToVector3();
            joints[i].localEulerAngles = joinData[i].rotation.ToVector3();
        }
    }

    private void OnDisable()
    {
        if (_playHandDataCoroutine != null) StopCoroutine(_playHandDataCoroutine);
    }

    public float StartTime => _startIndex;
    public float EndTime => _endIndex;
    public string HandDataFilePath => _handDataFilePath;
}
