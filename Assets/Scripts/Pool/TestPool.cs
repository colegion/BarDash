using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPool : MonoBehaviour
{
    public TestPrefab test;
    void Start()
    {
        PoolManager.Instance.CreatePool("Test", test, 3);
        PoolManager.Instance.DebugPool();
        for (int i = 0; i < 4; i++)
        {
            PoolManager.Instance.DequeueItemFromPool<TestPrefab>("Test");
        }
    }


}
