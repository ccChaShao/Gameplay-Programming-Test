using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectPool : MonoBehaviour
{
    // 单例实例，用于全局访问
    public static ObjectPool Instance;

    [Serializable]
    public class Pool
    {
        public string tag;
        public int defaultSize;
        public GameObject prefab;
    }

    public Transform poolRoot;
    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }

        InitializePools();
    }

    // 初始化所有对象池
    private void InitializePools()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            // 初始化并创建指定数量的对象
            for (int i = 0; i < pool.defaultSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab, poolRoot); // 可指定父物体
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    // 从指定对象池中获取对象
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        // 检查对象池是否存在
        if (!poolDictionary.ContainsKey(tag))
        {
            return null;
        }

        // 如果池为空，可选择动态扩展（这里简单创建并返回一个新对象，可根据需要修改为扩展池逻辑）
        if (poolDictionary[tag].Count == 0)
        {
            GameObject newObj = Instantiate(GetPrefabByTag(tag), poolRoot);
            newObj.SetActive(false);
            poolDictionary[tag].Enqueue(newObj); // 虽然立即取出，但也先放进去以保证逻辑一致
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        // 设置对象的位置、旋转并激活
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

        return objectToSpawn;
    }

    // 将对象返回对象池
    public void ReturnToPool(string tag, GameObject objectToReturn)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            return;
        }

        objectToReturn.SetActive(false);
        poolDictionary[tag].Enqueue(objectToReturn);
    }

    // 根据tag查找对应的预制体（用于池空时动态创建）
    private GameObject GetPrefabByTag(string tag)
    {
        foreach (Pool pool in pools)
        {
            if (pool.tag == tag)
            {
                return pool.prefab;
            }
        }
        return null;
    }
}