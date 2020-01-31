using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TEST : MonoBehaviour
{
    public GameObject inputFieldObj;
    public InputField inputfield;
    public GameObject textObj;
    public Text text;
    // Start is called before the first frame update
    void Start()
    {
        inputfield = inputFieldObj.GetComponent<InputField>();
        text = textObj.GetComponent<Text>();
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
    }



    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        GameSettings.MyDebug("TEST DESTROY");

        /*foreach (Transform cc in transform)
            cc.GetComponent<SAVER>().OnParentAboutToBeDestroyed();*/

        //test2.OnDestroyPlease();
        
    }

    public void ChangeScene()
    {
        
        SceneSwitcher.LoadScene2(GameSettings.INTRO_SCREEN);
    }

    


}
