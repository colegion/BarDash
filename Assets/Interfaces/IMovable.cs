using System;
using UnityEngine;

namespace Interfaces
{
    public interface IMovable
    {
        public abstract void Move(Transform target, Action onComplete = null);
    }
}
