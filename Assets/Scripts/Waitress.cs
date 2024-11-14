using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GoalSystem;
using Helpers;
using Interfaces;
using Tweeners;
using UnityEngine;
using TweenType = Tweeners.TweenType;

public class Waitress : BaseTile, ITappable
{
    [SerializeField] private Collider collider;
    [SerializeField] private WaitressTweener tweener;

    private WaitressSlot _targetSlot;

    public static event Action<Waitress> OnSuccessfulInput;
    public static event Action OnWaitressReachedTarget;

    private void OnEnable()
    {
        AddListeners();
    }

    private void OnDisable()
    {
        RemoveListeners();
    }

    public void OnTap()
    {
        Move(null);
    }
    
    public override void Move(Transform target, Action onComplete = null)
    {
        if (GameController.Instance.IsInputAcceptable())
        {
            collider.enabled = false;
            if (GameController.Instance.TryFindPath(this, out List<Cell> path))
            {
                OnSuccessfulInput?.Invoke(this);
                IteratePath(path);
            }
        }
        else
        {
            //@todo: play some angry emoji.
        }
    }

    private void IteratePath(List<Cell> path)
    {
        Sequence sequence = DOTween.Sequence();
        
        foreach (var target in path)
        {
            Vector3 targetPosition = target.GetWorldPosition();
            sequence.AppendCallback(() =>
            {
                tweener.TweenWaitress(this, targetPosition, TweenType.Grid);
            });
            sequence.AppendInterval(tweener.GetTweenDuration(TweenType.Grid));
        }
        
        sequence.AppendCallback(() =>
        {
            tweener.TweenWaitress(this, transform.position, TweenType.Slot, () =>
            {
                OnWaitressReachedTarget?.Invoke();
            });
        });
    }

    public void HandleFinalMovement(Transform target, Action onComplete)
    {
        tweener.TweenWaitress(this, target.position, TweenType.Success, () =>
        {
            onComplete?.Invoke();
        });
    }
    
    public void SetTargetSlot(WaitressSlot slot)
    {
        _targetSlot = slot;
    }

    public WaitressSlot GetTargetSlot()
    {
        return _targetSlot;
    }

    public void ClearTargetSlot()
    {
        _targetSlot = null;
    }

    private void AddListeners()
    {
        InputController.Instance.AddTappable(this);
    }

    private void RemoveListeners()
    {
        InputController.Instance.RemoveTappable(this);
    }
}
