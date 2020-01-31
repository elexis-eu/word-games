using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;

public class RoomPicker : MonoBehaviour
{
    public GameObject room1;
    public GameObject room2;
    public GameObject room3;
    public GameObject room4;
    public GameObject room5;
    public GameObject room6;
    public GameObject room7;
    public GameObject room8;
    public GameObject room9;
    public GameObject room10;
    public GameObject endRoomButton;
    public GameObject forwardRoomButton;

    public Text firstNumber;
    public Text secondNumber;
    public Text thirdNumber;

    private Synonym2Words currentWord;
    private LevelInfo levelInfo;

    private Dictionary<int, RoomWord> loadedRooms;

    // Use this for initialization
    void Awake()
    {

        int currentLevel = GameInfoSynonym2.info.currentLevel;
        int first = currentLevel / 100;
        int second = (currentLevel / 10) % 10;
        int third = currentLevel % 10;
        firstNumber.text = first.ToString();
        secondNumber.text = second.ToString();
        thirdNumber.text = third.ToString();
        room1.SetActive(false);
        room2.SetActive(false);
        room3.SetActive(false);
        room4.SetActive(false);
        room5.SetActive(false);
        room6.SetActive(false);
        room7.SetActive(false);
        room8.SetActive(false);
        room9.SetActive(false);
        room10.SetActive(false);
    }

    void Start()
    {
        GetLevelInfoQuery(GameInfoSynonym2.info.currentLevel);
    }

    
    void Update()
    {

    }

    private void OnApplicationPause(bool pause)
    {

    }

    GameObject GetChildWithName(GameObject obj, string name)
    {
        Transform trans = obj.transform;
        Transform childTrans = trans.Find(name);
        if (childTrans != null)
        {
            return childTrans.gameObject;
        }
        else
        {
            return null;
        }
    }

    public void roomSelected(int roomNumber)
    {
        RoomWord roomWordSelect = this.loadedRooms[roomNumber];

        GameInfoSynonym2.currentGame.currentWord = roomWordSelect.wordText;
        GameInfoSynonym2.currentGame.currentWordID = roomWordSelect.wordID;

        SceneSwitcher.LoadScene2(GameSettings.SCENE_FILLER_WORD);

    }

    public void finishRoom()
    {
        if (GameInfoSynonym2.info.currentLevel < GameSettings.SYNONYM_MAX_LEVELS){

            //GameInfoSynonym2.info.currentLevel++;
            SceneSwitcher.LoadScene2(GameSettings.SCENE_LEVEL_SCORE);

        } else {

            SceneSwitcher.LoadScene2(GameSettings.SCENE_GAME_MODE);
        }
       
    }

    public void checkRoom(GameObject selectedRoom) {

        selectedRoom.SetActive(true);

        if (this.currentWord.score >= 0)
        {
            GameInfoSynonym2.info.wordsCompleted = GameInfoSynonym2.info.wordsCompleted + 1;

            GameObject RoomNumber = GetChildWithName(selectedRoom, "RoomNumber");
            RoomNumber.SetActive(false);

            GameObject RoomWord = GetChildWithName(selectedRoom, "RoomWord");
            Text RoomText = RoomWord.GetComponent<Text>();
            RoomText.text = this.currentWord.text;
            RoomWord.SetActive(true);

            GameObject Stars3 = GetChildWithName(selectedRoom, "Stars3");

            if (this.currentWord.score == 0)
            {
                GameObject Stars = GetChildWithName(Stars3, "Stars03");
                Stars3.SetActive(true);
                Stars.SetActive(true);
            }

            if (this.currentWord.score == 1) {
                GameObject Stars = GetChildWithName(Stars3, "Stars13");
                Stars3.SetActive(true);
                Stars.SetActive(true);
            }

            if (this.currentWord.score == 2)
            {
                GameObject Stars = GetChildWithName(Stars3, "Stars23");
                Stars3.SetActive(true);
                Stars.SetActive(true);
            }

            if (this.currentWord.score == 3)
            {
                GameObject Stars = GetChildWithName(Stars3, "Stars33");
                Stars3.SetActive(true);
                Stars.SetActive(true);
            }

        }
    }

    public void checkEndRoom()
    {
        if (GameInfoSynonym2.info.wordsCompleted >= 10) {
            /*
             * if (GameInfoSynonym2.info.gameMode == "campaign") {
                endRoomButton.SetActive(true);
            } else
            {
                forwardRoomButton.SetActive(true);
            }
            */
            endRoomButton.SetActive(true);
        }
    }

    public void parseLevelInfo() {

        GameInfoSynonym2.info.wordsCompleted = 0;
        this.loadedRooms = new Dictionary<int, RoomWord>();

        foreach (Synonym2Words word in levelInfo.words)
        {
            RoomWord RoomWordData = new RoomWord();
            RoomWordData.wordID = word.linguistic_unit_id;
            RoomWordData.wordText = word.text;

            this.loadedRooms.Add(word.position, RoomWordData);
            this.currentWord = word;

            if (word.position == 1)
            {
                this.checkRoom(room1);
            }

            if (word.position == 2)
            {
                this.checkRoom(room2);
            }

            if (word.position == 3)
            {
                this.checkRoom(room3);
            }

            if (word.position == 4)
            {
                this.checkRoom(room4);
            }

            if (word.position == 5)
            {
                this.checkRoom(room5);
            }

            if (word.position == 6)
            {
                this.checkRoom(room6);
            }

            if (word.position == 7)
            {
                this.checkRoom(room7);
            }

            if (word.position == 8)
            {
                this.checkRoom(room8);
            }

            if (word.position == 9)
            {
                this.checkRoom(room9);
            }

            if (word.position == 10)
            {
                this.checkRoom(room10);
            }

        }

        this.checkEndRoom();
    }

    public void GetLevelInfoQuery(int levelId)
    {
        string url = GameSettings.GETLevelInfoURL + GameSettings.GetUserFBToken() + "&level_id=" + levelId.ToString() + "&type=" + GameInfoSynonym2.info.gameMode;

        GameSettings.MyDebug(url);

        StartCoroutine(GetRequest(url));
    }

    IEnumerator GetRequest(string url)
    {

        UnityWebRequest uwr = UnityWebRequest.Get(url);

        uwr.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();

        uwr.timeout = 10;
        yield return uwr.SendWebRequest();

        if (uwr.isHttpError || uwr.isNetworkError)
        {
            GameSettings.MyDebug("Error While Sending: " + uwr.error);
            //SceneSwitcher.LoadScene2(GameSettings.MENU_LEADERBOARDS);
        }
        else
        {
            GameSettings.MyDebug("Received: " + uwr.downloadHandler.text);
            levelInfo = JsonUtility.FromJson<LevelInfo>(uwr.downloadHandler.text);

            parseLevelInfo();
        }
    }

    [System.Serializable]
    class Synonym2Words
    {
        public int linguistic_unit_id;
        public string text;
        public int score;
        public int position;
    }

    [System.Serializable]
    class LevelInfo
    {
        public Synonym2Words[] words = new Synonym2Words[10];
    }

    class RoomWord {
        public int wordID;
        public string wordText;
    }

}