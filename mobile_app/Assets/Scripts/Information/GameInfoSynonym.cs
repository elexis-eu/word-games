using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GameInfoSynonym
{
    public static SynonymInfo info = new SynonymInfo();
    public static SynonymInfo infoCurrent = new SynonymInfo();
    public static ReceiveSynonym rec = new ReceiveSynonym();
    public static int currentRound;
    public static float timeLeft;
    public static string[] chosenWords;
    public static int score;
    public static long currentRoundTimeLeft;

    public static void SetRoundStartInfo(string infoR, bool current)
    {
        SynonymInfo temp = JsonUtility.FromJson<SynonymInfo>(infoR); ;
        if (!current)
        {
            info = temp;
        }
        else
        {
            infoCurrent = temp;
        }

        // interpolation for internet erros
        currentRound = info.next_round - 1;

        timeLeft = 10;
        currentRoundTimeLeft = 0;
        chosenWords = new string[3];
        score = 0;

        temp.max_round_score = 3 * temp.scoring[0];

        // hacked stuff
        temp.collecting_results_duration_ms = 0;

        GameSettings.MyDebug(temp.player_id);
        //GameSettings.MyDebug(GameSettings.user);
        if (temp.player_id != null)
        {
            if (GameSettings.user == null) {
                GameSettings.MyDebug("Setting player: "+temp.player_id);
                GameSettings.username = temp.player_id;
            }
        }
    }

    public static void SetRoundOverInfo(string infoR)
    {
        rec = JsonUtility.FromJson<ReceiveSynonym>(infoR);
    }

    public static void SetNewRound()
    {
        currentRound++;
    }
}

[System.Serializable]
public class SynonymInfo
{
    public int max_round_score;
    public int next_round;
    public string game_type;
    public long game_start;
    public long current_time;
    public int max_select;
    public int number_of_rounds;
    public int buttons_number;
    public int round_duration_ms;
    public int round_pause_duration_ms;
    public int collecting_results_duration_ms;
    public int show_results_duration_ms;
    public string cycle_id;
    public SynonymWords[] words;
    public int[] scoring;
    public long connect_immediate_lock_time;
    public string player_id;
}

[System.Serializable]
public class SynonymWords
{
    public string word;
    public int position;
    public string structure_text;
    public SynonymButtons[] buttons;
}

[System.Serializable]
public class SynonymButtons
{
    public string word;
    public int group_position;
    public int score;
}





[System.Serializable]
public class ReceiveSynonym
{
    public int code;
    public ReceiveDataSynonym[] data = new ReceiveDataSynonym[1];
}

[System.Serializable]
public class ReceiveDataSynonym
{
    public string word;
    public string[] buttons = new string[3 /*this is max cause of graphics currently*/];
    public ReceiveSynonymScores[] scores = new ReceiveSynonymScores[3];
}

[System.Serializable]
public class ReceiveSynonymScores
{
    public string word;
    public int score;
}