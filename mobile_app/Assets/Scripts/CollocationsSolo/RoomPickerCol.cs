using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;

public class RoomPickerCol : MonoBehaviour
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

    public Text levelText;
    public Text scoreText;

    public Text roomTitle;

    private CollocationRooms currentRoom;
    private LevelInfo levelInfo;

    private int levelScore = 0;
    private int next_round_limit;

    private Dictionary<int, CollocationRooms> loadedRooms;

    // Use this for initialization
    void Awake()
    {

        int currentLevel = GameInfoCollocation.info.currentLevel;
        levelText.text = currentLevel.ToString();
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

        levelScore = 0;
        scoreText.text = levelScore.ToString();
    }

    void Start()
    {
        GetLevelInfoQuery(GameInfoCollocation.info.currentLevel);
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
        CollocationRooms roomSelected = this.loadedRooms[roomNumber];

        GameInfoCollocation.currentGame.currentRoom = roomSelected.position;
        GameInfoCollocation.currentGame.currentCollocationLevelID = roomSelected.collocation_level_id;
        GameInfoCollocation.currentGame.currentGameType = roomSelected.game_type;

        SceneSwitcher.LoadScene2(GameSettings.SCENE_FILLER_WORD_COL);

    }

    public void finishRoom()
    {
        if (GameInfoCollocation.info.currentLevel < GameSettings.COLLOCATIONS_SOLO_MAX_LEVELS){

            //GameInfoCollocation.info.currentLevel++;
            SceneSwitcher.LoadScene2(GameSettings.SCENE_LEVEL_SCORE_COL);

        } else {

            SceneSwitcher.LoadScene2(GameSettings.SCENE_GAME_MODE_COL);
        }
       
    }

    public void checkRoom(GameObject selectedRoom) {

        selectedRoom.SetActive(true);

        if (this.currentRoom.score >= 0)
        {

            levelScore += this.currentRoom.score;
            scoreText.text = levelScore.ToString();

            selectedRoom.GetComponent<Image>().color = GameSettings.COLOR_BLUE;

            GameInfoCollocation.info.wordsCompleted = GameInfoCollocation.info.wordsCompleted + 1;

            GameObject RoomNumber = GetChildWithName(selectedRoom, "RoomNumber");
            RoomNumber.SetActive(false);

            GameObject RoomType = GetChildWithName(selectedRoom, "RoomType");
            Text RoomTypeText = RoomType.GetComponent<Text>();

            if (this.currentRoom.game_type == GameSettings.GAME_MODE_CHOOSE)
            {
                RoomTypeText.text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_SOLO_ROOM_PICKER_GAME_TYPE_CHOOSE");
            }
            else if (this.currentRoom.game_type == GameSettings.GAME_MODE_INSERT)
            {
                RoomTypeText.text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_SOLO_ROOM_PICKER_GAME_TYPE_INSERT");
            }
            else if (this.currentRoom.game_type == GameSettings.GAME_MODE_DRAG)
            {
                RoomTypeText.text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_SOLO_ROOM_PICKER_GAME_TYPE_DRAG");
            }
            else {
                RoomTypeText.text = this.currentRoom.game_type;
            }

            //RoomTypeText.text = this.currentRoom.game_type;
            RoomType.SetActive(true);

            GameObject RoomWord = GetChildWithName(selectedRoom, "RoomWord");
            Text RoomText = RoomWord.GetComponent<Text>();
            RoomText.text = this.currentRoom.headword1;
            RoomWord.SetActive(true);

            if (this.currentRoom.headword2 != "") {
                GameObject RoomWord2 = GetChildWithName(selectedRoom, "RoomWord2");
                Text RoomText2 = RoomWord2.GetComponent<Text>();
                RoomText2.text = this.currentRoom.headword2;
                RoomWord2.SetActive(true);
            }

            GameObject RoomScore = GetChildWithName(selectedRoom, "RoomScore");
            RoomScore.GetComponent<Text>().text = this.currentRoom.score.ToString();
            RoomScore.SetActive(true);
        }
    }

    public void setRoomTitle(string title)
    {
        roomTitle.text = title;
    }

    public void checkEndRoom()
    {
        if (GameInfoCollocation.info.wordsCompleted >= this.next_round_limit) {
            
            if (GameInfoCollocation.info.gameMode == "campaign") {
                endRoomButton.SetActive(true);
            } else
            {
                forwardRoomButton.SetActive(true);
            }

            endRoomButton.SetActive(true);
        }
    }

    public void parseLevelInfo() {

        GameInfoCollocation.info.wordsCompleted = 0;
        this.loadedRooms = new Dictionary<int, CollocationRooms>();

        this.scoreText.text = levelInfo.score.ToString();

        foreach (CollocationRooms room in levelInfo.rooms)
        {
            this.loadedRooms.Add(room.position, room);

            this.currentRoom = room;

            if (room.position == 1)
            {
                this.checkRoom(room1);
                this.setRoomTitle(room.level_title);
                this.next_round_limit = room.next_round;
            }

            if (room.position == 2)
            {
                this.checkRoom(room2);
            }

            if (room.position == 3)
            {
                this.checkRoom(room3);
            }

            if (room.position == 4)
            {
                this.checkRoom(room4);
            }

            if (room.position == 5)
            {
                this.checkRoom(room5);
            }

            if (room.position == 6)
            {
                this.checkRoom(room6);
            }

            if (room.position == 7)
            {
                this.checkRoom(room7);
            }

            if (room.position == 8)
            {
                this.checkRoom(room8);
            }

            if (room.position == 9)
            {
                this.checkRoom(room9);
            }

            if (room.position == 10)
            {
                this.checkRoom(room10);
            }

        }

        this.checkEndRoom();
    }

    public void GetLevelInfoQuery(int levelId)
    {
        string url = GameSettings.GETLevelInfoCollocationURL + GameSettings.GetUserFBToken() + "&level_id=" + levelId.ToString() + "&type=" + GameInfoCollocation.info.gameMode;

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

    [Serializable]
    class CollocationRooms
    {
        public int level;
        public string headword1;
        public string headword2;
        public string game_type;
        public string type;
        public int score;
        public int position;
        public string level_title;
        public int next_round;
        public int collocation_level_id;
    }

    [System.Serializable]
    class LevelInfo
    {
        public CollocationRooms[] rooms = new CollocationRooms[10];
        public int score;
    }

    class RoomWord {
        public int wordID;
        public string wordText;
    }

}