using System.Collections.Generic;
using UnityEngine;

public class PrefabStorage : MonoBehaviour
{/*
    List<Prefab2> storage;

    public GameObject prefab;

    // Use this for initialization
    void Awake()
    {
        storage = new List<Prefab2>();
    }

    public GameObject GetPrefab(float x, float y, float z)
    {
        for (int i = 0; i < storage.Count; i++)
        {
            if (!storage[i].active)
            {
                return storage[i].GetPrefab(x, y, z);
            }
        }

        storage.Add(new Prefab2(prefab));
        return storage[storage.Count - 1].GetPrefab(x, y, z); 
    }

    public void FreeStorage()
    {
        for (int i = 0; i < storage.Count; i++)
        {
            storage[i].FreePrefab();
        }
    }

    public void FreeStorage2()
    {
        for (int i = 0; i < storage.Count; i++)
        {
            storage[i].FreePrefab();
            storage[i].prefabActive.GetComponent<ButtonScript>().Reset();
        }
    }

    public void FreeStorage3()
    {
        for (int i = 0; i < storage.Count; i++)
        {
            storage[i].FreePrefab();
            storage[i].prefabActive.GetComponent<DragObject>().Reset();
        }
    }

    public class Prefab2
    {
        public GameObject prefabActive;

        public bool active;

        public Prefab2(GameObject prefab)
        {
            prefabActive = (GameObject)GameObject.Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
            prefabActive.SetActive(false);
            active = false;
        }

        public GameObject GetPrefab(float x, float y, float z)
        {
            prefabActive.transform.position = new Vector3(x, y, z);
            active = true;
            prefabActive.SetActive(true);
            return prefabActive;
        }

        public void FreePrefab()
        {
            active = false;
            prefabActive.SetActive(false);
        }
    }*/
}
