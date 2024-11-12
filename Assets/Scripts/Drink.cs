using System.Collections;
using System.Collections.Generic;
using Helpers;
using Interfaces;
using UnityEngine;

public class Drink : BaseTile
{
    [SerializeField] private float moveDuration;
    [SerializeField] private AnimationCurve moveCurve;
    
    public override void Move(Transform target)
    {
        
    }
}
