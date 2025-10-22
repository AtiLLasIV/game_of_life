using TMPro;
using UnityEngine;

public class ResultUI : MonoBehaviour
{
    public CanvasGroup panel;
    public TMP_Text titleText;
    public TMP_Text bodyText;
    public LifeRunner life;
    public GameModeManager mode;

    void Awake()
    {
        if (panel) { panel.alpha = 0f; panel.interactable = false; panel.blocksRaycasts = false; }
    }

    public void Show()
    {
        if (!panel) return;

        if (titleText) titleText.text = "Result";

        int p1 = life ? life.scoreP1 : 0;
        int p2 = life ? life.scoreP2 : 0;
        string winner = (p1 == p2) ? "Draw" : (p1 > p2 ? "P1" : "P2");

        if (bodyText)
            bodyText.text = $"P1: {p1}\nP2: {p2}\n \nWinner: {winner}";

        panel.alpha = 1f;
        panel.interactable = true;
        panel.blocksRaycasts = true;
    }

    public void Hide()
    {
        if (!panel) return;
        panel.alpha = 0f;
        panel.interactable = false;
        panel.blocksRaycasts = false;
    }
}
