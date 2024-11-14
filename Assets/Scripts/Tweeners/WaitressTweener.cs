using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Tweeners
{
    public class WaitressTweener : MonoBehaviour
    {
        [SerializeField] private List<TweenConfig> tweenConfigs;

        private readonly Dictionary<TweenType, TweenConfig> _tweenConfigDictionary = new Dictionary<TweenType, TweenConfig>();
        private void Awake()
        {
            foreach (var config in tweenConfigs)
            {
                if (!_tweenConfigDictionary.TryAdd(config.type, config))
                {
                    Debug.LogError("Duplicate Tween Config Type!");
                }
            }
        }

        public void TweenWaitress(Waitress waitress, Vector3 target, TweenType moveType, Action onComplete = null)
        {
            if (moveType == TweenType.Slot)
            {
                target = waitress.GetTargetSlot().GetTarget().position;
            }
            var configToUse = _tweenConfigDictionary[moveType];
            waitress.transform.DOMove(target, configToUse.duration).SetEase(configToUse.curve).OnComplete(() =>
            {
                if (moveType == TweenType.Slot || moveType == TweenType.Success)
                {
                    onComplete?.Invoke();
                }
            });
        }
        
        public float GetTweenDuration(TweenType moveType)
        {
            return _tweenConfigDictionary[moveType].duration;
        }
    }
    
    [Serializable]
    public class TweenConfig
    {
        public TweenType type;
        public float duration;
        public Ease ease;
        public AnimationCurve curve;
    }

    public enum TweenType
    {
        Grid,
        Slot,
        Success
    }
}
