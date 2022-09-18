using Com.RandomDudes.Managers;
using System;
using UnityEngine;

namespace Com.RandomDudes.Datas
{
    [Serializable]
    public struct SoundMixerValue
    {
        public SoundMixerType soundMixerType;
        public string parameterName;

        [SerializeField, Range(-80f, 80f)] private float volume;

        public float Volume
        {
            get => volume;
            set
            {
                volume = value;
                RefreshSoundAudioMixer();
            }
        }

        public void RefreshSoundAudioMixer() => Manager.GetManager<SoundManager>()?.ChangeExposedParam(parameterName, volume);
    }
}