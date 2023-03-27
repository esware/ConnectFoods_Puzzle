using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Data", menuName = "ScriptableObjects/GameData",order = 1)]
public class GameData : ScriptableObject
{
    public LevelData[] levelDatas;

    private int _lastLevelIndex = 0;
    public int LastLevelIndex
    {
        get => _lastLevelIndex;
        set => _lastLevelIndex = value;
    }
}
