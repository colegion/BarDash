using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace UI.WinLosePanel
{
    public class WinLosePanel : MonoBehaviour
    {
        public Camera failCamera;
        [SerializeField] private RenderTexture targetTexture;
        [Header("Win Panel Values")] [SerializeField]
        private RectTransform levelTextRectTransform;

        [SerializeField] private RectTransform upperBadgeRectTransform;

        [SerializeField] private TextMeshProUGUI completeOrLoseLevelText;
        [SerializeField] private RectTransform nextOrRestartLevelButtonRectTransform;
        [SerializeField] private RectTransform shineRectTransform;


        [Header("Target Values")] [SerializeField]
        private Vector2 upperBadgeVisibleTargetPosition;

        [SerializeField] private Vector2 upperBadgeDissapearPosition;

        [Header("Button Ref")] [SerializeField]
        private Button nextOrRestartButton;

        [Header("Image Properties")] [SerializeField]
        private Sprite winEmojiSprite;

        [SerializeField] private RawImage failSlotsImage;
        [SerializeField] private GameObject outOfSpace;

        [SerializeField] private Sprite winTextSprite;
        [SerializeField] private Sprite loseTextSprite;
        [SerializeField] private Sprite loseEmojiSprite;

        [SerializeField] private Sprite nextLevelButtonSprite;
        [SerializeField] private Sprite restartLevelButtonSprite;

        [SerializeField] private Image winOrLoseEmoji;
        [SerializeField] private Image bgImage;

        [SerializeField] private Image winOrLoseTextImage;

        private bool canPlay = false;

        public void OpenPanel(bool isWin)
        {
            nextOrRestartButton.onClick.RemoveAllListeners();
            if (isWin)
            {
                failCamera.gameObject.SetActive(false);
                failSlotsImage.gameObject.SetActive(false);
                outOfSpace.gameObject.SetActive(false);
                completeOrLoseLevelText.text = "COMPLETE!";
                winOrLoseEmoji.sprite = winEmojiSprite;
                winOrLoseTextImage.sprite = winTextSprite;
                winOrLoseTextImage.SetNativeSize();
                nextOrRestartButton.image.sprite = nextLevelButtonSprite;
                nextOrRestartButton.onClick.AddListener(NextLevel);
            }
            else
            {
                failCamera.gameObject.SetActive(true);
                failCamera.targetTexture = targetTexture;
                failCamera.Render();
                Camera.main.Render();
                failSlotsImage.gameObject.SetActive(true);
                outOfSpace.gameObject.SetActive(true);
                completeOrLoseLevelText.text = "FAILED!";
                winOrLoseEmoji.sprite = loseEmojiSprite;
                winOrLoseTextImage.sprite = loseTextSprite;
                winOrLoseTextImage.SetNativeSize();
                nextOrRestartButton.image.sprite = restartLevelButtonSprite;
                nextOrRestartButton.onClick.AddListener(RestartLevel);
            }

            gameObject.SetActive(true);
            StartCoroutine(SetPanelOpenAnimation(isWin));
        }

        public void ClosePanel()
        {
        }
        
        private IEnumerator SetPanelOpenAnimation(bool isWin)
        {
            canPlay = true;
            upperBadgeRectTransform.anchoredPosition = upperBadgeDissapearPosition;
            upperBadgeRectTransform.DOAnchorPos(upperBadgeVisibleTargetPosition, 0.5f).SetEase(Ease.InBounce);
            nextOrRestartLevelButtonRectTransform.localScale = Vector3.zero;
            shineRectTransform.localScale = Vector3.zero;
            winOrLoseEmoji.rectTransform.localScale = Vector3.zero;
            levelTextRectTransform.localScale = Vector3.zero;
            completeOrLoseLevelText.rectTransform.localScale = Vector3.zero;
            winOrLoseTextImage.rectTransform.localScale = Vector3.zero;
            yield return new WaitForSeconds(0.5f);
            levelTextRectTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutElastic);
            completeOrLoseLevelText.rectTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutElastic);
            yield return new WaitForSeconds(0.5f);
            winOrLoseTextImage.rectTransform.DOScale(Vector3.one * 1.5f, 0.5f).SetEase(Ease.OutElastic);
            
            if (isWin)
            {
                StartCoroutine(PlayShineAnimation());
                winOrLoseEmoji.rectTransform.DOScale(Vector3.one * 2, 0.5f).SetEase(Ease.OutElastic);
                yield return new WaitForSeconds(0.5f);
                shineRectTransform.DOScale(Vector3.one * 2, 0.3f).SetEase(Ease.Flash);
                StartCoroutine(PlayEmojiAnimation());
            }
            
            nextOrRestartLevelButtonRectTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutElastic);
        }

        private IEnumerator SetPanelCloseAnimation()
        {
            levelTextRectTransform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.OutCubic);
            completeOrLoseLevelText.rectTransform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.OutCubic);
            yield return new WaitForSeconds(0.2f);
            upperBadgeRectTransform.DOAnchorPos(upperBadgeDissapearPosition, 0.2f).SetEase(Ease.OutCubic);
            yield return new WaitForSeconds(0.2f);
            winOrLoseTextImage.rectTransform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.OutCubic);
            shineRectTransform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.OutCubic);
            yield return new WaitForSeconds(0.2f);
            winOrLoseEmoji.rectTransform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.OutCubic);
            nextOrRestartLevelButtonRectTransform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.OutCubic);
            yield return new WaitForSeconds(0.2f);
            gameObject.SetActive(false);
             SceneManager.LoadScene("GameScene");
        }

        private IEnumerator PlayEmojiAnimation()
        {
            while (canPlay)
            {
                winOrLoseEmoji.rectTransform.DORotate(new Vector3(0, 0, -15), 1f).OnComplete(() =>
                {
                    winOrLoseEmoji.rectTransform.DORotate(new Vector3(0, 0, 15), 1f);
                });
                yield return new WaitForSeconds(3f);
            }
        }

        private IEnumerator PlayShineAnimation()
        {
            while (canPlay)
            {
                shineRectTransform.transform.Rotate(Vector3.forward, 40f * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
        }

        private void NextLevel()
        {
            canPlay = false;
            DOTween.Kill(this);
            StopCoroutine(PlayEmojiAnimation());
            StopCoroutine(PlayShineAnimation());
            StartCoroutine(SetPanelCloseAnimation());          
            GameController.Instance.SetLevel(true);
        }

        private void RestartLevel()
        {
            canPlay = false;
            DOTween.Kill(this);
            StopCoroutine(PlayEmojiAnimation());
            StopCoroutine(PlayShineAnimation());
            StartCoroutine(SetPanelCloseAnimation());
            GameController.Instance.SetLevel(false);
        }
    }
}