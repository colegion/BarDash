using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour, IPoolable
{
    public string effectName;
    public ParticleSystem effectParticle;
    public void OnAssignPool()
    {
        gameObject.SetActive(false);
    }

    public void OnCreatedForPool()
    {
        Debug.Log("Created");
    }

    public void OnDeletePool()
    {
        Debug.Log("Deleted");
    }

    public void OnReleasePool()
    {
        gameObject.SetActive(true);
        StartCoroutine(EnqueuePool());
    }

    private IEnumerator EnqueuePool()
    {
        yield return new WaitForSeconds(effectParticle.main.duration);
        PoolManager.Instance.EnqueueItemToPool(effectName, this);

    }

}
