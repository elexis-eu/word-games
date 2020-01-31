using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LoadGamePlayMode : MonoBehaviour
{

    private PlayMode playmode;

    // Start is called before the first frame update
    void Start()
    {
        this.GetPlayModeQuery();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetPlayModeQuery()
    {
        string url = GameSettings.GETPlayModeURL;

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
            playmode = JsonUtility.FromJson<PlayMode>(uwr.downloadHandler.text);

            PlayModeInfo.collocations_solo = playmode.collocations.solo;
            PlayModeInfo.collocations_multi = playmode.collocations.multi;
            PlayModeInfo.synonym_solo = playmode.synonyms.solo;
            PlayModeInfo.synonym_multi = playmode.synonyms.multi;
        }
    }

    [System.Serializable]
    class PlayMode
    {
        public PlayModeSettings collocations;
        public PlayModeSettings synonyms;
    }

    [System.Serializable]
    class PlayModeSettings
    {
        public bool solo;
        public bool multi;
    }

}
