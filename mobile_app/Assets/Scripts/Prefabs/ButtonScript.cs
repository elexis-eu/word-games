using UnityEngine;

public class ButtonScript : MonoBehaviour
{/*
    private GameState_Choose scrGameState;
    private Color taken;
    private Color free;

    public int value;
    public bool selected;
    public string word;

    // Use this for initialization
    void Awake()
    {
        scrGameState = GameObject.Find("Main Camera").GetComponent<GameState_Choose>();
        taken = new Color(0.8f, 0.8f, 0.8f);
        free = new Color(1, 1, 1);
        selected = false;
    }

    public void OnMouseDown()
    {
        if (selected)
        {
            gameObject.GetComponent<SpriteRenderer>().color = free;
            selected = false;
            scrGameState.selected--;
        } else if (scrGameState.selected < Gamechoose.choose.max_select)
        {
            gameObject.GetComponent<SpriteRenderer>().color = taken;
            selected = true;
            scrGameState.selected++;
        }
    }

    public void Reset()
    {
        selected = false;
        gameObject.GetComponent<SpriteRenderer>().color = free;
    }*/
}
