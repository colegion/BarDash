using System.Collections.Generic;
using UnityEngine;

namespace GoalSystem
{
    public class WaitressSlot : MonoBehaviour
    {
        [SerializeField] private Transform waitressTarget;
        private List<Drink> _drinkRefs = new List<Drink>(TargetDrinkCount);
        private const int TargetDrinkCount = 3;
        private int _currentDrinkCount = 0;
        private Waitress _waitress;
        private bool _clearingSelf;

        public int CurrentDrinkCount
        {
            get
            {
                return _currentDrinkCount;
            }

        }

        public void SetWaitressRef(Waitress waitress)
        {
            _waitress = waitress;
        }

        public Waitress GetWaitressRef()
        {
            return _waitress;
        }

        public bool IsAvailable()
        {
            return _waitress == null;
        }

        public bool AppendDrinks(Drink drink)
        {
            if (_drinkRefs.Capacity == _drinkRefs.Count || _drinkRefs.Contains(drink)) return false;
            _drinkRefs.Add(drink);
            return true;
        }

        public void IncrementReachedDrinkCount()
        {
            _currentDrinkCount++;
        }

        public bool HasCompleted()
        {
            return _drinkRefs.Capacity == _drinkRefs.Count && _currentDrinkCount == _drinkRefs.Capacity;
        }

        public Transform GetTarget()
        {
            return waitressTarget;
        }

        public void ResetSelf()
        {
            _drinkRefs.Clear();
            _waitress = null;
            _currentDrinkCount = 0;
        }
    }
}
