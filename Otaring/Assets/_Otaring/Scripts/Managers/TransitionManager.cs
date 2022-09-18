using System.Collections;
using System.Collections.Generic;
using Com.RandomDudes.Managers;
using TMPro;
using UnityEngine;

namespace Com.IsartDigital.Otaring.Managers
{
    public class TransitionManager : Manager
    {
        [SerializeField] private GameObject overlay = default;
        [SerializeField] private TextMeshProUGUI timerText = default;
        [SerializeField] private AnimationCurve animationCurve = default;

        [SerializeField] private Animator transitionAnimator = default;
        [SerializeField] private Animator lobbyAnimator = default;
        [SerializeField] private Animator cameraAnimator = default;

        [SerializeField] private float secondsBeforeStartLevel = 10;

        private Coroutine countdown;
        private Coroutine bounceCoroutine;

        private void Start()
        {
            GetManager<LevelManager>().OnGameOver += LevelManager_OnGameOver;
        }
        public void ResetManager()
        {
            overlay.SetActive(false);
            cameraAnimator.SetTrigger("Assemble");
        }
        public void StartCountdown()
        {
            if (countdown != null) StopCoroutine(countdown);
            countdown = StartCoroutine(Countdown());
            transitionAnimator.SetTrigger("Show");
        }

        public void StopCountdown()
        {
            if (countdown != null) StopCoroutine(countdown);
            transitionAnimator.SetTrigger("Hide");
        }

        private IEnumerator Countdown()
        {
            bool hasStartedMusic = false;
            float timer = secondsBeforeStartLevel +1;

            while (timer > 1)
            {
                timer -= Time.deltaTime;

                if (timer < 3 && !hasStartedMusic)
                {
                    GetManager<SoundManager>().PlayLoopWithCrossFade(SoundsContainer.Instance.InGameMusic, 3);
                    hasStartedMusic = true;
                }

                if(timerText.text != "" + (int)timer)
                {
                    BounceText();
                    timerText.text = "" + (int)timer;
                }

                yield return null;
            }

            GetManager<LevelManager>().CurrentlyInGame = true;
            transitionAnimator.SetTrigger("Hide");
            yield return new WaitForSeconds(0.5f);

            lobbyAnimator.ResetTrigger("Show");
            lobbyAnimator.SetTrigger("Hide");
            cameraAnimator.SetTrigger("Split");
            overlay.SetActive(true);

            yield return new WaitForSeconds(1);
            GetManager<GameManager>().StartLevel();
        }

        private void BounceText()
        {
            if (bounceCoroutine != null) StopCoroutine(bounceCoroutine);
            bounceCoroutine = StartCoroutine(Bounce());
        }

        private IEnumerator Bounce()
        {
            float timeToBounce =0.5f;
            float elapsedTime = 0;
            float scale;
            while (elapsedTime < timeToBounce)
            {
                elapsedTime += Time.deltaTime;
                scale = animationCurve.Evaluate(elapsedTime / timeToBounce);
                timerText.transform.localScale = new Vector3(scale, scale);
                yield return null;
            }
            yield break;
        }

        private void LevelManager_OnGameOver(uint a =0, uint b = 0)
        {
            cameraAnimator.SetTrigger("Assemble");
        }

        protected override void OnDestroy()
        {
            if(GetManager<LevelManager>() !=null) GetManager<LevelManager>().OnGameOver -= LevelManager_OnGameOver;
        }
    }
}
