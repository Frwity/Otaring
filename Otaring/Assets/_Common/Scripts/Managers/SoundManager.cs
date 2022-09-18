using Com.RandomDudes.Debug;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Com.RandomDudes.Managers
{
    public class SoundManager : Manager
    {
        [SerializeField] protected AudioMixer currentAudioMixer;

        //Transitions To ExposedParams
        protected Dictionary<string, Coroutine> currentCoroutinesToTransitionParam = new Dictionary<string, Coroutine>();
        protected Dictionary<string, float> currentValueToTransitionParam = new Dictionary<string, float>();

        //Loop
        protected Coroutine currentCoroutineToPlayLoop = null;
        protected Coroutine currentCoroutineToStopLoop = null;
        protected SoundObject currentLoop;
        protected SoundObject nextLoop;

        //Other Music
        protected Dictionary<SoundObject, Coroutine> currentCoroutinesToPlayOtherMusic = new Dictionary<SoundObject, Coroutine>();
        protected Dictionary<SoundObject, Coroutine> currentCoroutinesToStopOtherMusic = new Dictionary<SoundObject, Coroutine>();

        #region ExposedParams
        // ============================================================================
        //						  ***** EXPOSED PARAMS *****
        // ============================================================================
        /// <summary>
        /// Permet de récupérer un paramètre exposé
        /// </summary>
        /// <param name="exposedParam"></param>
        public float GetExposedParam(string exposedParam)
        {
            currentAudioMixer.GetFloat(exposedParam, out float result);
            return result;
        }

        /// <summary>
        /// Permet de changer un paramètre exposé.
        /// </summary>
        /// <param name="nameExposedParam"></param>
        /// <param name="nextValue"></param>
        public void ChangeExposedParam(string nameExposedParam, float nextValue)
        {
            StopCurrentCoroutineToExposedParams(nameExposedParam);
            currentAudioMixer.SetFloat(nameExposedParam, nextValue);
        }

        /// <summary>
        /// Permet de changer un paramètre exposé avec durée de transition.
        /// </summary>
        /// <param name="nameExposedParam"></param>
        /// <param name="nextValue"></param>
        /// <param name="durationTransition"></param>
        public void ChangeExposedParam(string nameExposedParam, float nextValue, float durationTransition)
        {
            StopCurrentCoroutineToExposedParams(nameExposedParam);

            currentCoroutinesToTransitionParam.Add(nameExposedParam, StartCoroutine(CoroutineToTransitionExposedParam(nameExposedParam, nextValue, durationTransition)));
            currentValueToTransitionParam.Add(nameExposedParam, nextValue);
        }

        protected IEnumerator CoroutineToTransitionExposedParam(string exposedParam, float nextValue, float durationTransition)
        {
            float lElapsedTime = 0;

            float lStartValue = GetExposedParam(exposedParam);

            while (lElapsedTime < durationTransition)
            {
                lElapsedTime += Time.deltaTime;
                currentAudioMixer.SetFloat(exposedParam, Mathf.Lerp(lStartValue, nextValue, lElapsedTime / durationTransition));

                yield return null;
            }

            ChangeExposedParam(exposedParam, nextValue);
            currentCoroutinesToTransitionParam.Remove(exposedParam);
            currentValueToTransitionParam.Remove(exposedParam);
        }

        /// <summary>
        /// Stop la currentCoroutineToTransitionParam pour l'exposedParam selectionné.
        /// </summary>
        /// <param name="nameExposedParam"></param>
        protected void StopCurrentCoroutineToExposedParams(string nameExposedParam)
        {
            if (currentCoroutinesToTransitionParam.ContainsKey(nameExposedParam))
            {
                StopCoroutine(currentCoroutinesToTransitionParam[nameExposedParam]);
                currentCoroutinesToTransitionParam.Remove(nameExposedParam);

                ChangeExposedParam(nameExposedParam, currentValueToTransitionParam[nameExposedParam]);
                currentValueToTransitionParam.Remove(nameExposedParam);
            }
        }
        #endregion

        #region Snapshot
        // ============================================================================
        //							   ***** SNAPSHOT *****
        // ============================================================================
        /// <summary>
        /// Permet de faire une transition de snapshot.
        /// </summary>
        /// <param name="snapshot"> Snapshot utilisée pour la transition </param>
        /// <param name="durationTransition"> Durée de la transition </param>
        public void SnapshotTransition(AudioMixerSnapshot snapshot, float durationTransition)
        {
            snapshot.TransitionTo(durationTransition);
        }

        /// <summary>
        /// Permet de faire une transition de snapshot.
        /// </summary>
        /// <param name="nameSnapshot"> Nom de la snapshot utilisée pour la transition </param>
        /// <param name="durationTransition"> Durée de la transition </param>
        public void SnapshotTransition(string nameSnapshot, float durationTransition)
        {
            SnapshotTransition(currentAudioMixer.FindSnapshot(nameSnapshot), durationTransition);
        }
        #endregion

        #region Play Loop
        /// <summary>
        /// Joue une loop sans transition
        /// </summary>
        /// <param name="nextSound"> SoundObject utilisé </param>
        public void PlayLoop(SoundObject nextSound)
        {
            if (ProtectionToOtherMusicAndLoop(nextSound)) return;

            StopCoroutinesToPlayLoop(nextSound);
            if (currentLoop != null) currentLoop.Sound.Stop();

            currentLoop = nextSound;
            AudioSource lAudioSource = currentLoop.Sound;

            lAudioSource.volume = currentLoop.SoundVolume;
            lAudioSource.Play();
        }

        /// <summary>
        /// Joue une loop avec une transition de type cross fade.
        /// </summary>
        /// <param name="nextSound"> SoundObject utilisé </param>
        /// <param name="durationTransition"> Durée du CrossFade </param>
        public void PlayLoopWithCrossFade(SoundObject nextSound, float durationTransition)
        {
            if (ProtectionToOtherMusicAndLoop(nextSound)) return;

            StopCoroutinesToPlayLoop(nextSound);

            nextLoop = nextSound;
            currentCoroutineToPlayLoop = StartCoroutine(CoroutinePlayLoopWithCrossFade(durationTransition));
        }

        protected IEnumerator CoroutinePlayLoopWithCrossFade(float durationTransition)
        {
            bool lCurrentLoopExist = currentLoop != null;

            AudioSource lCurrentSound = null;
            float lCurrentVolume = 0;

            if (lCurrentLoopExist)
            {
                lCurrentVolume = currentLoop.SoundVolume;
                lCurrentSound = currentLoop.Sound;
            }

            AudioSource lNextSound = nextLoop.Sound;
            float lNextVolume = nextLoop.SoundVolume;

            lNextSound.volume = 0;
            lNextSound.Play();

            float lElapsedTime = 0;
            float lRatio;

            while (lElapsedTime < durationTransition)
            {
                lElapsedTime += Time.unscaledDeltaTime;

                lRatio = lElapsedTime / durationTransition;

                if (lCurrentLoopExist) lCurrentSound.volume = Mathf.Lerp(lCurrentVolume, 0, lRatio);
                lNextSound.volume = Mathf.Lerp(0, lNextVolume, lRatio);

                yield return null;
            }

            if (lCurrentLoopExist)
            {
                lCurrentSound.volume = lCurrentVolume;
                lCurrentSound.Stop();
            }

            lNextSound.volume = lNextVolume;
            currentLoop = nextLoop;
            nextLoop = null;
            currentCoroutineToPlayLoop = null;
        }

        /// <summary>
        /// Joue une loop avec une transition de type FadeOut FadeIn.
        /// </summary>
        /// <param name="nextSound"> SoundObject utilisé </param>
        /// <param name="durationTransitionFadeOut"> Durée du FadeOut </param>
        /// <param name="durationTransitionFadeIn"> Durée du FadeIn </param>
        public void PlayLoopWithFadeOutFadeIn(SoundObject nextSound, float durationTransitionFadeOut, float durationTransitionFadeIn)
        {
            if (ProtectionToOtherMusicAndLoop(nextSound)) return;

            StopCoroutinesToPlayLoop(nextSound);

            nextLoop = nextSound;
            currentCoroutineToPlayLoop = StartCoroutine(CoroutinePlayLoopWithFadeOutFadeIn(durationTransitionFadeOut, durationTransitionFadeIn));
        }

        protected IEnumerator CoroutinePlayLoopWithFadeOutFadeIn(float durationTransitionFadeOut, float durationTransitionFadeIn)
        {
            float lElapsedTime = 0;
            float lVolume;
            AudioSource lAudioSource;

            if (currentLoop != null)
            {
                lVolume = currentLoop.SoundVolume;
                lAudioSource = currentLoop.Sound;

                while (lElapsedTime < durationTransitionFadeOut)
                {
                    lElapsedTime += Time.deltaTime;
                    lAudioSource.volume = Mathf.Lerp(lVolume, 0, lElapsedTime / durationTransitionFadeOut);

                    yield return null;
                }

                lAudioSource.Stop();
                lAudioSource.volume = lVolume;

                lElapsedTime = 0;
            }

            lVolume = nextLoop.SoundVolume;
            lAudioSource = nextLoop.Sound;
            lAudioSource.Play();

            while (lElapsedTime < durationTransitionFadeOut)
            {
                lElapsedTime += Time.deltaTime;
                lAudioSource.volume = Mathf.Lerp(0, lVolume, lElapsedTime / durationTransitionFadeOut);

                yield return null;
            }

            lAudioSource.volume = lVolume;
            currentLoop = nextLoop;
            nextLoop = null;
            currentCoroutineToPlayLoop = null;
        }
        #endregion

        #region Stop Loop
        /// <summary>
        /// Stop l'actuel loop sans transition.
        /// </summary>
        public void StopCurrentLoop()
        {
            StopCurrentCoroutineLoop();
            nextLoop = null;

            if (currentLoop == null)
            {
                DevLog.Message("[SoundManager] No Loop");
                return;
            }

            currentLoop.Sound.Stop();
            currentLoop = null;
        }

        /// <summary>
        /// Stop l'actuel loop avec une transition.
        /// </summary>
        /// <param name="durationTransition"></param>
        public void StopCurrentLoopWithFadeOut(float durationTransition)
        {
            StopCurrentCoroutineLoop();
            nextLoop = null;

            if (currentLoop == null)
            {
                DevLog.Message("[SoundManager] No Loop");
                return;
            }

            currentCoroutineToStopLoop = StartCoroutine(CoroutineStopLoop(durationTransition));
        }

        protected IEnumerator CoroutineStopLoop(float durationTransition)
        {
            float lCurrentVolume = currentLoop.SoundVolume;
            AudioSource lCurrentSound = currentLoop.Sound;

            float lElapsedTime = 0;

            while (lElapsedTime < durationTransition)
            {
                lElapsedTime += Time.deltaTime;

                lCurrentSound.volume = Mathf.Lerp(lCurrentVolume, 0, lElapsedTime / durationTransition);

                yield return null;
            }

            lCurrentSound.volume = lCurrentVolume;
            lCurrentSound.Stop();
            currentLoop = null;

            currentCoroutineToStopLoop = null;
        }
        #endregion

        #region StopCoroutineLoop
        /// <summary>
        /// Permet de stopper les coroutines de loop mais aussi les coroutines de other music
        /// </summary>
        /// <param name="loop">SoundObject</param>
        protected void StopCoroutinesToPlayLoop(SoundObject loop)
        {
            StopCoroutineOtherMusic(loop);
            StopCurrentCoroutineLoop();
        }

        /// <summary>
        /// Arrête les currentCoroutineToPlayLoop et currentCoroutineToStopLoop, si elles sont actives.
        /// </summary>
        protected void StopCurrentCoroutineLoop()
        {
            AudioSource lAudioSource;

            if (currentCoroutineToPlayLoop != null)
            {
                StopCoroutine(currentCoroutineToPlayLoop);
                currentCoroutineToPlayLoop = null;

                if (currentLoop != null)
                {
                    lAudioSource = currentLoop.Sound;

                    lAudioSource.volume = currentLoop.SoundVolume;
                    lAudioSource.Stop();
                }

                nextLoop.Sound.volume = nextLoop.SoundVolume;
                currentLoop = nextLoop;
            }
            else if (currentCoroutineToStopLoop != null)
            {
                StopCoroutine(currentCoroutineToStopLoop);
                currentCoroutineToStopLoop = null;

                lAudioSource = currentLoop.Sound;

                lAudioSource.volume = currentLoop.SoundVolume;
                lAudioSource.Stop();
                currentLoop = null;
            }
        }
        #endregion

        #region Other Music
        /// <summary>
        /// Permet de jouer une musique qui n'est pas utilisé en tant que loop.
        /// </summary>
        /// <param name="music"> Le SoundObject utilisé </param>
        public void PlayOtherMusic(SoundObject music)
        {
            if (ProtectionToOtherMusicAndLoop(music)) return;

            StopCoroutineOtherMusic(music);
            AudioSource lAudioSource = music.Sound;

            lAudioSource.volume = music.SoundVolume;
            lAudioSource.Play();
        }

        /// <summary>
        /// Permet de jouer une musique avec un FadeIn qui n'est pas utilisé en tant que loop.
        /// </summary>
        /// <param name="music"> Le SoundObject utilisé </param>
        /// <param name="durationTransition"> La durée du FadeIn </param>
        public void PlayOtherMusic(SoundObject music, float durationTransition)
        {
            if (ProtectionToOtherMusicAndLoop(music)) return;

            StopCoroutineOtherMusic(music);
            currentCoroutinesToPlayOtherMusic.Add(music, StartCoroutine(CoroutinePlayOtherMusic(music, durationTransition)));
        }

        /// <summary>
        /// Coroutine de FadeIn pour les autres music.
        /// </summary>
        protected IEnumerator CoroutinePlayOtherMusic(SoundObject music, float durationTransition)
        {
            AudioSource lMusic = music.Sound;
            float lVolume = music.SoundVolume;

            lMusic.volume = 0;
            lMusic.Play();

            float lElapsedTime = 0;

            while (lElapsedTime < durationTransition)
            {
                lElapsedTime += Time.deltaTime;
                lMusic.volume = Mathf.Lerp(0, lVolume, lElapsedTime / durationTransition);

                yield return null;
            }

            lMusic.volume = lVolume;
            currentCoroutinesToPlayOtherMusic.Remove(music);
        }

        /// <summary>
        /// Permet de stop une musique qui n'est pas utilisé en tant que loop.
        /// </summary>
        /// <param name="music"> Le SoundObject utilisé </param>
        public void StopOtherMusic(SoundObject music)
        {
            if (ProtectionToOtherMusicAndLoop(music)) return;

            StopCoroutineOtherMusic(music);
            music.Sound.Stop();
        }

        /// <summary>
        /// Permet de stop une musique avec un FadeOut qui n'est pas utilisé en tant que loop.
        /// </summary>
        /// <param name="music"> Le SoundObject utilisé </param>
        /// <param name="durationTransition"> La durée du FadeOut </param>
        public void StopOtherMusic(SoundObject music, float durationTransition)
        {
            if (ProtectionToOtherMusicAndLoop(music)) return;

            StopCoroutineOtherMusic(music);
            currentCoroutinesToStopOtherMusic.Add(music, StartCoroutine(CoroutineStopOtherMusic(music, durationTransition)));
        }

        /// <summary>
        /// Coroutine de FadeOut pour les autres music
        /// </summary>
        protected IEnumerator CoroutineStopOtherMusic(SoundObject music, float durationTransition)
        {
            AudioSource lMusic = music.Sound;
            float lVolume = music.SoundVolume;

            float lElapsedTime = 0;

            while (lElapsedTime < durationTransition)
            {
                lElapsedTime += Time.deltaTime;
                lMusic.volume = Mathf.Lerp(lVolume, 0, lElapsedTime / durationTransition);

                yield return null;
            }

            lMusic.volume = lVolume;
            lMusic.Stop();
            currentCoroutinesToStopOtherMusic.Remove(music);
        }

        /// <summary>
        /// Permet de stopper une coroutine pour une autre music, si on l'arrête ou la play alors que la coroutine est en cours d'éxecution.
        /// </summary>
        /// <param name="music"> Le SoundObject utilisé </param>
        protected void StopCoroutineOtherMusic(SoundObject music)
        {
            AudioSource lAudioSource;

            if (currentCoroutinesToPlayOtherMusic.ContainsKey(music))
            {
                lAudioSource = music.Sound;
                lAudioSource.volume = music.SoundVolume;
                StopCoroutine(currentCoroutinesToPlayOtherMusic[music]);
                currentCoroutinesToPlayOtherMusic.Remove(music);
            }
            else if (currentCoroutinesToStopOtherMusic.ContainsKey(music))
            {
                lAudioSource = music.Sound;
                lAudioSource.volume = music.SoundVolume;
                lAudioSource.Stop();
                StopCoroutine(currentCoroutinesToStopOtherMusic[music]);
                currentCoroutinesToStopOtherMusic.Remove(music);
            }
        }
        #endregion

        #region Protection
        /// <summary>
        /// Protection pour empêcher de lancer deux fois la même loop
        /// </summary>
        /// <param name="music"> Le SoundObject testé </param>
        protected bool ProtectionToOtherMusicAndLoop(SoundObject music)
        {
            if ((music == currentLoop && nextLoop == null) || music == nextLoop)
            {
                DevLog.Warning("[SoundManager] The music is currently used for the loop or the loop transition");
                return true;
            }

            return false;
        }
        #endregion
    }

    /// <summary>
    /// Classe serializable pour pouvoir utiliser le SoundManager.
    /// </summary>
    [Serializable]
    public class SoundObject
    {
        [SerializeField] protected AudioSource _sound;
        public AudioSource Sound => _sound;

        [SerializeField, Range(0, 1f)] protected float _soundVolume = 1f;
        public float SoundVolume => _soundVolume;
    }
}