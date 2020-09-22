using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChooseChoose : MonoBehaviour
{
    public GameObject objTimer;
    private TimerUI scrTimer;

    public GameObject choice1Obj, choice2Obj, choice3Obj;
    private Text choice1Text, choice2Text, choice3Text;

    public GameObject previewWordObj;
    private PreviewWordPrefab scrPrWPre;

    private int roundOfRound;

    // Use this for initialization
    void Awake()
    {
        if (GameSettings.SOLO)
        {
            objTimer.SetActive(false);
        }

        scrTimer = objTimer.GetComponent<TimerUI>();

        scrPrWPre = previewWordObj.GetComponent<PreviewWordPrefab>();

        choice1Text = choice1Obj.GetComponentInChildren<Text>();
        choice2Text = choice2Obj.GetComponentInChildren<Text>();
        choice3Text = choice3Obj.GetComponentInChildren<Text>();

        roundOfRound = -1;
    }

    private void Start()
    {
        scrTimer.Activate(GameInfoChoose.timeLeft);

        if (GameInfoChoose.info.words[GameInfoChoose.currentRound].position == 1)
        {
            scrPrWPre.wordText.text = GameInfoChoose.info.words[GameInfoChoose.currentRound].word + " + ";
            scrPrWPre.leftImageObj.SetActive(false);
        }
        else
        {
            scrPrWPre.wordText.text = " + " + GameInfoChoose.info.words[GameInfoChoose.currentRound].word;
            scrPrWPre.rightImageObj.SetActive(false);
        }

        CreateRoundOfRound();
    }

    void CreateRoundOfRound()
    {
        roundOfRound++;

        if (roundOfRound > 2)
        {
            ContinueToClassifying();
            return;
        }


        CreateButtons();
    }

    void CreateButtons()
    {
        choice1Text.text = GameInfoChoose.info.words[GameInfoChoose.currentRound].buttons[3 * roundOfRound + 0].word;
        choice2Text.text = GameInfoChoose.info.words[GameInfoChoose.currentRound].buttons[3 * roundOfRound + 1].word;
        choice3Text.text = GameInfoChoose.info.words[GameInfoChoose.currentRound].buttons[3 * roundOfRound + 2].word;
    }

    public void Choice1ButtonClicked()
    {
        GameInfoChoose.selectedButtons[GameInfoChoose.currentRound, roundOfRound] = (GameInfoChoose.info.words[GameInfoChoose.currentRound].buttons[3*roundOfRound + 0]);
        GameInfoChoose.selectedButtons[GameInfoChoose.currentRound, roundOfRound].group = -1;
        GameInfoChoose.chosenButtons.Add((GameInfoChoose.info.words[GameInfoChoose.currentRound].buttons[3 * roundOfRound + 0]));
        WordChoose();
    }

    public void Choice2ButtonClicked()
    {
        GameInfoChoose.selectedButtons[GameInfoChoose.currentRound, roundOfRound] = (GameInfoChoose.info.words[GameInfoChoose.currentRound].buttons[3*roundOfRound + 1]);
        GameInfoChoose.selectedButtons[GameInfoChoose.currentRound, roundOfRound].group = -1;
        GameInfoChoose.chosenButtons.Add((GameInfoChoose.info.words[GameInfoChoose.currentRound].buttons[3 * roundOfRound + 1]));
        WordChoose();
    }

    public void Choice3ButtonClicked()
    {
        GameInfoChoose.selectedButtons[GameInfoChoose.currentRound, roundOfRound] = (GameInfoChoose.info.words[GameInfoChoose.currentRound].buttons[3*roundOfRound + 2]);
        GameInfoChoose.selectedButtons[GameInfoChoose.currentRound, roundOfRound].group = -1;
        GameInfoChoose.chosenButtons.Add((GameInfoChoose.info.words[GameInfoChoose.currentRound].buttons[3 * roundOfRound + 2]));
        WordChoose();
    }

    void WordChoose()
    {
        CreateRoundOfRound();
    }

    void ContinueToClassifying()
    {
        GameInfoChoose.timeLeft = scrTimer.GetTimeLeft();
        SceneSwitcher.LoadScene2(GameSettings.CLASSIFY_INFO_CHOOSE);
    }

    void Update()
    {
        if (!GameSettings.SOLO && scrTimer.time <= 0)
        {
            SceneSwitcher.LoadScene2(GameSettings.ROUND_SCORE_CHOOSE);
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            SceneSwitcher.LoadScene2Back2(GameSettings.COMPETITIVE_MODE_SELECTION_MENU);
        }
    }
}
