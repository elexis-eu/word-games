using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    Text timerText;
    public float time;
    bool active;

    private RectTransform rectTransform;

	// Use this for initialization
    void Awake()
    {
        timerText = gameObject.GetComponentInChildren<Text>();
        active = false;

        /*rectTransform = GetComponent<RectTransform>();

        GameSettings.MyDebug(rectTransform.position.x+"/"+ rectTransform.position.y);
        rectTransform.position = new Vector3(-300, 0, 0);
        GameSettings.MyDebug(rectTransform.right + "/" + transform.position.y);*/
    }

    public void Activate(float time)
    {
        this.time = time / GameSettings.TIME_MS_TO_S;
        active = true;
    }
	
    public float GetTimeLeft()
    {
        return time * GameSettings.TIME_MS_TO_S;
    }

    public void UpdateTime(float time)
    {
        timerText.text = time + "";
    }

	// Update is called once per frame
	void Update() {
        if (active)
        {
            time -= Time.deltaTime;
            if (time <= 0)
            {
                time = 0;
            }
            timerText.text = ((int)time+1) + "";
        }
    }
}
