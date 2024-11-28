using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UI.WinLosePanel;

namespace UI.UIManager
{
    public class UIManager : MonoBehaviour
    {
        #region Instance

        private static UIManager _instance;

        public static UIManager Instance
        {
            get { return _instance; }
        }

        #endregion

        [Header("UI Elements")] [SerializeField]
        private WinLosePanel.WinLosePanel winLosePanel;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        private void OnEnable()
        {
            GameController.OnGameEnd += GameController_OnGameEnd;
            GameController.OnSetLevel += GameController_OnSetLevel;
        }

        private void GameController_OnGameEnd(bool isWin)
        {
            winLosePanel.OpenPanel(isWin);
        }

        private void GameController_OnSetLevel(bool isNextLevel)
        {
        }

        private void OnDisable()
        {
            GameController.OnGameEnd -= GameController_OnGameEnd;
            GameController.OnSetLevel -= GameController_OnSetLevel;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                winLosePanel.OpenPanel(true);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                winLosePanel.OpenPanel(false);
            }
        }
    }
}