using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GridManager grid;
    public float zoomMin = 5f, zoomMax = 40f, zoomSpeed = 10f;

    Camera cam;
    Vector3 dragOriginWorld;
    bool dragging;

    void Awake(){ cam = GetComponent<Camera>(); cam.orthographic = true; }

    void Update()
    {
        var scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.01f)
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - scroll * zoomSpeed * Time.deltaTime,
                zoomMin, zoomMax);

        if (Input.GetMouseButtonDown(1)){ dragging = true;  dragOriginWorld = cam.ScreenToWorldPoint(Input.mousePosition); }
        if (Input.GetMouseButtonUp(1))  dragging = false;

        if (dragging)
        {
            var cur = cam.ScreenToWorldPoint(Input.mousePosition);
            var delta = dragOriginWorld - cur;
            transform.position += delta;
        }

        if (grid)
        {
            float left=-1f, bottom=-1f, right=grid.width, top=grid.height;
            var p = transform.position;
            p.x = Mathf.Clamp(p.x, left, right);
            p.y = Mathf.Clamp(p.y, bottom, top);
            transform.position = p;
        }
    }
}
