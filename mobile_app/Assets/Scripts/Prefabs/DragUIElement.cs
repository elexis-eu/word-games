using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragUIElement : MonoBehaviour
{
    public GameObject gameStateObject;
    DragDrag scrGameState;

    float screenWidth;
    float screenHeight;
    float elementHeight, elementWidth;

    float OffsetX;
    float OffsetY;

    float saveX;
    float saveY;

    bool moveToMiddlePos;
    bool moveToOriginalPos;
    int choiceSelected = -1;

    bool blockActions;

    float speed = 2*Screen.width;

    // Start is called before the first frame update
    void Awake()
    {
        moveToMiddlePos = false;

        moveToOriginalPos = false;
        choiceSelected = -1;
    }

    void Start()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        scrGameState = gameStateObject.GetComponent<DragDrag>();

        saveX = transform.position.x;
        saveY = transform.position.y;

        /*RectTransform rt = GetComponent<RectTransform>();
        elementHeight = rt.rect.height;
        elementWidth = rt.rect.width;*/
        //GameSettings.MyDebug(elementHeight + " && " + elementWidth);

        ResetPosition();
    }

    public void CallInvoke()
    {
        moveToMiddlePos = true;
        ResetPosition();
    }

    public void BeginDrag()
    {

        if (blockActions)
            return;

        OffsetX = transform.position.x - Input.mousePosition.x;
        OffsetY = transform.position.y - Input.mousePosition.y;

        moveToMiddlePos = false;
        moveToOriginalPos = false;
    }

    public void OnDrag()
    {
        if (blockActions)
            return;

        transform.position = new Vector3(OffsetX + Input.mousePosition.x, OffsetY + Input.mousePosition.y);
    }

    public void EndDrag()
    {
        if (blockActions)
            return;

        moveToOriginalPos = true;
        moveToMiddlePos = false;

        //GameSettings.MyDebug(saveY + " && " + elementHeight + " && " +transform.position.y);

        
        if (transform.position.y >= saveY + 0.084*screenHeight)
        {
            scrGameState.WordBelongsToUserChoice(0);
            choiceSelected = 0;
            blockActions = true;
        }
        else if (transform.position.y <= saveY - 0.084*screenHeight)
        {
            scrGameState.WordBelongsToUserChoice(1);
            choiceSelected = 1;
            blockActions = true;
        }
        else if (transform.position.x >= saveX + 0.2*screenWidth)
        {
            scrGameState.WordBelongsToUserChoice(2);
            choiceSelected = 2;
            blockActions = true;
        }
    }
    
    void ResetPosition()
    {
        blockActions = false;
        //moveToOriginalPos = false;
        //GameSettings.MyDebug("BEFORE POS: " + transform.position.x + " & " + transform.position.y);
        transform.position = new Vector3(-screenWidth, saveY, 0);
        //GameSettings.MyDebug("RESET POS: " + transform.position.x + " & " + transform.position.y);
        choiceSelected = -1;
        //GameSettings.MyDebug(saveX + " / "  +saveY);
        //GameSettings.MyDebug(screenHeight);
        //GameSettings.MyDebug(saveY);
    }
    
    void Update()
    {
        float step = speed * Time.deltaTime;
        if (moveToMiddlePos)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(saveX, saveY, 0), step);
        }
        if (choiceSelected != -1)
        { 
            if (choiceSelected == 0)
            {
                /*if (!blockActions && Vector3.Distance(transform.position, new Vector3(saveX, saveY + elementHeight / 2)) < 1f)
                {
                    blockActions = true;

                } else*/
              
                    if (!blockActions)
                    {
                        //transform.position = Vector3.MoveTowards(transform.position, new Vector3(saveX, saveY + elementHeight / 2, 0), step);
                    } else
                    {
                        transform.position = Vector3.MoveTowards(transform.position, new Vector3(scrGameState.fake3Obj.transform.position.x/*saveX*/, scrGameState.fake3Obj.transform.position.y/*saveY + screenHeight/3.8f*/, 0), step);
                    }
                    
               
            } else if (choiceSelected == 1)
            {
                /*if (!blockActions && Vector3.Distance(transform.position, new Vector3(saveX, saveY - elementHeight / 2, 0)) < 1f)
                {
                    
                    blockActions = true;
      
                } else*/
                
                    if (!blockActions)
                    {
                        //transform.position = Vector3.MoveTowards(transform.position, new Vector3(saveX, saveY - elementHeight / 2, 0), step);
                    } else
                    {
                        transform.position = Vector3.MoveTowards(transform.position, new Vector3(scrGameState.fake4Obj.transform.position.x/*saveX*/, scrGameState.fake4Obj.transform.position.y/*saveY - screenHeight/4*/, 0), step);
                    }
                    
               
            } else if (choiceSelected == 2)
            {
                /*if (!blockActions && Vector3.Distance(transform.position, new Vector3(saveX + screenWidth / 2.5f, saveY, 0)) < 1f)
                {
                    blockActions = true;
   
                } else*/
              
                    if (!blockActions)
                    {
                        //transform.position = Vector3.MoveTowards(transform.position, new Vector3(saveX + screenWidth / 2.5f, saveY, 0), step);
                    } else
                    {
                        transform.position = Vector3.MoveTowards(transform.position, new Vector3(saveX + screenWidth, saveY, 0), step);
                    }
                    
               
            }
        }
        else if (!moveToMiddlePos && moveToOriginalPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(saveX, saveY, 0), step);
        }
    }
}
