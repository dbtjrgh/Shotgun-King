using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUIDamagePool : MonoBehaviour
{
    public GameObject uiDamagePrefab;
    private int poolSize = 20;
    private Queue<GameObject> pool;

    void Start()
    {
        pool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(uiDamagePrefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetObject()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            GameObject obj = Instantiate(uiDamagePrefab, transform);
            return obj;
        }
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
