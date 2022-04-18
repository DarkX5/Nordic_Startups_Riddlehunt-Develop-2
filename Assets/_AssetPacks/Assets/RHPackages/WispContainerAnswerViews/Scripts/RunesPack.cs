using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRunesPack
{
    public int GetUniqueRuneCount();
    public Sprite GetRuneIcon(string id);
}

[CreateAssetMenu(fileName = "GraphicsPackage", menuName = "Riddlehouse/Graphics/RunesPackage")]
public class RunesPack : ScriptableObject, IRunesPack
{
    public RunesPack(List<string> runesIds, List<Sprite> runeIcons)
    {
        RunesIds = runesIds;
        RuneIcons = runeIcons;
    }
    
    [SerializeField] List<string> RunesIds;
    [SerializeField] private List<Sprite> RuneIcons;
    private Dictionary<string, Sprite> Runes;

    private void Initialize()
    {
        Runes = new Dictionary<string, Sprite>();
        if(RunesIds.Count != RuneIcons.Count)
            throw new ArgumentException("You need to have the same amount of Ids as Icons");
        int i = 0;
        foreach (var id in RunesIds)
        {
            Runes.Add(id, RuneIcons[i]);
            i++;
        }
    }

    public Sprite GetRuneIcon(string id)
    {
        if(Runes == null)
            Initialize();
        
        if (Runes.ContainsKey(id))
            return Runes[id];
        throw new ArgumentException("No such Rune exists: " + id);
    }

    public int GetUniqueRuneCount()
    {
        if(Runes == null)
            Initialize();
        
        return Runes.Count;
    }
}
