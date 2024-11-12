using System;
using System.Collections.Generic;
using Interfaces;
using JetBrains.Annotations;
using UnityEngine;

namespace Helpers
{
    public class InputController : MonoBehaviour
    {
        private static InputController _instance;
        
        public static InputController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("InputController").AddComponent<InputController>();
                }
                return _instance;
            }
        }

        private List<ITappable> _tappable = new List<ITappable>();
        private readonly string _layerMask = "Input";

        private void Awake()
        {
            DontDestroyOnLoad(_instance);
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var isValid = TryGetTappable(out ITappable tappable);

                if (isValid && tappable != null)
                {
                    tappable.OnTap();
                }
            }
        }

        private bool TryGetTappable(out ITappable tappable)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Vector3.down, out hit, 300f, LayerMask.NameToLayer(_layerMask)))
            {
                var go = hit.collider.gameObject;

                if (go.TryGetComponent(out ITappable item))
                {
                    tappable = item;
                    return true;
                }
            }

            tappable = null;
            return false;
        }

        public void AddTappable(ITappable tappable)
        {
            _tappable.Add(tappable);
        }

        public void RemoveTappable(ITappable tappable)
        {
            _tappable.Remove(tappable);
        }
    }
}
