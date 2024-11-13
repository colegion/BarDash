using System;
using System.Collections.Generic;
using Helpers;
using UnityEngine;

namespace GoalSystem
{
    public class SlotController : MonoBehaviour
    {
        [SerializeField] private DrinkController drinkController;
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

        private bool _isCheckingMatches;

        private void CheckMatches()
        {
            // Avoid multiple simultaneous checks
            if (_isCheckingMatches) return;

            _isCheckingMatches = true;

            var cellsToCheck = drinkController.GetBottomRow();
            bool matchFound = false;

            foreach (var cell in cellsToCheck)
            {
                var drink = cell.GetTile(_drinkLayer);
        
                // Skip if there is no drink in this cell
                if (drink == null) continue;

                var color = drink.GetTileColor();
                var slot = TryGetAvailableSlot(color);
        
                if (slot != null)
                {
                    matchFound = true;

                    // Trigger the match and move the drink
                    slot.AppendDrinks((Drink)drink);
                    drink.Move(slot.GetTarget(), () =>
                    {
                        cell.SetTileNull(_drinkLayer);
                        drinkController.UpdateColumn(cell.X);

                        // After updating column, check for matches again
                        _isCheckingMatches = false;
                        CheckMatches();
                    });

                    // Exit the loop to wait for the match animation to finish before re-checking
                    break;
                }
            }

            // End the recursive checking if no matches were found in this iteration
            if (!matchFound) _isCheckingMatches = false;
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
