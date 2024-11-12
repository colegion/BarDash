using System;
using System.Collections.Generic;
using Helpers;
using UnityEngine;

namespace GoalSystem
{
    public class SlotController : MonoBehaviour
    {
        [SerializeField] private List<WaitressSlot> slots;

        private int _drinkLayer = 2;

        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void HandleSuccessfulInput(Waitress waitress)
        {
            var slot = GetAvailableSlot();
            slot.SetWaitressRef(waitress);
            waitress.SetTargetSlot(slot);
        }

        private void CheckMatches()
        {
            var cellsToCheck = GameController.Instance.GetDrinkBottomCells();

            foreach (var cell in cellsToCheck)
            {
                var drink = cell.GetTile(_drinkLayer);
                var color = drink.GetTileColor();
                var slot = TryGetAvailableSlot(color);
                if(slot != null)
                {
                    //@todo: BERKE BURDA UYGUN SLOTU BULDUK İÇECEK DE HAZIR BURDAN TRİGGERLANMALI BİŞEYLER.
                }
            }
        }

        private WaitressSlot TryGetAvailableSlot(GameColors targetColor)
        {
            return slots.Find(s => !s.IsAvailable() && s.GetWaitressRef().GetTileColor() == targetColor);
        }
        
        private WaitressSlot GetAvailableSlot()
        {
            return slots.Find(x => x.IsAvailable());
        }

        public bool IsAvailableSlotExist()
        {
            foreach (var slot in slots)
            {
                if (slot.IsAvailable())
                    return true;
            }

            return false;
        }

        private void AddListeners()
        {
            Waitress.OnSuccessfulInput += HandleSuccessfulInput;
            Waitress.OnWaitressReachedTarget += CheckMatches;
        }

        private void RemoveListeners()
        {
            Waitress.OnSuccessfulInput -= HandleSuccessfulInput;
            Waitress.OnWaitressReachedTarget -= CheckMatches;
        }
    }
}
