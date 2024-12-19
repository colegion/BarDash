using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EffectController : MonoBehaviour
{
    public static EffectController Instance;
    [SerializeField]
    private AllEffectReferences effectReferences;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    void Start()
    {
        Initialize();
    }
    private void Initialize()
    {
        for (int i = 0; i < effectReferences.inGameEffects.Count; i++)
        {
            PoolManager.Instance.CreatePool(effectReferences.inGameEffects[i].effect.effectName, effectReferences.inGameEffects[i].effect, effectReferences.inGameEffects[i].poolAmount, transform);
        }

    }
    public Effect PlayEffect(string effectName, Vector3 effectPosition, Vector3 effectEulerAngles, Vector3 effectScale)
    {
        Effect pooledEffect = PoolManager.Instance.DequeueItemFromPool<Effect>(effectName);
        pooledEffect.transform.position = effectPosition;
        pooledEffect.transform.eulerAngles = effectEulerAngles;
        pooledEffect.transform.localScale = effectScale;
        pooledEffect.effectParticle.Play();
        return pooledEffect;

    }
    public Effect PlayEffect(string effectName, Vector3 effectPosition, Quaternion effectRotation, Vector3 effectScale)
    {
        Effect pooledEffect = PoolManager.Instance.DequeueItemFromPool<Effect>(effectName);
        pooledEffect.transform.position = effectPosition;
        pooledEffect.transform.rotation = effectRotation;
        pooledEffect.transform.localScale = effectScale;
        pooledEffect.effectParticle.Play();
        return pooledEffect;
    }
    public Effect PlayEffectAndSetParent(string effectName, Vector3 effectLocalPosition, Transform parentObject, Quaternion effectLocalRotation, Vector3 effectlocalScale)
    {
        Effect pooledEffect = PoolManager.Instance.DequeueItemFromPool<Effect>(effectName);
        pooledEffect.transform.SetParent(parentObject);
        pooledEffect.transform.localPosition = effectLocalPosition;
        pooledEffect.transform.localRotation = effectLocalRotation;
        pooledEffect.transform.localScale = effectlocalScale;
        pooledEffect.effectParticle.Play();
        return pooledEffect;


    }
    public void StopEffect(Effect effect)
    {
        effect.effectParticle.Stop();
    }


}



