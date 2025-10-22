using UnityEngine;
using System.Collections.Generic;

public static class PatternLibrary
{
    public static readonly Dictionary<string, Vector2Int[]> Patterns = new()
    {
        { "Single", new []{ new Vector2Int(0,0) } },

        { "Glider", new []{
            new Vector2Int(0,1), new Vector2Int(1,2), new Vector2Int(2,0),
            new Vector2Int(2,1), new Vector2Int(2,2)
        }},

        { "Blinker", new []{
            new Vector2Int(-1,0), new Vector2Int(0,0), new Vector2Int(1,0)
        }},

        { "Beacon", new []{
            new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(0,1),
            new Vector2Int(3,2), new Vector2Int(2,3), new Vector2Int(3,3)
        }},
        
        { "Gun", new []{
            new Vector2Int(0,4), new Vector2Int(0,5),
            new Vector2Int(1,4), new Vector2Int(1,5),
            new Vector2Int(10,4), new Vector2Int(10,5), new Vector2Int(10,6),
            new Vector2Int(11,3), new Vector2Int(11,7),
            new Vector2Int(12,2), new Vector2Int(12,8),
            new Vector2Int(13,2), new Vector2Int(13,8),
            new Vector2Int(14,5),
            new Vector2Int(15,3), new Vector2Int(15,7),
            new Vector2Int(16,4), new Vector2Int(16,5), new Vector2Int(16,6),
            new Vector2Int(17,5),
            new Vector2Int(20,2), new Vector2Int(20,3), new Vector2Int(20,4),
            new Vector2Int(21,2), new Vector2Int(21,3), new Vector2Int(21,4),
            new Vector2Int(22,1), new Vector2Int(22,5),
            new Vector2Int(24,0), new Vector2Int(24,1), new Vector2Int(24,5), new Vector2Int(24,6),
            new Vector2Int(34,2), new Vector2Int(34,3),
            new Vector2Int(35,2), new Vector2Int(35,3)
        }},
    };

    public static Vector2Int[] Get(string name) =>
        Patterns.TryGetValue(name, out var arr) ? arr : Patterns["Single"];
}
