using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClassifyInfoChoose : MonoBehaviour
{
    public GameObject objTimer;
    private TimerUI scrTimer;

    public GameObject choice1Obj, choice2Obj, choice3Obj;
    private Text choice1Text, choice2Text, choice3Text;

    public GameObject additionalInfoObj;
    private Text additionalInfoText;

    private string text = "Izbrane 3 besede razvrsti od 1. do 3. mesta.";

    // Use this for initialization
    void Awake()
    {
        if (GameSettings.SOLO)
        {
            objTimer.SetActive(false);
        }

        scrTimer = objTimer.GetComponent<TimerUI>();
        
        choice1Text = choice1Obj.GetComponentInChildren<Text>();
        choice2Text = choice2Obj.GetComponentInChildren<Text>();
        choice3Text = choice3Obj.GetComponentInChildren<Text>();

        additionalInfoText = additionalInfoObj.GetComponent<Text>();

        setPreviewWords();
    }

    private void Start()
    {
        if (GameSettings.THEMATIC)
            additionalInfoText.text = text;

        scrTimer.Activate(GameInfoChoose.timeLeft);
    }

    void setPreviewWords()
    {
        string previewWord = GameInfoChoose.info.words[GameInfoChoose.currentRound].word;

        int currentRound = GameInfoChoose.currentRound;

        ChooseButtons choice1 = GameInfoChoose.selectedButtons[currentRound, 0];
        ChooseButtons choice2 = GameInfoChoose.selectedButtons[currentRound, 1];
        ChooseButtons choice3 = GameInfoChoose.selectedButtons[currentRound, 2];

        int wordPosition = GameInfoChoose.info.words[currentRound].position;

        if (choice1 != null)
            choice1Text.text = wordPosition == 1 ? previewWord + " " + choice1.word:
                                                   choice1.word + " " + previewWord;                         
        if (choice2 != null)
            choice2Text.text = wordPosition == 1 ? previewWord + " " + choice2.word :
                                                   choice2.word + " " + previewWord;
        if (choice3 != null)
            choice3Text.text = wordPosition == 1 ? previewWord + " " + choice3.word :
                                                   choice3.word + " " + previewWord;
    }

    public void GoToClassify()
    {
        GameInfoChoose.timeLeft = scrTimer.GetTimeLeft();
        SceneSwitcher.LoadScene2(GameSettings.CLASSIFY_CHOOSE);
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
            SceneSwitcher.LoadScene2(GameSettings.COMPETITIVE_MODE_SELECTION_MENU);
        }
    }
}
