using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "levelData", menuName = "ScriptableObjects/levelData",order = 1)]
public class LevelData : ScriptableObject
{
    [Header("Level")] [SerializeField] public GameObject levelObject;
    [SerializeField] private int[] bestMoves;
    public bool isPlayed = false;

    public int CalculateStars()
    {
        if (!isPlayed)
        {
            return 0;
        }
        var level = levelObject.GetComponent<GridManager>();

        int stars = 0;

        if (level.leftMoves >= bestMoves[2])
        {
            stars = 3;
        }
        else if (level.leftMoves >= bestMoves[1])
        {
            stars = 2;
        }
        else if (level.leftMoves >= bestMoves[0])
        {
            stars = 1;
        }

        return stars;
    }

}
