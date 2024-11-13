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
            return _waitress != null;
        }

        public void AppendDrinks(Drink drink)
        {
            if (_drinkRefs.Capacity == _drinkRefs.Count) return;
            _currentDrinkCount++;
            _drinkRefs.Add(drink);
        }

        public bool HasCompleted()
        {
            return _drinkRefs.Capacity == _drinkRefs.Count;
        }

        public Transform GetTarget()
        {
            return waitressTarget;
        }

        public void ResetSelf()
        {
            _waitress = null;
            _currentDrinkCount = 0;
        }
    }
}
