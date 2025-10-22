using UnityEngine;

public class MatchController : MonoBehaviour
{
    public static MatchController Instance { get; private set; }

    public GridManager gridManager;
    public LifeRunner lifeRunner;

    public int piecesPerPlayer = 15;

    public bool IsPlacementActive;
    public string[] patternNames = { "Single", "Glider", "Blinker", "Beacon", "Gun" };
    public int selectedPattern = 0;
    
    int currentPlayer;
    int p1Left, p2Left;

    void Awake()
    {
        Instance = this;
        if (!gridManager) gridManager = GetComponent<GridManager>();
        if (!lifeRunner) lifeRunner = GetComponent<LifeRunner>();
    }

    public void BeginPlacement()
    {
        IsPlacementActive = true;
        currentPlayer = 1;
        p1Left = piecesPerPlayer;
        p2Left = piecesPerPlayer;

        lifeRunner.Pause();
        gridManager.ClearAll();
        lifeRunner.ResetScores();
    }
    
    public void SetPatternIndex(int i)
    {
        selectedPattern = Mathf.Clamp(i, 0, patternNames.Length-1);
        ShowHUD($"Pattern: {patternNames[selectedPattern]}");
    }

    public void OnCellClicked(CellBehaviour cell)
    {
        if (!IsPlacementActive) return;

        var owner = (currentPlayer == 1) ? CellBehaviour.Owner.P1 : CellBehaviour.Owner.P2;
        int left  = (currentPlayer == 1) ? p1Left : p2Left;

        var offs = PatternLibrary.Get(patternNames[selectedPattern]);
        int need = 0;
        foreach (var o in offs)
        {
            int x = Mathf.RoundToInt(cell.transform.position.x) + o.x;
            int y = Mathf.RoundToInt(cell.transform.position.y) + o.y;
            if (x < 0 || y < 0 || x >= gridManager.width || y >= gridManager.height) continue;
            var c = gridManager.cells[x, y];
            if (c.isAlive) continue;
            need++;
        }

        if (need == 0) return;

        if (need > left)
        {
            ShowHUD($"Not enough cells: need {need}, left {left}");
            return;
        }

        int placed = 0;
        foreach (var o in offs)
        {
            int x = Mathf.RoundToInt(cell.transform.position.x) + o.x;
            int y = Mathf.RoundToInt(cell.transform.position.y) + o.y;

            if (x < 0 || y < 0 || x >= gridManager.width || y >= gridManager.height) continue;
            var c = gridManager.cells[x, y];
            if (c.isAlive) continue; 
            c.SetAlive(true, owner);
            placed++;
        }
        
        if (placed == 0) return;

        if (currentPlayer == 1) p1Left -= placed;
        else p2Left -= placed;
        if (lifeRunner) lifeRunner.RecountScores();

        if (currentPlayer == 1) currentPlayer = (p2Left > 0) ? 2 : 1;
        else currentPlayer = (p1Left > 0) ? 1 : 2;

        ShowHUD($"P{currentPlayer} turn\nLeft: P1={p1Left}, P2={p2Left}");

        if (p1Left <= 0 && p2Left <= 0)
        {
            IsPlacementActive = false;
            ShowHUD("Match started!");
            lifeRunner.Play();
        }
    }
    
    public void PlacePatternClassic(CellBehaviour center)
    {
        if (gridManager == null) return;
        if (patternNames == null || patternNames.Length == 0) return;

        var patName = patternNames[Mathf.Clamp(selectedPattern, 0, patternNames.Length - 1)];
        var offs = PatternLibrary.Get(patName);

        int cx = Mathf.RoundToInt(center.transform.position.x);
        int cy = Mathf.RoundToInt(center.transform.position.y);

        foreach (var o in offs)
        {
            int x = cx + o.x;
            int y = cy + o.y;
            if (x < 0 || y < 0 || x >= gridManager.width || y >= gridManager.height) continue;

            var c = gridManager.cells[x, y];
            if (!c || c.isAlive) continue;

            c.SetAlive(true, CellBehaviour.Owner.None);
        }
    }

    void ShowHUD(string msg)
    {
        var ui = FindObjectOfType<UILinks>();
        if (ui) ui.ShowHUD(msg);
    }
}
