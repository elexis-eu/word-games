using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDestroyPlease()
    {
        gameObject.transform.SetParent(null, false);
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        GameSettings.MyDebug("TEST2 DESTROY");
    }
}
