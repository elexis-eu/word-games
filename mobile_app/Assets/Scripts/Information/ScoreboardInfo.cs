using System.Collections.Generic;
using UnityEngine;

// choosermation about scoreboard shared across scenes

public static class ScoreboardInfo
{
    public static Scoreboard info = new Scoreboard();

    // Holders if there are multiple tabs to save data for tabbing beetwen tabzz
    public static Scoreboard[] infoHolder = new Scoreboard[10];

    public static void SetScoreboardInfo(string chooseR)
    {
        info = JsonUtility.FromJson<Scoreboard>(chooseR);
    }

    public static void SetScoreboardInfoForHolders(string chooseR, int pos)
    {
        infoHolder[pos] = JsonUtility.FromJson<Scoreboard>(chooseR);
    }

    public static void DeleteScoreboardDataInHolder()
    {
        for (int i = 0; i < infoHolder.Length; i++)
        {
            infoHolder[i] = null;
        }

        info = null;
    }

    [System.Serializable]
    public class Scoreboard
    {
        public string thematic_name;
        public UserScore user_score;
        public Player[] scoreboard;
    }

    [System.Serializable]
    public class Player
    {
        public string display_name;
        public int score;
        public int position;
    }

    [System.Serializable]
    public class UserScore
    {
        public string display_name;
        public int score;
        public int position;
    }
}
