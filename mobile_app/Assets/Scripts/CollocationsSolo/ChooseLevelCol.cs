using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChooseLevelCol : MonoBehaviour
{
    public Text firstNumberText;
    public Text secondNumberText;
    public Text thirdNumberText;
    public Text helpText;

    private int firstNumber;
    private int secondNumber;
    private int thirdNumber;

    private int maxLevels = GameSettings.SYNONYM_MAX_LEVELS;

    // Use this for initialization
    void Awake()
    {
        firstNumber = Random.Range(0, 5);
        secondNumber = Random.Range(0, 10);
        thirdNumber = Random.Range(0, 10);

        //firstNumber = 0;
        //secondNumber = 0;
        //thirdNumber = 1;

        firstNumberText.text = firstNumber.ToString();
        secondNumberText.text = secondNumber.ToString();
        thirdNumberText.text = thirdNumber.ToString();

        //helpText.text = "Izbrana je stopnja " + firstNumber.ToString() + secondNumber.ToString() + thirdNumber.ToString();
        helpText.text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_SOLO_PICK_LEVEL_HELPER").Replace("{{LEVEL}}", firstNumber.ToString() + secondNumber.ToString() + thirdNumber.ToString());
    }

    void Start()
    {

    }

    
    void Update()
    {

    }

    public void ChangeCounterFirst(int action)
    {
        firstNumber = firstNumber + action;

        if (firstNumber > 5) {
            firstNumber = 5;
        }

        if (firstNumber < 0)
        {
            firstNumber = 0;
        }

        checkMax();
    }

    public void ChangeCounterSecond(int action)
    {
        secondNumber = secondNumber + action;

        if (secondNumber > 9) {
            secondNumber = 0;
            //ChangeCounterFirst(1);
        }

        if (secondNumber < 0)
        {
            secondNumber = 9;
            //ChangeCounterFirst(-1);
        }

        checkMax();
    }

    public void ChangeCounterThird(int action)
    {
        thirdNumber = thirdNumber + action;

        if (thirdNumber > 9)
        {
            thirdNumber = 0;
            //ChangeCounterSecond(1);
        }

        if (thirdNumber < 0)
        {
            thirdNumber = 9;
            //ChangeCounterSecond(-1);
        }

        checkMax();
    }

    private void checkMax() {
        if (GetLevelNumber() > maxLevels) {
            firstNumber = 5;
            secondNumber = 0;
            thirdNumber = 0;
        }

        if (firstNumber == 0 && secondNumber == 0 && thirdNumber == 0) {
            thirdNumber = 1;
        }

        firstNumberText.text = firstNumber.ToString();
        secondNumberText.text = secondNumber.ToString();
        thirdNumberText.text = thirdNumber.ToString();
        //helpText.text = "Izbrana je stopnja " + firstNumber.ToString() + secondNumber.ToString() + thirdNumber.ToString();
        helpText.text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_SOLO_PICK_LEVEL_HELPER").Replace("{{LEVEL}}", firstNumber.ToString() + secondNumber.ToString() + thirdNumber.ToString());
    }

    private int GetLevelNumber()
    {
        return firstNumber * 100 + secondNumber * 10 + thirdNumber;
    }

        public void PlayLevel()
    {

        GameInfoCollocation.info.currentLevel = GetLevelNumber();
        SceneSwitcher.LoadScene2(GameSettings.SCENE_FILLER_LEVEL_COL);

    }


    private void OnApplicationPause(bool pause)
    {


    }
}
