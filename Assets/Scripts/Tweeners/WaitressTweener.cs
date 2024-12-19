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

        private readonly Dictionary<TweenType, TweenConfig> _tweenConfigDictionary =
            new Dictionary<TweenType, TweenConfig>();

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
            Sequence sequence = DOTween.Sequence();
            if (moveType == TweenType.Slot)
            {
                target = waitress.GetTargetSlot().GetTarget().position;
            }
            else
            {
                target += Vector3.up;
            }

            var configToUse = _tweenConfigDictionary[moveType];
            sequence.Append(waitress.transform.DOMove(target, configToUse.duration)
                .SetEase(configToUse.curve)
                .OnComplete(
                    () =>
                    {
                        if (moveType == TweenType.Slot)
                        {
                            onComplete?.Invoke();
                        }
                    }));
        }

        public void HandleSuccessMovement(Waitress waitress, Vector3 target, TweenType moveType, Action onComplete)
        {
            Sequence sequence = DOTween.Sequence();
            var config = _tweenConfigDictionary[moveType];
            var targetPosition = waitress.transform.position + Vector3.forward;
            sequence.Append(waitress.transform.DOMove(targetPosition, config.duration).SetEase(config.curve));
            sequence.Append(waitress.transform.DOMove(target, config.duration).SetEase(config.curve));

            sequence.OnComplete(() =>
            {
                onComplete?.Invoke();
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