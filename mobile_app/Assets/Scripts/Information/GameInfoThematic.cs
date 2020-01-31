using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class GameInfoThematic
{
    public static ThematicInfo info = new ThematicInfo();

    public static void SetRoundStartInfo(string infoR, bool current)
    {
        info = JsonUtility.FromJson<ThematicInfo>(infoR);

        int gameThinksRounds = GameSettings.numberOfPlayedRounds;
        int serverRealRounds = info.next_round;

        if (gameThinksRounds != serverRealRounds)
        {
            GameSettings.numberOfPlayedRounds = serverRealRounds;
            GameSettings.midGameThematicChange = true;
        }
    }
}

[System.Serializable]
public class ThematicInfo
{
    public string game_type;
    public int next_round;
    public int number_of_rounds;
    public string thematic_name;
    public long start_of_thematic;
    public long current_time;
    public long end_of_thematic;
}