using TMPro;
using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    public enum GameMode { Classic, PvP }
    
    public GridManager gridManager;
    public LifeRunner lifeRunner;
    public MatchController matchController;
    
    public GameMode CurrentMode { get; private set; } = GameMode.Classic;
    
    void Start()
    {
        ApplyMode(CurrentMode, doInit:true);
    }
    
    public void ToggleMode()
    {
        var next = (CurrentMode == GameMode.Classic) ? GameMode.PvP : GameMode.Classic;
        SetMode(next);
    }

    public void SetMode(GameMode m)
    {
        if (m == CurrentMode) return;
        ApplyMode(m, doInit:true);
    }
    
    void ApplyMode(GameMode m, bool doInit)
    {
        lifeRunner.Pause();
        gridManager.ClearAll();
        lifeRunner.ResetScores();

        if (matchController)
        {
            matchController.enabled = (m == GameMode.PvP);
            // matchController.IsPlacementActive = false;
        }

        if (m == GameMode.PvP && matchController && doInit)
        {
            matchController.BeginPlacement();
        }

        CurrentMode = m;
        var result = FindObjectOfType<ResultUI>();
        if (result) result.Hide();
    }
}
