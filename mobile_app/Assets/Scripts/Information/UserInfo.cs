using UnityEngine;

// choosermation about scoreboard shared across scenes

public static class UserInfo
{
    public static string userToken;

    public static Scoreboard info = new Scoreboard(); 

    public static void SetScoreboardInfo(string chooseR)
    {
        info = JsonUtility.FromJson<Scoreboard>(chooseR);
    }

    [System.Serializable]
    public class Scoreboard
    {
        public Player[] scoreboard;
    }

    [System.Serializable]
    public class Player
    {
        public string player_id;
        public int score;
        public int position;
    }
}
