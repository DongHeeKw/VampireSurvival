using UnityEngine;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    [SerializeField] private List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePools();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePools()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = CreateNewObject(pool.prefab, pool.tag);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    private GameObject CreateNewObject(GameObject prefab, string tag)
    {
        GameObject obj = Instantiate(prefab);
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        return obj;
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return null;
        }

        Queue<GameObject> pool = poolDictionary[tag];
        
        // If pool is empty, create new object
        if (pool.Count == 0)
        {
            Pool poolData = pools.Find(p => p.tag == tag);
            if (poolData != null)
            {
                GameObject newObj = CreateNewObject(poolData.prefab, tag);
                pool.Enqueue(newObj);
            }
        }

        GameObject objectToSpawn = pool.Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();
        if (pooledObj != null)
        {
            pooledObj.OnObjectSpawn();
        }

        // Add back to queue for reuse
        pool.Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    public void ReturnToPool(string tag, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return;
        }

        obj.SetActive(false);
    }

    // Interface for pooled objects
    public interface IPooledObject
    {
        void OnObjectSpawn();
    }
}