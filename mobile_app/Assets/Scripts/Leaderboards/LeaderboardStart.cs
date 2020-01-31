using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardStart : MonoBehaviour
{
    public GameObject thematicBtnObj;
    private Button thematicBtn;

    void Awake()
    {
        thematicBtn = thematicBtnObj.GetComponent<Button>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (GameSettings.user == null)
        {
            thematicBtn.image.color = GameSettings.COLOR_WHITE;
            thematicBtn.interactable = false;
        } else
        {
            if (GameSettings.past_and_current_games_info ==  null || GameSettings.past_and_current_games_info.Length == 0)
            {
                thematicBtn.image.color = GameSettings.COLOR_WHITE;
                thematicBtn.interactable = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
