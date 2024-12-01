using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectData/AllEffectData")]
public class AllEffectReferences : ScriptableObject
{
    [System.Serializable]
    public struct EffectData
    {
        public Effect effect;
        public int poolAmount;

    }
    public List<EffectData> inGameEffects = new List<EffectData>();
}
