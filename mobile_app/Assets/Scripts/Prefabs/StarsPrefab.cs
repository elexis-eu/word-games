using UnityEngine;
using System.Collections;

public class StarsPrefab : MonoBehaviour
{
    public GameObject[] starsEmptyObj;
    public GameObject[] starsFullObj;

    private void Awake()
    {
        for (int i = 0; i < starsEmptyObj.Length; i++)
        {
            starsEmptyObj[i].SetActive(false);
        }

        for (int i = 0; i < starsFullObj.Length; i++)
        {
            starsFullObj[i].SetActive(false);
        }
    }

    public void EnableStarsObjects(int starsEmpty, int starsFull)
    {
        //GameSettings.MyDebug(starsEmpty + " / " + starsFull);
        for (int i = 0; i < starsEmpty; i++)
        {
            starsEmptyObj[i].SetActive(true);
        }

        for (int i = 0; i < starsFull; i++)
        {
            starsFullObj[i].SetActive(true);
        }
    }
}
