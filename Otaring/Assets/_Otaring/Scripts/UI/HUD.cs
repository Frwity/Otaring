using Com.RandomDudes.Managers;
using Com.RandomDudes.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Com.IsartDigital.Otaring
{
    public class HUD : UIScreen
    {
        [SerializeField] protected List<GameObject> lifeList = default;

        protected void Awake()
        {
            Manager.GetManager<LevelManager>().OnScoreUpdated += LevelManager_OnScoreUpdated;
        }

        protected void LevelManager_OnTimerUpdated(float timer)
        {
            
        }

        virtual protected void LevelManager_OnScoreUpdated(uint leftTeamLife, uint rightTeamLife)
        {
            for (int i = 0; i < 3; i++)
            {
                if (i > (int)rightTeamLife - 1)
                    lifeList[i].SetActive(false);
                else
                    lifeList[i].SetActive(true);
            }
        }

        public void OnPauseButton()
        {
            Manager.GetManager<LevelManager>().Pause();

            uiManager.RemoveScreen<HUD>();
            uiManager.AddScreen<PauseScreen>().StartInteraction();
        }

        protected void OnDestroy()
        {
            if (Manager.GetManager<LevelManager>())
                Manager.GetManager<LevelManager>().OnScoreUpdated -= LevelManager_OnScoreUpdated;
        }
    }
}