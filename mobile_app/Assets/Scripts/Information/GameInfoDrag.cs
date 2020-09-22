using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class GameInfoDrag
{
    public static DragInfo info = new DragInfo();
    public static DragInfo infoCurrent = new DragInfo();
    public static int currentRound;
    public static float timeLeft;
    public static int score;
    public static int correct_subsequently = 0;
    public static int correct_subsequently_max = 0;
    public static List<int> chosenButtonsScores;
    public static List<int> chosenButtonsBonus;
    public static List<string> chosenButtonsNames;
    public static List<int> chosenButtonsGroups;
    public static float currentRoundTimeLeft;

    public static void SetRoundStartInfo(string infoR, bool current)
    {
        DragInfo temp = JsonUtility.FromJson<DragInfo>(infoR); ;
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
        correct_subsequently = 0;
        correct_subsequently_max = 0;

        temp.max_round_score = temp.buttons_number * temp.scoring[0] + temp.scoring[0];

        for (int i = 0; i < info.words.Length; i++)
        {
            Array.Sort(info.words[i].buttons,
                delegate (DragButtons x, DragButtons y) { return -x.word.CompareTo(y.word); });

            /*for (int j = 0; j < 3; j++)
            {
                GameSettings.MyDebug(i + ": " + info.words[i].buttons[j].word);
            }*/
        }

        chosenButtonsScores = new List<int>();
        chosenButtonsNames = new List<string>();
        chosenButtonsGroups = new List<int>();
        chosenButtonsBonus = new List<int>();

        temp.round_pause_duration_ms = temp.collecting_results_duration_ms;
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

    public static void SetNewRound()
    {
        currentRound++;
    }
}

[System.Serializable]
public class DragInfo
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
    public DragWords[] words;
    public int[] scoring;
    public long connect_immediate_lock_time;
    public string player_id;
    public int bonus_condition;
    public int bonus_condition_points;
    public int double_points_round;
    public string log_session;
}

[System.Serializable]
public class DragWords
{
    public string word;
    public int position;
    public string structure_text;
    public DragButtons[] buttons;
}


[System.Serializable]
public class DragButtons
{
    public string word;
    public int group_position;
    public int score;
    public int collocation_id;
}