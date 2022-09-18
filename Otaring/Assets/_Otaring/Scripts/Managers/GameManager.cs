using Com.IsartDigital.Otaring.Managers;
using Com.IsartDigital.Otaring.Otaring.UI;
using Com.RandomDudes;
using Com.RandomDudes.Events;
using Com.RandomDudes.Managers;
using UnityEngine;

namespace Com.IsartDigital.Otaring
{
    public class GameManager : Manager
    {
        protected override void Awake()
        {
            base.Awake();

            if (gameManager == null)
                gameManager = this;
        }

        private void Start()
        {
            GetManager<UIManager>().AddScreen<TitleCard>().StartInteraction();
            UIManager.GetScreen<HUD>().gameObject.SetActive(false);
            GetManager<SoundManager>().PlayLoopWithCrossFade(SoundsContainer.Instance.UIMusic, 2f);
            GetManager<LevelManager>().OnGameOver += LevelManager_GameOver;
        }

        public void StartLevel()
        {
            GetManager<UIManager>().AddScreen<HUD>().StartInteraction();
            GetManager<UIManager>().AddScreen<HUD2>().StartInteraction();

            GetManager<LevelManager>().StartLevel();
        }

        private void LevelManager_GameOver(uint scoreA = 0, uint scoreB = 0)
        {
            ResetDatas();
        }

        public void ResetDatas()
        {
            GetManager<PlayerManager>().ResetManager();
            GetManager<TeamManager>().ResetManager();
            GetManager<LevelManager>().ResetManager();
            GetManager<TransitionManager>().ResetManager();
        }

        override protected void OnDestroy()
        {
            base.OnDestroy();
            GetManager<LevelManager>().OnGameOver -= LevelManager_GameOver;

            if (gameManager == this)
                gameManager = null;
        }
    }
}