using System;
using System.Collections;
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

        private bool _isCheckingMatches;
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


        private void HandleOnWaitressReachedSlot(Waitress waitress)
        {
            if (waitress.IsMoving()) return;
            _isCheckingMatches = true;
            var cellsToCheck = drinkController.GetBottomRow();

            foreach (var cell in cellsToCheck)
            {
                var drink = cell.GetTile(_drinkLayer);
                if(drink == null || drink.IsMoving()) continue;

                var color = drink.GetTileColor();
                if (waitress.GetTileColor() == color)
                {
                    var slot = GetSlotByWaitressRef(waitress);
                    if (slot != null)
                    {
                        if (slot.AppendDrinks((Drink)drink))
                        {
                            drink.SetIsMoving(true);
                            drink.Move(waitress.GetTraySlot(), () =>
                            {
                                drink.SetIsMoving(false);
                                drink.GetComponent<Drink>().SetParent(waitress.GetTray());
                                drink.GetComponent<Drink>().SetScale();
                                cell.SetTileNull(_drinkLayer);
                                
                                drinkController.UpdateColumn(cell.X, () =>
                                {
                                    slot.IncrementReachedDrinkCount();
                                    if (slot.HasCompleted())
                                    {
                                        
                                        slot.ResetSelf();
                                        waitress.HandleFinalMovement(completedWaitressTarget, () =>
                                        {
                                            CheckConsecutiveMatches();
                                            GameController.Instance.WaitressMadeFinalMovement(waitress, slot);
                                        });
                                    }
                                    else
                                    {
                                        CheckConsecutiveMatches();
                                    }
                                });
                            });
                        }
                    }
                }
            }
            
            _isCheckingMatches = false;
        }

        private List<Waitress> GetReadyWaitresses()
        {
            List<Waitress> waitresses = new List<Waitress>();
            foreach (var slot in slots)
            {
                var waitress = slot.GetWaitressRef();
                if (waitress != null)
                {
                    waitresses.Add(waitress);
                }
            }

            return waitresses;
        }

        private void CheckConsecutiveMatches()
        {
            StartCoroutine(CheckConsecutiveMatchesCoroutine());
        }

        private IEnumerator CheckConsecutiveMatchesCoroutine()
        {
            var waitresses = GetReadyWaitresses();

            foreach (var waitress in waitresses)
            {
                while (waitress.IsMoving()) 
                {
                    yield return null;
                }
                HandleOnWaitressReachedSlot(waitress);
                
                yield return new WaitForSeconds(0.25f);
            }
            
            CheckGameEndCondition();
        }

        private WaitressSlot GetSlotByWaitressRef(Waitress waitress)
        {
            foreach (var slot in slots)
            {
                if (slot.GetWaitressRef() == waitress)
                {
                    return slot;
                }
            }

            return null;
        }

        private void CheckGameEndCondition()
        {
            int currentWaitressCount = 0;
            foreach (var slot in slots)
            {
                var waitress = slot.GetWaitressRef();
                if (waitress != null && !waitress.IsMoving() && !_isCheckingMatches)
                {
                    currentWaitressCount++;
                }
            }

            if (currentWaitressCount == slots.Count)
            {
                GameController.Instance.GameEnd(false);
            }
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
            Waitress.OnWaitressReachedTarget += HandleOnWaitressReachedSlot;
        }

        private void RemoveListeners()
        {
            Waitress.OnSuccessfulInput -= HandleSuccessfulInput;
            Waitress.OnWaitressReachedTarget -= HandleOnWaitressReachedSlot;
        }
    }
}