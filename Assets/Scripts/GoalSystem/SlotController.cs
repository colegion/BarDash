using System;
using System.Collections.Generic;
using Helpers;
using UnityEngine;

namespace GoalSystem
{
    public class SlotController : MonoBehaviour
    {
        [SerializeField] private Transform completedWaitressTarget;
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
            if (_isCheckingMatches) return; // Prevent duplicate calls.
            _isCheckingMatches = true;

            var cellsToCheck = drinkController.GetBottomRow();
            bool matchFound = false;

            foreach (var cell in cellsToCheck)
            {
                var drink = cell.GetTile(_drinkLayer);
                if (drink == null) continue;

                var color = drink.GetTileColor();
                var slot = TryGetMatchingSlot(color);

                if (slot != null)
                {
                    matchFound = true;

                    if (slot.AppendDrinks((Drink)drink))
                    {
                        drink.Move(slot.GetTarget(), () =>
                        {
                            drink.gameObject.SetActive(false);
                            cell.SetTileNull(_drinkLayer);

                            slot.IncrementReachedDrinkCount();
                            drinkController.UpdateColumn(cell.X, () =>
                            {
                                if (slot.HasCompleted())
                                {
                                    var waitress = slot.GetWaitressRef();
                                    GameController.Instance.WaitressMadeFinalMovement(waitress, slot);
                                    slot.ResetSelf();
                                    waitress.HandleFinalMovement(completedWaitressTarget, () =>
                                    {
                                        _isCheckingMatches = false;
                                        CheckMatches(); // Continue after waitress movement completes.
                                    });
                                }
                                else
                                {
                                    _isCheckingMatches = false;
                                    CheckMatches(); // Retry matches after grid stabilizes.
                                }
                            });
                        });
                        return; // Avoid further checks until this process completes.
                    }
                }
            }

            // If no matches are found, finalize state.
            if (!matchFound)
            {
                _isCheckingMatches = false;
                CheckGameEndCondition();
            }
        }

        private void CheckGameEndCondition()
        {
            int currentWaitressCount = 0;
            foreach (var slot in slots)
            {
                if (slot.GetWaitressRef() != null)
                {
                    currentWaitressCount++;
                }
            }

            if (currentWaitressCount == slots.Count)
            {
                GameController.Instance.GameEnd(false);
            }
        }


        private WaitressSlot TryGetMatchingSlot(GameColors targetColor)
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