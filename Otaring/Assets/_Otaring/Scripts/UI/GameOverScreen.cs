using Com.IsartDigital.Otaring.Otaring.UI;
using Com.RandomDudes.Managers;
using Com.RandomDudes.UI;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Com.IsartDigital.Otaring
{
    public class GameOverScreen : UIScreen
    {
        [SerializeField] private TMP_Text redFinalScore = default;
        [SerializeField] private TMP_Text blueFinalScore = default;

        [SerializeField] private TMP_Text leftBeyblade = default;
        [SerializeField] private TMP_Text rightBeyblade = default;

        [SerializeField] private Volume postProcessVolume = default;

        [SerializeField] private AudioSource collapseSound = default;

        private void Awake()
        {
            Manager.GetManager<LevelManager>().OnGameOver += LevelManager_OnGameOver;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Time.timeScale = 0;
        }

        public void EnablePostProcess()
        {
            postProcessVolume.gameObject.SetActive(true);
            postProcessVolume.weight = 0f;
            StartCoroutine(FadeInVolume(0.2f));
        }

        private IEnumerator FadeInVolume(float timeToLerp)
        {
            float elapsedTime = 0f;

            while (postProcessVolume.weight < 1)
            {
                elapsedTime += Time.unscaledDeltaTime;
                postProcessVolume.weight = Mathf.Lerp(0f, 1f, elapsedTime / timeToLerp);
                yield return null;
            }
        }

        private IEnumerator FadeOutVolume(float timeToLerp)
        {
            float elapsedTime = 0f;

            while (postProcessVolume.weight > 0)
            {
                elapsedTime += Time.unscaledDeltaTime;
                postProcessVolume.weight = Mathf.Lerp(1, 0, elapsedTime / timeToLerp);
                yield return null;
            }
        }

        public void DisablePostProcess()
        {
            postProcessVolume.gameObject.SetActive(false);
        }

        private void LevelManager_OnGameOver(uint blueScore, uint redScore)
        {
            blueFinalScore.text = blueScore.ToString();
            redFinalScore.text = redScore.ToString();

            if (blueScore == 0)
            {
                leftBeyblade.color = Color.red;
                rightBeyblade.color = Color.blue;
            }
            else
            {
                leftBeyblade.color = Color.blue;
                rightBeyblade.color = Color.red;
            }
        }

        public void MakeQuitButtonAppear()
        {
            StartAnimation("isAnimationOver");

            RemoveRandomColor();
        }

        public void PlayCollapseSound()
        {
            collapseSound.Play();
            EnablePostProcess();
            Manager.GetManager<SoundManager>().StopCurrentLoop();
        }

        public void OnQuit()
        {
            uiManager.RemoveScreen<GameOverScreen>();
            DisablePostProcess();

            for (int i = 0; i < postProcessVolume.profile.components.Count; i++)
            {
                if (postProcessVolume.profile.components[i].GetType() == typeof(ColorAdjustments))
                {
                    for (int j = 0; j < postProcessVolume.profile.components[i].parameters.Count; j++)
                    {
                        if (postProcessVolume.profile.components[i].parameters[j].GetType() == typeof(ClampedFloatParameter))
                        {
                            ClampedFloatParameter clampedFloatParameter = (ClampedFloatParameter)postProcessVolume.profile.components[i].parameters[j];

                            if (j == 1)
                            {
                                clampedFloatParameter.value = 70f;
                                postProcessVolume.profile.components[i].parameters[j].SetValue(clampedFloatParameter);
                            }
                            else if (j == 3)
                            {
                                clampedFloatParameter.value = 80f;
                                postProcessVolume.profile.components[i].parameters[j].SetValue(clampedFloatParameter);
                            }
                        }
                    }
                }
            }

            Manager.GetManager<LevelManager>().GoToLobby();
        }

        public void RemoveRandomColor()
        {
            for (int i = 0; i < postProcessVolume.profile.components.Count; i++)
            {
                if (postProcessVolume.profile.components[i].GetType() == typeof(ColorAdjustments))
                {
                    for (int j = 0; j < postProcessVolume.profile.components[i].parameters.Count; j++)
                    {
                        if (postProcessVolume.profile.components[i].parameters[j].GetType() == typeof(ClampedFloatParameter))
                        {
                            ClampedFloatParameter clampedFloatParameter = (ClampedFloatParameter)postProcessVolume.profile.components[i].parameters[j];

                            if (j == 1)
                            {
                                StartCoroutine(Lerp((ClampedFloatParameter)postProcessVolume.profile.components[i].parameters[j], clampedFloatParameter.value, 0f));
                                //clampedFloatParameter.value = 0f;
                                //postProcessVolume.profile.components[i].parameters[j].SetValue(clampedFloatParameter);
                            }
                            else if (j == 3)
                            {
                                StartCoroutine(Lerp((ClampedFloatParameter)postProcessVolume.profile.components[i].parameters[j], clampedFloatParameter.value, 0f));
                                //clampedFloatParameter.value = 0f;
                                //postProcessVolume.profile.components[i].parameters[j].SetValue(clampedFloatParameter);
                            }
                        }
                    }
                }
            }

            StartCoroutine(FadeOutVolume(0.2f));
            Manager.GetManager<SoundManager>().PlayLoopWithFadeOutFadeIn(SoundsContainer.Instance.UIMusic, 0, 0.5f);
        }

        private IEnumerator Lerp(ClampedFloatParameter valueToChange, float currentValue, float targetValue)
        {
            float timeToLerp = 0.2f;
            float elapsedTime = 0;

            ClampedFloatParameter clampedFloatParameter = valueToChange;

            while(elapsedTime < timeToLerp)
            {
                elapsedTime += Time.unscaledDeltaTime;

                clampedFloatParameter.value = Mathf.Lerp(currentValue, targetValue, elapsedTime / timeToLerp);

                valueToChange.SetValue(clampedFloatParameter);
                yield return null;
            }

            yield return null;
        }

        private void OnDestroy()
        {
            if (Manager.GetManager<LevelManager>())
                Manager.GetManager<LevelManager>().OnGameOver -= LevelManager_OnGameOver;
        }
    }
}