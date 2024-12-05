using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private GameObject levelTextObject;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button settingsButton;

    [Header("Settings Panel Properties")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject settingsPanelRaycastBlocker;
    [SerializeField] private Button settingsPanelCloseButton;
    [SerializeField] private Button restoreButton;

    private void OnEnable()
    {
        GameController.OnGameEnd += GameController_OnGameEnd;
        GameController.OnSetLevel += GameController_OnSetLevel;
    }
    private void GameController_OnGameEnd(bool isWin)
    {
        levelTextObject.SetActive(false);
        settingsButton.gameObject.SetActive(false);
        settingsPanel.SetActive(false);
        settingsPanelRaycastBlocker.SetActive(false);
    }
    private void GameController_OnSetLevel(bool isWin)
    {
        levelTextObject.SetActive(true);
        settingsButton.gameObject.SetActive(true);
    }
    private void OnDisable()
    {
        GameController.OnGameEnd -= GameController_OnGameEnd;
        GameController.OnSetLevel -= GameController_OnSetLevel;
    }

    private void Start()
    {
        Initialize();
    }
    private void Initialize()
    {
        settingsButton.onClick.AddListener(SettingsButtonFunc);
        settingsPanelCloseButton.onClick.AddListener(SettingsPanelCloseButtonFunc);

    }

    private void SettingsButtonFunc()
    {
        settingsPanel.transform.localScale = Vector3.zero;
        settingsPanel.SetActive(true);
        settingsPanel.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InCubic);
        settingsPanelRaycastBlocker.SetActive(true);

    }
    private void SettingsPanelCloseButtonFunc()
    {

        settingsPanelRaycastBlocker.SetActive(false);
        settingsPanel.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InCubic).OnComplete(() =>
        {
            settingsPanel.SetActive(false);
        });

    }
    private void RestoreButtonFunc()
    {
        //Do restore shit;

    }
}
