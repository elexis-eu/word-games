using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DragUIScore : MonoBehaviour
{
    Text txtScore;

    // Use this for initialization
    void Start()
    {
        txtScore = this.GetComponent<Text>();
    }

    void ShowScore(int score)
    {
        txtScore.text = ""+score;

        //startAnimation();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
