using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 20;
    public int height = 20;
    
    public GameObject cellPrefab;
    
    public CellBehaviour[,] cells;
    
    
    void Start()
    {
        GenerateGrid();
        CenterCamera();
    }

    void GenerateGrid() 
    {
        var old = transform.Find("Cells");
        if (old) Destroy(old.gameObject);
        
        var parent = new GameObject("Cells").transform;
        parent.SetParent(transform, false);
        
        cells = new CellBehaviour[width, height];
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var go = Instantiate(cellPrefab, new Vector3(x, y, 0), Quaternion.identity, parent);
                go.name = $"Cell_{x}_{y}";
                var cb = go.GetComponent<CellBehaviour>();
                cb.SetAlive(false);
                cells[x, y] = cb;
            }
        }
    }
    
    public void ClearAll()
    {
        foreach (var c in cells)
        {
            c.SetAlive(false);
        }
    }

    public void Randomize(float p = 0.2f)
    {
        var rand = new System.Random();
        foreach (var c in cells)
        {
            c.SetAlive(rand.NextDouble() < p);
        }
    }
    
    void CenterCamera()
    {
        var cam = Camera.main;
        cam.orthographic = true;
        cam.transform.position = new Vector3(width/2f - 0.5f, height/2f - 0.5f, -10f);

        float aspect = (float)Screen.width / Screen.height;
        float sizeByH = height/2f + 1f;
        float sizeByW = (width/2f)/aspect + 1f;
        cam.orthographicSize = Mathf.Max(sizeByH, sizeByW);
    }
}
