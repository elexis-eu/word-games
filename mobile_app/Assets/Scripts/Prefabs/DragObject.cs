using UnityEngine;

public class DragObject : MonoBehaviour {
    private bool touching;

    private Vector3 _startTouchPos;
    private Vector3 _startPos;
    private SpriteRenderer spriteRenderer;

    private Color correct;
    private Color wrong;
    private Color neutral;

    private readonly float SPEED = 6;
    private readonly float MAX_X = 1.5f;
    private readonly float SELECTED_X = 1.35f;
    private readonly float CUT_X = 0.9f;
    private readonly float CENTER = 0;
    
    public int position;
    public int correctPosition;
    public int value;

    public string word; 

    private void Awake()
    {
        position = 0;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        correct = new Color(0, 1, 0);
        wrong = new Color(1, 0, 0);
        neutral = new Color(1, 1, 1);
    }

    private void OnMouseDown()
    {
        _startTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _startPos = transform.position;

        touching = true;
    }

    private void OnMouseDrag()
    {
        Vector3 _currentTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 _diff = _currentTouchPos - _startTouchPos;
        Vector3 _pos = _startPos + _diff;
        float x = _pos.x;

        if (x <= -MAX_X)
            x = -MAX_X;
        else if (x >= MAX_X)
            x = MAX_X;

        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }

    private void OnMouseUp()
    {
        touching = false;
    }

    void Update()
    {
        if (!touching)
        {
            float step = SPEED * Time.deltaTime;
            if (transform.position.x <= -CUT_X)
            {
                transform.position = Vector3.MoveTowards(transform.position, 
                                     new Vector3(-SELECTED_X, transform.position.y, transform.position.z),
                                     step);
                position = 1;
            } else if (transform.position.x >= CUT_X)
            {
                transform.position = Vector3.MoveTowards(transform.position,
                                     new Vector3(SELECTED_X, transform.position.y, transform.position.z),
                                     step);
                position = 2;
            } else
            {
                transform.position = Vector3.MoveTowards(transform.position,
                                     new Vector3(CENTER, transform.position.y, transform.position.z),
                                     step);
                position = 0;
            }
        }
    }

    public bool CheckPosition()
    {
        if (position == correctPosition)
        {
            IsCorrectPosition();
            return true;
        } else
        {
            IsWrongPosition();
            return false;
        }
    }

    public void IsCorrectPosition()
    {
        spriteRenderer.color = correct;
    }

    public void IsWrongPosition()
    {
        spriteRenderer.color = wrong;
    }

    public void Reset()
    {
        position = 0;
        transform.position = new Vector3(CENTER, transform.position.y, transform.position.z);
        spriteRenderer.color = neutral;
    }
}
