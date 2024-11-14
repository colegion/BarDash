using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Helpers;
using Interfaces;
using UnityEngine;

public class Drink : BaseTile
{
    [SerializeField] private float moveDuration;
    [SerializeField] private AnimationCurve moveCurve;
    
    public override void Move(Transform target, Action onComplete = null)
    {
        transform.DOMove(target.position + Vector3.up, moveDuration).SetEase(moveCurve).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }
}
