using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnFieldSelected : MonoBehaviour, ISelectHandler
{
    private InsertInsert insertInsertSrc;

    [HideInInspector]
    public int pos;

    public void OnSelect(BaseEventData eventData)
    {
        insertInsertSrc.OnSelectedFocusChangedTo(pos);
    }

    public void Init(InsertInsert insertInsertSrc, int pos)
    {
        this.insertInsertSrc = insertInsertSrc;
        this.pos = pos;
    }
}
