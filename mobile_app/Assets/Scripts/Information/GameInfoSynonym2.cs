using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GameInfoSynonym2
{
    public static SynonymInfo2 info = new SynonymInfo2();
    public static SynonymGameInfo currentGame = new SynonymGameInfo();

    public static void SetRoundStartInfo(string infoR, bool current)
    {
        info.currentLevel = 13;
    }
    public static void LoadTest()
    {
        info.campaignLevel = 123;
        currentGame.currentWord = "pretrigati";
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
public class SynonymInfo2
{
    public int currentLevel;
    public int campaignLevel;
    public int wordsCompleted;
    public string gameMode;

}

[System.Serializable]
public class SynonymRoomInfo
{
    public int currentWord;

}

[System.Serializable]
public class SynonymGameInfo
{
    public string currentWord;
    public int currentWordID;
}


[System.Serializable]
public class ReceiveSynonym2
{
    public int code;
    public ReceiveDataSynonym[] data = new ReceiveDataSynonym[1];
}

[System.Serializable]
public class ReceiveDataSynonym2
{
    public string word;
    public string[] buttons = new string[3 /*this is max cause of graphics currently*/];
    public ReceiveSynonymScores2[] scores = new ReceiveSynonymScores2[3];
}

[System.Serializable]
public class ReceiveSynonymScores2
{
    public string word;
    public int score;
}