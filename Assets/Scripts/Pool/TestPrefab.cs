using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPrefab : MonoBehaviour, IPoolable
{
    public void OnAssignPool()
    {
        Debug.Log("Assigned");
    }

    public void OnCreatedForPool()
    {
        Debug.Log("CreatedForPool");
        gameObject.SetActive(false);
    }

    public void OnDeletePool()
    {
        throw new System.NotImplementedException();
    }

    public void OnReleasePool()
    {
        Debug.Log("ReleasedFromPool");
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
