using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GameInfoInsert
{
    public static InsertInfo info = new InsertInfo();
    public static InsertInfo infoCurrent = new InsertInfo();
    public static ReceiveInsert rec = new ReceiveInsert();
    public static int currentRound;
    public static float timeLeft;
    public static string[] chosenWords;
    public static int score;
    public static long currentRoundTimeLeft;

    public static void SetRoundStartInfo(string infoR, bool current)
    {
        InsertInfo temp = JsonUtility.FromJson<InsertInfo>(infoR); ;
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

        timeLeft = 0;
        currentRoundTimeLeft = 0;
        chosenWords = new string[3];
        score = 0;

        temp.max_round_score = 3 * temp.scoring[0];

        // hacked stuff
        temp.collecting_results_duration_ms = 0;

        /*GameSettings.MyDebug(temp.player_id);
        GameSettings.MyDebug(GameSettings.user);
        if (temp.player_id != null)
        {
            if (GameSettings.user == null)
                GameSettings.username = temp.player_id;
        }*/
    }

    public static void SetRoundOverInfo(string infoR)
    {
        rec = JsonUtility.FromJson<ReceiveInsert>(infoR);
    }

    public static void SetNewRound()
    {
        currentRound++;
    }
}

[System.Serializable]
public class InsertInfo
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
    public InsertWords[] words;
    public int[] scoring;
    public long connect_immediate_lock_time;
    public string player_id;
}

[System.Serializable]
public class InsertWords
{
    public string word;
    public int position;
    public string structure_text;
    public InsertButtons[] buttons;
}

[System.Serializable]
public class InsertButtons
{
    public string word;
    public int group_position;
    public int score;
}





[System.Serializable]
public class ReceiveInsert
{
    public int code;
    public ReceiveDataInsert[] data = new ReceiveDataInsert[1];
}

[System.Serializable]
public class ReceiveDataInsert
{
    public string word;
    public string[] buttons = new string[3 /*this is max cause of graphics currently*/];
    public ReceiveInsertScores[] scores = new ReceiveInsertScores[3];
}

[System.Serializable]
public class ReceiveInsertScores
{
    public string word;
    public int score;
}