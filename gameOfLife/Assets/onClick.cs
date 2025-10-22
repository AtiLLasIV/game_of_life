using UnityEngine;

public class onClick : MonoBehaviour
{
    public Camera cam;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var wp = cam.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.OverlapPoint(new Vector2(wp.x, wp.y));

            var cell = hit.GetComponent<CellBehaviour>() ?? hit.GetComponentInParent<CellBehaviour>();
            cell.SetAlive(!cell.isAlive);
        }
    }
}
