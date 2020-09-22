using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class GameInfoChoose
{
    public static ChooseInfo info = new ChooseInfo();
    public static ChooseInfo infoCurrent = new ChooseInfo();
    public static int currentRound;
    public static ChooseButtons[,] selectedButtons;
    public static float timeLeft;
    public static List<ChooseButtons> chosenButtons;
    public static List<ChooseButtons> tempList;
    public static List<ChooseButtons> classyfiedBtns;
    public static long currentRoundTimeLeft;

    public static void SetRoundStartInfo(string infoR, bool current)
    {
        ChooseInfo temp = JsonUtility.FromJson<ChooseInfo>(infoR); ;
        if (!current)
        {
            info = temp;
        }
        else
        {
            infoCurrent = temp;
        }
        selectedButtons = new ChooseButtons[info.number_of_rounds, 3];

        GameSettings.MyDebug("Bonus: " + temp.bonus_points);
        temp.max_round_score = 3 * temp.scoring[0] + temp.bonus_points;
        GameSettings.MyDebug("Max: "+temp.max_round_score);

        for (int i = 0; i < info.words.Length; i++)
        {
            Array.Sort(info.words[i].buttons,
                delegate (ChooseButtons x, ChooseButtons y) { return x.group.CompareTo(y.group); });

            /*for (int j = 0; j < 3; j++)
            {
                GameSettings.MyDebug(info.words[i].buttons[j].word + ": " + info.words[i].buttons[j].score + ": " + info.words[i].buttons[j].group + ": " + info.words[i].buttons[j].choose_position);
            }*/
        }
        
        string json = JsonUtility.ToJson(info);
        GameSettings.MyDebug(json);

        timeLeft = 0;
        currentRoundTimeLeft = 0;


        // interpolation for internet erros
        currentRound = info.next_round - 1;

        // hacked stuff
        temp.collecting_results_duration_ms = 0;

        /*GameSettings.MyDebug(temp.player_id);
        GameSettings.MyDebug(GameSettings.user);
        if (temp.player_id != null)
        {
            if (GameSettings.user == null)
                GameSettings.username = temp.player_id;
        }*/

        chosenButtons = new List<ChooseButtons>();
    }

    public static void SetNewRound()
    {
        currentRound++;
        selectedButtons = new ChooseButtons[info.number_of_rounds, 3];
        chosenButtons = new List<ChooseButtons>();
        tempList = new List<ChooseButtons>();
        classyfiedBtns = new List<ChooseButtons>();
    }
}

[System.Serializable]
public class ChooseInfo
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
    public ChooseWords[] words;
    public int[] scoring;
    public int bonus_points;
    public long connect_immediate_lock_time;
    public string player_id;
    public string log_session;
}

[System.Serializable]
public class ChooseWords
{
    public string word;
    public int position;
    public string structure_text;
    public ChooseButtons[] buttons;
}

[System.Serializable]
public class ChooseButtons
{
    public string word;
    public int group;
    public int score;
    public int choose_position;
    public int stars;
    public int collocation_id;
}