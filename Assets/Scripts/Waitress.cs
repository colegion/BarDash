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
    private bool _waitingProcess;

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
    
    public override void SetTransform()
    {
        transform.SetParent(GameController.Instance.GetParentByType(TileArea));
        transform.localPosition = new Vector3(X, -.8f, Y);
    }

    private void Update()
    {
        Vector3 currentPosition = transform.position;
        if (currentPosition != _lastPosition)
        {
            Vector3 direction = new Vector3(currentPosition.x - _lastPosition.x, 0f, currentPosition.z - _lastPosition.z).normalized;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
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
                if(path.Count > 0)
                    path.RemoveAt(0);
                StartCoroutine(MoveRoutine(path));
            }
            else
            {
                collider.enabled = true;
                EffectController.Instance.PlayEffect("BlockedEmoji", new Vector3(transform.position.x, transform.position.y + 3f, transform.position.z), Vector3.zero, new Vector3(0.5f, 0.5f, 0.5f));
            }
        }
    }

    private IEnumerator MoveRoutine(List<Cell> path)
    {
        isMoving = true;
        animator.SetBool(IsWalking, true);
        Debug.Log(path.Count+"PATH COUNT");
        foreach (var cell in path)
        {
            Vector3 targetPosition = cell.GetWorldPosition();
            //tweener.TweenWaitress(this, targetPosition, TweenType.Grid);
            //yield return new WaitForSeconds(tweener.GetTweenDuration(TweenType.Grid));
            while(transform.position!=targetPosition)
            {
                transform.position=Vector3.MoveTowards(transform.position,targetPosition,5f*Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
         
        }

        tweener.TweenWaitress(this, transform.position, TweenType.Slot, () =>
        {
            isMoving = false;
            transform.rotation = Quaternion.Euler(Vector3.zero);
            animator.SetBool(IsWalking, false);
            OnWaitressReachedTarget?.Invoke(this);
        });
    }

    public void HandleFinalMovement(Transform target, Action onComplete)
    {
        DOVirtual.DelayedCall(0.15f, ()=>
        {
            EffectController.Instance.PlayEffect("Confetti", tray.transform.position, Vector3.zero, Vector3.one);
        });
        animator.SetBool(IsWalking, true);
        /*tweener.TweenWaitress(this, target.position, TweenType.Success, () =>
        {
            onComplete?.Invoke();
            animator.SetBool(IsWalking, false);
        });*/

        tweener.HandleSuccessMovement(this, target.position, TweenType.Success, () =>
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

    public void SetProcess(bool value)
    {
        _waitingProcess = value;
    }

    public bool IsWaitingProcess()
    {
        return _waitingProcess;
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