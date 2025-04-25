using UnityEngine;
using Meta.XR;

public class ScriptTemp : MonoBehaviour
{
    [SerializeField] private Transform obj;
    [SerializeField] private EnvironmentRaycastManager manager;
    [SerializeField] private Camera _camera;

    private EnvironmentRaycastHit hit;
    private Vector2 centerScreen = Vector2.zero;

    private void Start()
    {
        centerScreen = new Vector2(Screen.width, Screen.height) * 0.5f;
    }

    private void Update()
    {
        UpdateObject(centerScreen);
    }

    public void UpdateObject(Vector3 position)
    {
        //Vector2 pos = camera.WorldToScreenPoint(position);
        Ray ray = _camera.ScreenPointToRay(centerScreen);

        if (manager.Raycast(ray, out hit, 100))
        {
            obj.transform.position = hit.point;
        }
    }

    public void OnBoxDetected(Vector2 position)
    {
        Ray ray = _camera.ScreenPointToRay(position);

        if (manager.Raycast(ray, out hit, 100))
        {
            obj.transform.position = hit.point;
        }
    }
}
