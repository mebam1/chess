using UnityEngine;
using UnityEngine.EventSystems;

public class TestLogger : MonoBehaviour, IDragHandler
{
    int dragTargetLayer = 0;
    Camera mainCam;
    //EventTrigger evtTrigger;

    public void Log(string msg)
    {
        Debug.Log(msg);
    }

    private void Awake()
    {
        mainCam = Camera.main;
        dragTargetLayer = LayerMask.GetMask("GRID");
        //evtTrigger = GetComponent<EventTrigger>();
    }

    public void Move()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out var hit, mainCam.farClipPlane, dragTargetLayer))
        {
            transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
        }
    }
    

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("!");
        Move();
    }
}
