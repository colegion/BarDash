using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Helpers;
using Interfaces;
using UnityEngine;

public class Drink : BaseTile
{
    [SerializeField] private GameObject visuals;
    [SerializeField] private float onTrayScale;
    [SerializeField] private float moveDuration;
    [SerializeField] private AnimationCurve moveCurve;
    
    public override void SetTransform()
    {
        transform.SetParent(GameController.Instance.GetParentByType(TileArea));
        transform.localPosition = new Vector3(X, -0.329f, Y);
    }
    
    public override void Move(Transform target, Action onComplete = null)
    {
        DOVirtual.DelayedCall(0.15f, () =>
        {
            transform.DOMove(target.position + Vector3.up, moveDuration).SetEase(moveCurve).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        });
    }
    public override void Move(Vector3 targetVector)
    {
        DOVirtual.DelayedCall(0.15f, () =>
        {
            transform.DOMove(targetVector , moveDuration).SetEase(moveCurve);
        });
    }

    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
    }

    public void SetScale()
    {
        visuals.transform.localScale = new Vector3(onTrayScale, onTrayScale, onTrayScale);
    }
}
