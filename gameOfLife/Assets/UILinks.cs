using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILinks : MonoBehaviour
{
    public LifeRunner LifeRunner;
    public GridManager GridManager;
    public GameModeManager GameModeManager;
    public MatchController MatchController;
    public ResultUI Result;
    public Button RandomButton;
    public Button EndMatchButton;

    
    public Slider Slider;
    public TMP_Text ScoreText;
    public TMP_Text HudText;
    public CanvasGroup BottomPanel;

    public float minDelay = 0.03f;
    public float maxDelay = 0.50f;
    public float showSec = 7.0f;

    private bool flagRunning;
    private float timer;
    
    
    void Start()
    {
        UpdateEndMatchButton();
        UpdateRandomButton();
        if (Slider)
        {
            Slider.minValue = minDelay;
            Slider.maxValue = maxDelay;
            Slider.value = Mathf.Clamp(LifeRunner.stepDelay, minDelay, maxDelay);
            Slider.onValueChanged.AddListener(v =>
            {
                LifeRunner.stepDelay = v;
                ShowHUD($"Speed: {1f / v:0.0} step/s");
            });
            ShowHUD("Space: Play/Pause\nS: Step\nR: Random\nC: Clear\n+/-: Speed\nTab: Show/Hide menu\n[|]: Change pattern");
        }
    }

    void Update()
    {
        UpdateEndMatchButton();
        UpdateRandomButton();
        if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.Equals)) ChangeSpeed(-0.2f);
        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.Underscore)) ChangeSpeed(+0.2f);
        if (Input.GetKeyDown(KeyCode.Space)) OnPlayPause();
        if (Input.GetKeyDown(KeyCode.S)) OnStep();
        if (Input.GetKeyDown(KeyCode.R)) OnRandom();
        if (Input.GetKeyDown(KeyCode.C)) OnClear();
        if (Input.GetKeyDown(KeyCode.Tab)) TogglePanel();
        if (Input.GetKeyDown(KeyCode.LeftBracket)) PrevPattern();
        if (Input.GetKeyDown(KeyCode.RightBracket)) NextPattern();
        
        bool pvp = GameModeManager && GameModeManager.CurrentMode == GameModeManager.GameMode.PvP;
        if (ScoreText) ScoreText.gameObject.SetActive(pvp);

        if (pvp && ScoreText && LifeRunner)
            ScoreText.text = $"P1: {LifeRunner.scoreP1}\nP2: {LifeRunner.scoreP2}";

        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f && HudText)
            {
                HudText.text = "";
            }
        }
        
    }
    
    void UpdateEndMatchButton()
    {
        if (!EndMatchButton || !GameModeManager) return;
        EndMatchButton.interactable = (GameModeManager.CurrentMode == GameModeManager.GameMode.PvP);
    }
    
    void UpdateRandomButton()
    {
        if (!RandomButton || !GameModeManager) return;
        RandomButton.interactable = (GameModeManager.CurrentMode == GameModeManager.GameMode.Classic);
    }

    public void OnEndMatch()
    {
        if (!GameModeManager || GameModeManager.CurrentMode != GameModeManager.GameMode.PvP) return;
        
        if (LifeRunner)
        {
            LifeRunner.Pause();
            LifeRunner.RecountScores();
        }
        if (Result) Result.Show();
    }

    public void OnPlayPause()
    {
        bool isPause = LifeRunner.Toggle();
        if (isPause) ShowHUD("<Space>: Pause", false);
        else ShowHUD("<Space>: Play");
    }

    public void OnStep()
    {
        LifeRunner.StepOnce();
        ShowHUD("<S>: Step");
    }

    public void OnRandom()
    {
        GridManager.Randomize(LifeRunner.procentage);
        ShowHUD("<R>: Random filled");
    }

    public void OnClear()
    {
        GridManager.ClearAll();
        ShowHUD("<C>: Cleared all");
    }
    
    public void OnToggleMode()
    {
        if (!GameModeManager) return;
        GameModeManager.ToggleMode();
        ShowHUD(GameModeManager.CurrentMode == GameModeManager.GameMode.PvP ? "PvP" : "Classic");
    }
    
    void TogglePanel()
    {
        if (!BottomPanel) return;
        bool flagVisible = BottomPanel.alpha < 0.9f;
        BottomPanel.alpha = flagVisible ? 1f : 0f;
        BottomPanel.interactable = flagVisible;
        BottomPanel.blocksRaycasts = flagVisible;
        
        if(flagVisible) ShowHUD("<Tab>: Show menu"); else ShowHUD("<Tab>: Hide menu");
    }
    

    void ChangeSpeed(float percent)
    {
        float koef = 1f + percent;
        LifeRunner.stepDelay = Mathf.Clamp(koef * LifeRunner.stepDelay, minDelay, maxDelay);
        Slider.value = LifeRunner.stepDelay;
        ShowHUD($"Speed: {1f / LifeRunner.stepDelay:0.0} step/s");
    }
    
    public void ShowHUD(string msg, bool hideAfterTimer = true)
    {
        Debug.Log($"ShowHUD: {msg}");
        HudText.text = msg;
        if (hideAfterTimer) timer = showSec;
    }
    
    public void NextPattern()
    {
        if (!MatchController) return;
        int i = (MatchController.selectedPattern + 1) % MatchController.patternNames.Length;
        MatchController.SetPatternIndex(i);
        if (MatchController) ShowHUD($"Pattern: {MatchController.patternNames[i]}");
    }
    public void PrevPattern()
    {
        if (!MatchController) return;
        int i = (MatchController.selectedPattern - 1 + MatchController.patternNames.Length) % MatchController.patternNames.Length;
        MatchController.SetPatternIndex(i);
        if (MatchController) ShowHUD($"Pattern: {MatchController.patternNames[i]}");
    }
    
}
