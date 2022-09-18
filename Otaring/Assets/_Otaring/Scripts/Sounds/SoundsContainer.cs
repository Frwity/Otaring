using Com.RandomDudes.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.Otaring
{
    public class SoundsContainer : MonoBehaviour
    {
        [Header("Musics")]
        [SerializeField] private SoundObject _uiMusic = default;
        [SerializeField] private SoundObject _inGameMusic = default;

        [Header("SFX")]
        [SerializeField] private SoundObject _spinSound = default;
        [SerializeField] private SoundObject _pointMarked = default; //Mini explosion?
        [SerializeField] private SoundObject _playerLockInLobby = default; //Quelque chose comme dans smash
        [SerializeField] private SoundObject _gameStart = default; //Un "GOOOO"
        [SerializeField] private SoundObject _gameFinished = default; //Sifflet ?
        [SerializeField] private SoundObject _gameOverCollapseSound = default;
        [SerializeField] private SoundObject _throwBallSound = default;

        [Header("UISounds")]
        [SerializeField] private AudioSource _buttonClick = default;

        [Header("AudioSource list")]
        [SerializeField] private AudioSource[] audioSources = default;

        #region Getters
        public SoundObject UIMusic { get { return _uiMusic; } }
        public SoundObject InGameMusic { get { return _inGameMusic; } }
        public SoundObject SpinSound { get { return _spinSound; } }
        public SoundObject PointMarked { get { return _pointMarked; } }
        public SoundObject PlayerLockInLobby { get { return _playerLockInLobby; } }
        public SoundObject GameStart { get { return _gameStart; } }
        public SoundObject GameOver { get { return _gameFinished; } }
        public SoundObject GameOverCollapseSound { get { return _gameOverCollapseSound; } }
        public AudioSource ButtonClick { get { return _buttonClick; } }
        public SoundObject ThrowBallSound { get { return _throwBallSound; } }
        #endregion

        #region Singleton Instance
        static private SoundsContainer _instance;

        static public SoundsContainer Instance { 
            get { 
                return _instance; 
            }
        }

        private void Awake()
        {
            if (_instance == null) _instance = this;
            else Destroy(this);
        }
        #endregion

    }
}
