using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Levels", menuName = "Levels")]
public class Levels : ScriptableObject
{
    public static Levels instance;

    public List<LevelData> levels;

    public static Levels Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load("Levels") as Levels;
            }

            return instance;
        }
    }
}