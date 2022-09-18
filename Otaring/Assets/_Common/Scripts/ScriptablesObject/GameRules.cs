using Com.RandomDudes.Datas;
using Com.RandomDudes.Events;
using System;
using UnityEngine;

namespace Com.RandomDudes.ScriptableObjects
{
    [Serializable, CreateAssetMenu(fileName = "GameRules", menuName = "ScriptableObjects/Common/GameRules", order = 1)]
    public class GameRules : ScriptableObject
    {
        public ActionEvent<SoundMixerType> OnChangeValueMixer = new ActionEvent<SoundMixerType>();

        [Header("Languages")]
        [SerializeField] private readonly LanguagesData languagesDatas = null;

        [Header("Sounds")]
        [SerializeField] private readonly SoundMixerValue[] soundMixerValues = null;

        public LanguagesData LanguagesDatas { get => languagesDatas; }

        private void OnValidate() => OnEnable();

        private void OnEnable()
        {
            if (!Application.isPlaying)
                return;

            foreach (SoundMixerValue soundMixerValue in soundMixerValues)
                soundMixerValue.RefreshSoundAudioMixer();
        }

        public void ChangeLanguage(Languages language)
        {
            languagesDatas.ChangeLanguage(language);
        }

        public float GetVolumeMixer(SoundMixerType soundMixerType)
        {
            foreach (SoundMixerValue soundMixerValue in soundMixerValues)
                if (soundMixerValue.soundMixerType == soundMixerType)
                    return soundMixerValue.Volume;

            return 0;
        }

        public void SetVolumeMixer(SoundMixerType soundMixerType, float pValue)
        {
            for (int i = soundMixerValues.Length - 1; i >= 0; i--)
            {
                if (soundMixerValues[i].soundMixerType == soundMixerType)
                {
                    soundMixerValues[i].Volume = pValue;
                    OnChangeValueMixer.Invoke(soundMixerType);

                    return;
                }
            }
        }
    }
}