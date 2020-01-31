using UnityEngine;

public class EnabDisabCan : MonoBehaviour
{
    Canvas canvas;

    // Use this for initialization
    void Awake()
    {
        canvas = gameObject.GetComponent<Canvas>();
	}
	
    public void EnableDisableCanvas()
    {
        canvas.enabled = !canvas.enabled;
    }

    public void EnableCanvas()
    {
        canvas.enabled = true;
    }

    public void DisableCanvas()
    {
        canvas.enabled = false;
    }
}
