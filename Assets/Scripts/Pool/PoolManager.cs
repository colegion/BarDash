
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;
    private Dictionary<string, Queue<UnityEngine.Object>> _pools = new Dictionary<string, Queue<UnityEngine.Object>>();
    private Dictionary<string, Object> _prefabPools = new Dictionary<string, Object>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

    }
    public void CreatePool<T>(string poolName, T poolObject, int amount, Transform poolParentObject) where T : UnityEngine.Object, IPoolable
    {
        if (_pools.ContainsKey(poolName))
        {
            var createdObject = Instantiate(poolObject);
            createdObject.GameObject().transform.SetParent(poolParentObject);
            EnqueueItemToPool(poolName, createdObject);
        }
        else
        {
            Queue<T> pool = new Queue<T>();
            for (int i = 0; i < amount; i++)
            {
                var createdObject = Instantiate(poolObject);
                createdObject.GameObject().transform.SetParent(poolParentObject);
                pool.Enqueue(createdObject);
                poolObject.OnCreatedForPool();
                poolObject.OnAssignPool();
            }
            _pools[poolName] = new Queue<UnityEngine.Object>(pool);
            _prefabPools.Add(poolName, poolObject);
        }

    }
    public void RemovePool(string poolName)
    {
        _pools.Remove(poolName);
    }
    public void DeletePool<T>(string poolName)where T:Object,IPoolable
    {
       //StillProgress!!
    }
    public Queue<T> GetPool<T>(string poolName) where T : UnityEngine.Object, IPoolable
    {
        if (!_pools.ContainsKey(poolName))
        {
            Debug.Log("Pool doesn't exists");
            return null;
        }
        Queue<Object> currentExistingPool = _pools[poolName];
        Queue<T> typeCastedPool = new Queue<T>();
        foreach (var obj in currentExistingPool)
        {
            if (obj is T typeCastedObj)
            {
                typeCastedPool.Enqueue(typeCastedObj);
            }
            else
            {
                Debug.LogWarning($"Object in pool {poolName} cannot be cast to type {typeof(T)}");
            }
        }
        return typeCastedPool;

    }
    public void DebugPool()
    {
        for (int i = 0; i < _pools.Count; i++)
        {
            Debug.Log(_pools.ElementAt(i).Key);
            Debug.Log(_pools.ElementAt(i).Value.GetType());
        }

    }

    public T DequeueItemFromPool<T>(string poolName) where T : UnityEngine.Object, IPoolable
    {
        T pooleable;
        if (_pools.ContainsKey(poolName))
        {
            if (_pools[poolName].TryDequeue(out Object result))
            {
                pooleable = (T)result;
                pooleable.OnReleasePool();
                return (T)result;
            }
            else
            {
                Object instantiatedObject = Instantiate(_prefabPools[poolName]);
                pooleable = (T)instantiatedObject;
                pooleable.OnReleasePool();
                return (T)instantiatedObject;
            }

        }
        else
        {
            Debug.Log("Pool doesn't exists");
            return null;
        }

    }
    public void EnqueueItemToPool<T>(string poolName, T item) where T : UnityEngine.Object, IPoolable
    {
        if (_pools.ContainsKey(poolName))
        {
            item.OnAssignPool();
            _pools[poolName].Enqueue(item);
        }
        else
        {
            Debug.Log("Pool doesn't exists");
        }

    }



}
