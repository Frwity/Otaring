using Com.IsartDigital.Otaring.Otaring.UI;
using Com.RandomDudes;
using Com.RandomDudes.Managers;
using Com.RandomDudes.UI;

namespace Com.IsartDigital.Otaring
{
    public class PauseScreen : UIScreen
    {
        public void OnResumeButton()
        {
            Manager.GetManager<LevelManager>().Play();

            uiManager.RemoveScreen<PauseScreen>();
            uiManager.AddScreen<HUD>().StartInteraction();
        }

        public void OnRestartButton()
        {
            uiManager.RemoveScreen<PauseScreen>();

            Manager.GetManager<GameManager>().ResetDatas();
            Manager.GetManager<LevelManager>().GoToLobby();
        }

        public void OnQuitButton()
        {
            uiManager.RemoveScreen<PauseScreen>();
            uiManager.RemoveScreen<HUD>(AnimationScreenType.Immediate);
            uiManager.RemoveScreen<HUD2>(AnimationScreenType.Immediate);
            Manager.GetManager<GameManager>().ResetDatas();

            uiManager.AddScreen<TitleCard>(AnimationScreenType.Animator, "Open").StartInteraction();
        }
    }
}
