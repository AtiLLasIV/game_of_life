using System.Collections;
using UnityEngine;

public class LifeRunner : MonoBehaviour
{
    public GridManager gridManager;
    public GameModeManager gameModeManager;
    
    public int scoreP1 { get; private set; }
    public int scoreP2 { get; private set; }
    
    public float stepDelay = 0.15f;
    public float procentage = 0.2f;
    
    private bool running;
    private Coroutine loop;

    public void ResetScores()
    {
        scoreP1 = 0;
        scoreP2 = 0;
    }

    public bool Toggle()
    {
        if (running)
        {
            Pause();
            return true;
        }
        else
        {
            Play();
            return false;
        }
    }

    public void Play()
    {
        if (running) return;
        running = true;
        loop = StartCoroutine(Loop());
    }

    public void Pause()
    {
        running = false;
        if (loop != null) StopCoroutine(loop);
    }

    IEnumerator Loop()
    {
        while (running)
        {
            StepOnce();
            yield return new WaitForSeconds(stepDelay);
        }
    }

    public void StepOnce()
    {
        bool pvp = (gameModeManager && gameModeManager.CurrentMode == GameModeManager.GameMode.PvP);

        int width  = gridManager.width;
        int height = gridManager.height;

        var nextAlive = new bool[width, height];
        var nextOwner = new CellBehaviour.Owner[width, height];
        
        int newP1 = 0;
        int newP2 = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int aliveCount = 0;
                int numP1 = 0;
                int numP2 = 0;


                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;
                        int nx = x + dx;
                        int ny = y + dy;
                        if (nx < 0 || ny < 0 || nx >= width || ny >= height) continue;

                        var n = gridManager.cells[nx, ny];
                        if (!n || !n.isAlive) continue;

                        aliveCount++;
                        if (pvp)
                        {
                            if (n.owner == CellBehaviour.Owner.P1) numP1++;
                            else if (n.owner == CellBehaviour.Owner.P2) numP2++;
                        }
                    }
                }

                var cur   = gridManager.cells[x, y];
                bool aliveNow = cur.isAlive;

                bool willLive = aliveNow ? (aliveCount == 2 || aliveCount == 3) : (aliveCount == 3);

                nextAlive[x, y] = willLive;


                if (!willLive)
                {
                    nextOwner[x, y] = CellBehaviour.Owner.None;
                    continue;
                }

                if (!pvp)
                {
                    nextOwner[x, y] = CellBehaviour.Owner.None;
                }
                else
                {
                    if (aliveNow)
                    {
                        nextOwner[x, y] = cur.owner;
                    }
                    else
                    {

                        var bornOwner = (numP1 > numP2) ? CellBehaviour.Owner.P1 : CellBehaviour.Owner.P2;
                        nextOwner[x, y] = bornOwner;
                        if (bornOwner == CellBehaviour.Owner.P1) newP1++;
                        else newP2++;
                    }
                }
            }
        }
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridManager.cells[x, y].SetAlive(nextAlive[x, y], nextOwner[x, y]);
            }
        }
        
        if (pvp)
        {
            RecountScores();
        }
        
    }
    
    public void RecountScores()
    {
        int c1 = 0;
        int c2 = 0;
        int width  = gridManager.width;
        int height = gridManager.height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var c = gridManager.cells[x, y];
                if (!c || !c.isAlive) continue;

                if (c.owner == CellBehaviour.Owner.P1) c1++;
                else if (c.owner == CellBehaviour.Owner.P2) c2++;
            }
        }

        scoreP1 = c1;
        scoreP2 = c2;
    }
    
    
}
