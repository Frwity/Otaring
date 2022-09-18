using Com.RandomDudes.Debug;
using Com.RandomDudes.Managers;
using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Com.RandomDudes.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIScreen : MonoBehaviour
    {
        [Header("UIScreen")]
        [SerializeField] protected TextMeshProUGUI[] textsToTranslate = null;
        [SerializeField] protected float timeToFade = 0.3f;

        protected UIManager uiManager;

        private Animator animator;
        private Action callbackAnimator;
        private CanvasGroup canvasGroup;
        private LocalizationManager localizationManager;

        virtual protected void OnEnable()
        {
            TranslateTexts();
        }

        public void Init(UIManager uiManager)
        {
            UIManager.AddScreenToDictionnary(this);

            canvasGroup = GetComponent<CanvasGroup>();
            animator = GetComponent<Animator>();

            canvasGroup.alpha = 0;
            this.uiManager = uiManager;

            localizationManager = Manager.GetManager<LocalizationManager>();

            if (localizationManager != null)
                localizationManager.LanguagesData.OnLanguageChanged.AddListener(LanguagesData_OnLanguageChanged);
        }

        virtual public void StartInteraction() => canvasGroup.blocksRaycasts = true;

        virtual public void StopInteraction() => canvasGroup.blocksRaycasts = false;

        #region Localization

        private void LanguagesData_OnLanguageChanged() => TranslateTexts();

        private void TranslateTexts()
        {
            if (localizationManager != null)
                for (int i = textsToTranslate.Length - 1; i >= 0; i--)
                    textsToTranslate[i].text = localizationManager.GetTranslatedLanguage(textsToTranslate[i].text);
        }

        #endregion Localization

        #region Animator

        virtual public void StartAnimation(string triggerName, Action callback = null)
        {
            if (animator == null)
            {
                callback?.Invoke();

                DevLog.Warning("Missing Animator on: " + gameObject.name);
            }

            animator.SetTrigger(triggerName);
            callbackAnimator = callback;
        }

        public void EndAnimation()
        {
            callbackAnimator?.Invoke();
            callbackAnimator = null;
        }

        #endregion

        #region Tween

        virtual public void OpenScreenTween(Action callback = null)
        {
            gameObject.SetActive(true);
            canvasGroup.alpha = 0;

            StopInteraction();

            canvasGroup.DOFade(1, timeToFade).OnComplete(() => callback?.Invoke());
        }

        virtual public void CloseScreenTween(Action callback = null)
        {
            StopInteraction();

            canvasGroup.DOFade(0, timeToFade).OnComplete(() => { gameObject.SetActive(false); callback?.Invoke(); });
        }

        #endregion

        #region Immediate

        virtual public void OpenImmediate()
        {
            gameObject.SetActive(true);
            canvasGroup.alpha = 1;
        }

        virtual public void CloseImmediate()
        {
            gameObject.SetActive(false);
            canvasGroup.alpha = 0;

            StopInteraction();
        }

        #endregion

        #region Coroutine

        virtual public void OpenCoroutine(Action callback = null)
        {
            gameObject.SetActive(true);

            StartCoroutine(OpenScreenCoroutine(callback));
        }

        virtual public void CloseCoroutine(Action callback = null)
        {
            gameObject.SetActive(false);
            StopInteraction();

            StartCoroutine(CloseScreenCoroutine(callback));
        }

        virtual protected IEnumerator OpenScreenCoroutine(Action callback = null)
        {
            StopAllCoroutines();

            canvasGroup.alpha = 1;
            callback?.Invoke();

            yield return null;
        }

        virtual protected IEnumerator CloseScreenCoroutine(Action callback = null)
        {
            StopAllCoroutines();

            canvasGroup.alpha = 0;
            callback?.Invoke();

            yield return null;
        }

        #endregion

        private void OnDestroy()
        {
            UIManager.RemoveScreenFromDictionnary(this);

            localizationManager.LanguagesData.OnLanguageChanged.RemoveListener(LanguagesData_OnLanguageChanged);
        }
    }
}