using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SAVER : MonoBehaviour
{
    public void OnParentAboutToBeDestroyed()
    {
        GameSettings.MyDebug("My parent was about to be destroyed, so I moved out.");
        transform.parent = null;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        GameSettings.MyDebug("SAVER DESTROY");
    }
}
