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
    [SerializeField] private WaitressTray tray;
    [SerializeField] private Collider collider;
    [SerializeField] private WaitressTweener tweener;
    [SerializeField] private Animator animator;

    private WaitressSlot _targetSlot;
    private Vector3 _lastPosition;
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");

    public static event Action<Waitress> OnSuccessfulInput;
    public static event Action<Waitress> OnWaitressReachedTarget;

    private void OnEnable()
    {
        AddListeners();
    }

    private void OnDisable()
    {
        RemoveListeners();
    }

    private void Start()
    {
        _lastPosition = transform.position;
        tray.Initialize(3, -0.73f);
    }

    private void Update()
    {
        Vector3 currentPosition = transform.position;

        // Check if the position has changed
        if (currentPosition != _lastPosition)
        {
            // Calculate the direction of movement, ignoring the Y-axis
            Vector3 direction = new Vector3(currentPosition.x - _lastPosition.x, 0f, currentPosition.z - _lastPosition.z).normalized;

            // Update the waitress's rotation to look in the direction of movement
            if (direction != Vector3.zero) // Avoid rotation if there's no movement
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f); // Smooth rotation
            }

            // Update the last position to the current position
            _lastPosition = currentPosition;
        }
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
                StartCoroutine(MoveRoutine(path));
                //IteratePath(path);
            }
            else
            {
                collider.enabled = true;
            }
        }
        else
        {
            tweener.PlayBlockedEmote();
        }
    }

    private IEnumerator MoveRoutine(List<Cell> path)
    {
        animator.SetBool(IsWalking, true);
        foreach (var cell in path)
        {
            Vector3 targetPosition = cell.GetWorldPosition();
            tweener.TweenWaitress(this, targetPosition, TweenType.Grid);
            yield return new WaitForSeconds(tweener.GetTweenDuration(TweenType.Grid));
        }

        tweener.TweenWaitress(this, transform.position, TweenType.Slot, () =>
        {
            OnWaitressReachedTarget?.Invoke(this);
            animator.SetBool(IsWalking, false);
        });
    }

    public void HandleFinalMovement(Transform target, Action onComplete)
    {
        tweener.TweenWaitress(this, target.position, TweenType.Success, () =>
        {
            onComplete?.Invoke();
            animator.SetBool(IsWalking, false);
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

    public Transform GetTraySlot()
    {
        return tray.GiveDrinkTransform();
    }

    public Transform GetTray()
    {
        return tray.transform;
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