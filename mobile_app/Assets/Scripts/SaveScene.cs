using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveScene : MonoBehaviour
{
    public int CURRENT_MODE;
    public int ANOTHER_PREVIOUS_MODE;

    void Awake()
    {
        if (CURRENT_MODE != -1)
            if (GameSettings.PREVIOUS_MODE != CURRENT_MODE) {
                GameSettings.ANOTHER_PREVIOUS_MODE = GameSettings.PREVIOUS_MODE;
            }

            GameSettings.PREVIOUS_MODE = CURRENT_MODE;
        if (ANOTHER_PREVIOUS_MODE != -1)
            GameSettings.ANOTHER_PREVIOUS_MODE = ANOTHER_PREVIOUS_MODE;
    }

}
