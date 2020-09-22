using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GameInfoCollocation
{
    public static CollocationInfo info = new CollocationInfo();
    public static CollocationGameInfo currentGame = new CollocationGameInfo();

    public static void SetRoundStartInfo(string infoR, bool current)
    {
        info.currentLevel = 13;
    }
    public static void LoadTest()
    {
        info.campaignLevel = 123;
        currentGame.currentRoom = 1;
    }

    public static void SetRoundOverInfo(string infoR)
    {
        //rec = JsonUtility.FromJson<ReceiveSynonym>(infoR);
    }

    public static void SetNewRound()
    {

    }
}

[System.Serializable]
public class CollocationInfo
{
    public int currentLevel;
    public int campaignLevel;
    public int wordsCompleted;
    public string gameMode;

}

[System.Serializable]
public class CollocationRoomInfo
{
    public int currentWord;

}

[System.Serializable]
public class CollocationGameInfo
{
    public int currentRoom;
    public int currentCollocationLevelID;
    public int firstWordCollocationID;
    public int secondWordCollocationID;
    public string currentGameType;
    public string currentGameData;
    public int currentGamePointsMultiplier;
}


[System.Serializable]
public class ReceiveCollocation
{
    public int code;
    public ReceiveDataCollocation[] data = new ReceiveDataCollocation[1];
}

[System.Serializable]
public class ReceiveDataCollocation
{
    public string word;
    public string[] buttons = new string[3 /*this is max cause of graphics currently*/];
    public ReceiveCollocationScores[] scores = new ReceiveCollocationScores[3];
}

[System.Serializable]
public class ReceiveCollocationScores
{
    public string word;
    public int score;
}