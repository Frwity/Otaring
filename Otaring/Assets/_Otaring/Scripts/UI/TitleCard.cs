using Com.RandomDudes.Managers;
using Com.RandomDudes.UI;
using UnityEngine;

namespace Com.IsartDigital.Otaring.Otaring.UI
{
    public class TitleCard : UIScreen
    {
        [SerializeField] private AudioSource click = default;

        public void OnPlayButton()
        {
            Manager.GetManager<LevelManager>().GoToLobby();

            SoundsContainer.Instance.ButtonClick.Play();

            uiManager.RemoveScreen<TitleCard>();

            uiManager.AddScreen<HUD>().StartInteraction();
            uiManager.AddScreen<HUD2>().StartInteraction();
        }
    }
}