using System;
using UnityEngine;

// Information about game that is shared accross a number of scenes
public static class GameInfolegacy
{
    public static ChooseInfo choose = new ChooseInfo();
    public static InsertInfo insert = new InsertInfo();
    public static DragInfo drag = new DragInfo();

    public static void SetRoundStartInfoForChoose(string info)
    {
        choose = JsonUtility.FromJson<ChooseInfo>(info);
    }

    public static void SetRoundStartInfoForInsert(string info)
    {
        insert = JsonUtility.FromJson<InsertInfo>(info);
    }

    public static void SetRoundStartInfoForDrag(string info)
    {
        drag = JsonUtility.FromJson<DragInfo>(info);

        for (int i = 0; i < drag.words.Length; i++)
        {
            Array.Sort(drag.words[i].buttons,
                delegate (DragButtons x, DragButtons y) { return -x.word.CompareTo(y.word); });

            /*for (int j = 0; j < 3; j++)
            {
                GameSettings.MyDebug(i + ": " + drag.words[i].buttons[j].word);
            }*/
        }
    }

    /* Game mode - CHOOSE information */
    /* ----------------------------------------------------- */
    [System.Serializable]
    public class ChooseInfo
    {
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
    }

    [System.Serializable]
    public class ChooseWords
    {
        public string word;
        public int position;
        public ChooseButtons[] buttons;
    }

    [System.Serializable]
    public class ChooseButtons
    {
        public string word;
        public int group_position;
        public int score;
    }
    /* ----------------------------------------------------- */

    /* Game mode - INSERT information */
    /* ----------------------------------------------------- */
    [System.Serializable]
    public class InsertInfo
    {
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
    }

    [System.Serializable]
    public class InsertWords
    {
        public string word;
        public int position;
        public InsertButtons[] buttons;
    }

    [System.Serializable]
    public class InsertButtons
    {
        public string word;
        public int group_position;
        public int score;
    }
    /* ----------------------------------------------------- */

    /* Game mode - DRAG information */
    /* ----------------------------------------------------- */
    [System.Serializable]
    public class DragInfo
    {
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
    }

    [System.Serializable]
    public class DragWords
    {
        public string word;
        public int position;
        public DragButtons[] buttons;
    }

    [System.Serializable]
    public class DragButtons
    {
        public string word;
        public int group_position;
        public int score;
    }
    /* ----------------------------------------------------- */
}
