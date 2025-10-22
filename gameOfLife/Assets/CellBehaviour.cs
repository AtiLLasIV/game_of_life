using System.Collections;
using UnityEngine;

public class CellBehaviour : MonoBehaviour
{
    public enum Owner { None, P1, P2 }
    public Owner owner = Owner.None;
    
    public bool isAlive;
    
    public Color deadColor = Color.aquamarine;
    public Color aliveColor = Color.white;
    
    public Color p1Color = Color.darkBlue;
    public Color p2Color = Color.darkRed;
    
    public float animTime = 0.55f;
    
    Coroutine anim;

    private SpriteRenderer _fill;

    void Awake()
    {
        var fillTr = transform.Find("Fill");
        _fill = fillTr.GetComponent<SpriteRenderer>();
        UpdateVisual();
    }

    void OnMouseDown()
    {
        var gm = FindObjectOfType<GameModeManager>();
        bool pvp = gm && gm.CurrentMode == GameModeManager.GameMode.PvP;
        var mc = MatchController.Instance;
        
        if (pvp)
        {
            if (mc != null && mc.IsPlacementActive)
                mc.OnCellClicked(this);
            return;
        }
        
        if (mc != null && mc.patternNames != null && mc.patternNames.Length > 0)
        {
            string pat = mc.patternNames[Mathf.Clamp(mc.selectedPattern, 0, mc.patternNames.Length - 1)];
            if (pat != "Single")
            {
                mc.PlacePatternClassic(this);
                return;
            }
        }
        SetAlive(!isAlive, Owner.None);
    }

    public void SetAlive(bool alive, Owner newOwner = Owner.None)
    {
        bool changed = (isAlive != alive) || (alive && owner != newOwner);
        Color from = _fill ? _fill.color : Color.white;

        isAlive = alive;
        owner   = alive ? newOwner : Owner.None;

        Color to = TargetColor(isAlive, owner);

        if (anim != null) StopCoroutine(anim);
        if (changed) anim = StartCoroutine(FadeColor(from, to, isAlive));
        else UpdateVisual();
    }
    
    Color TargetColor(bool aliveState, Owner who)
    {
        if (!aliveState) return deadColor;
        if (who == Owner.None) return aliveColor;
        return who == Owner.P1 ? p1Color : p2Color;
    }
    
    IEnumerator FadeColor(Color from, Color to, bool born)
    {
        float dur = animTime;

        if (born)
        {
            to.a = 0.8f;
            from = to;
            from.a = 0f;
        }
        else
        {
            to = new Color(deadColor.r, deadColor.g, deadColor.b, 1f);
            from.a = 1f;
        }

        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / dur);
            if (_fill) _fill.color = Color.Lerp(from, to, k);
            yield return null;
        }
        if (_fill) _fill.color = to;
    }
    
    void UpdateVisual()
    {
        if (!isAlive)
        {
            _fill.color = deadColor;
            return;
        }

        if (owner == Owner.None)
        {
            _fill.color = aliveColor;
            return;
        }
        _fill.color = owner == Owner.P1 ? p1Color : p2Color;
    }
    
    
}
