using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Tweeners
{
    public class WaitressTweener : MonoBehaviour
    {
        [SerializeField] private GameObject blockedEmote;
        [SerializeField] private float emoteScale;
        [SerializeField] private float emoteDuration;
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

        private bool _emotePlaying; 
        public void PlayBlockedEmote()
        {
            if (_emotePlaying) return;
            _emotePlaying = true;
            blockedEmote.gameObject.SetActive(true);
            blockedEmote.transform.DOPunchScale(new Vector3(emoteScale, emoteScale, emoteScale), emoteDuration).OnComplete(
                () =>
                {
                    blockedEmote.gameObject.SetActive(false);
                    _emotePlaying = false;
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
