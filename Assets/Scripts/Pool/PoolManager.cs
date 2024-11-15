
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;
    private Dictionary<string, Queue<UnityEngine.Object>> _pools = new Dictionary<string, Queue<UnityEngine.Object>>();
    private Dictionary<string, Object> _prefabPools = new Dictionary<string, Object>();

    public Queue<T> CreatePool<T>(string poolName, T poolObject, int amount) where T : UnityEngine.Object
    {
        Queue<T> pool = new Queue<T>();
        for (int i = 0; i < amount; i++)
        {
            pool.Enqueue(Instantiate(poolObject));
        }
        _pools[poolName] = new Queue<UnityEngine.Object>(pool);
        _prefabPools.Add(poolName, poolObject);
        return pool;
    }
    public void DebugPool()
    {
        for (int i = 0; i < _pools.Count; i++)
        {
            Debug.Log(_pools.ElementAt(i).Key);
            Debug.Log(_pools.ElementAt(i).Value.GetType());
        }

    }
    private void Awake() => Instance = this;


    public T DequeueItemFromPool<T>(string poolName) where T : UnityEngine.Object
    {
        if (_pools.ContainsKey(poolName))
        {
            if (_pools[poolName].TryDequeue(out Object result))
            {
                return (T)result;
            }
            else
            {
                return Instantiate((T)_prefabPools[poolName]);
            }

        }
        else
        {
            Debug.Log("Pool doesn't exists");
            return null;
        }

    }
    public void EnqueueItemToPool<T>(string poolName, T item) where T : UnityEngine.Object
    {
        if (_pools.ContainsKey(poolName))
        {
            _pools[poolName].Enqueue(item);
        }
        else
        {
            Debug.Log("Pool doesn't exists");
        }

    }



}
