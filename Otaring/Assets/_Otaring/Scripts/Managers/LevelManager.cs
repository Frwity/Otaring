using Com.IsartDigital.Otaring.Gameplay.Elements.Balls;
using Com.IsartDigital.Otaring.Managers;
using Com.IsartDigital.Otaring.Gameplay.Elements;
using Com.RandomDudes;
using Com.RandomDudes.Events;
using Com.RandomDudes.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.Otaring
{
    public class LevelManager : Manager
    {
        public ActionEvent<uint, uint> OnScoreUpdated = new ActionEvent<uint, uint>();
        public ActionEvent<uint, uint> OnGameOver = new ActionEvent<uint, uint>();

        public bool CurrentlyInGame;

        [SerializeField] private Transform objectsContainer = default;
        [SerializeField] private Wall[] walls = default;
        [SerializeField] private GameObject[] levels = default;
        [SerializeField] private GameObject lobby = default;

        [SerializeField] private BallLauncher ballSpawn1 = default;
        [SerializeField] private BallLauncher ballSpawn2 = default;

        [SerializeField] private TeamObject leftGround = default;
        [SerializeField] private TeamObject rightGround = default;

        [SerializeField] private float delayBetweenEachWall = default;

        private List<Ball> balls = new List<Ball>();

        private uint rightTeamLife;
        private uint leftTeamLife;

        private bool isPlaying;
        private float gameTimer;
        private float elapsedTime;

        private bool isTeamFull;

        private void Start()
        {
            GetManager<TeamManager>().OnTeamFullValueChange += TeamManager_OnTeamFullValueChange;
            Ball.OnGroundHit += Ball_OnGroundHit;
        }

        public void StartLevel()
        {
            rightTeamLife = 3;
            leftTeamLife = 3;
            OnScoreUpdated?.Invoke(rightTeamLife, leftTeamLife);

            leftGround.SetTeam(TeamColor.BLUE);
            rightGround.SetTeam(TeamColor.RED);

            GoToLevel();
        }

        private void Update()
        {
            if (!isPlaying)
                return;

            elapsedTime += Time.deltaTime;
            gameTimer += Time.deltaTime;

            if (elapsedTime > delayBetweenEachWall)
            {
                ChooseWalls();
                elapsedTime = 0f;
            }
        }

        private void ChooseWalls()
        {
            float randomIndex = Random.Range(1f, walls.Length - 1);

            for (int i = (int)randomIndex - 1; i < randomIndex + 1; i++)
            {
                walls[i].Elevate();
            }
        }

        public void GoToLobby()
        {
            Time.timeScale = 1f;
            lobby.SetActive(true);
            lobby.GetComponent<Animator>().ResetTrigger("Hide");
        }

        private void GoToLevel()
        {
            Play();

            //Commenté car son pas intégré.
            //Manager.GetManager<SoundManager>().PlayLoopWithCrossFade(SoundsContainer.Instance.InGameMusic, 2f);

            balls.Add(ballSpawn1.CreateNewBall(new Vector3(0, 20, 0)));
            balls.Add(ballSpawn2.CreateNewBall(new Vector3(0, 20, 0)));
        }

        public void Play()
        {
            isPlaying = true;
            Time.timeScale = 1f;
        }

        public void Pause()
        {
            isPlaying = false;
            Time.timeScale = 0f;
        }

        public void GameOver()
        {
            GetManager<UIManager>().RemoveScreen<HUD>(RandomDudes.UI.AnimationScreenType.Immediate);
            GetManager<UIManager>().RemoveScreen<HUD2>(RandomDudes.UI.AnimationScreenType.Immediate);
            GetManager<UIManager>().AddScreen<GameOverScreen>().StartInteraction();

            Wall.StopWallParticles();

            OnGameOver?.Invoke(rightTeamLife, leftTeamLife);
        }

        private void PlayerManager_OnAllLockChange()
        {
            gameManager.StartLevel();
        }

        private void TeamManager_OnTeamFullValueChange(bool teamFull, int nbOfColoredPlayer)
        {
            if (teamFull && GetManager<PlayerManager>().PlayerControllers.Count == nbOfColoredPlayer)
            {
                StartCount();
                isTeamFull = true;
            }
            else if (!teamFull && isTeamFull)
            {
                isTeamFull = false;
                StopCount();
            }
        }
        private void StartCount()
        {

            GetManager<TransitionManager>().StartCountdown();
        }

        private void StopCount()
        {

            GetManager<TransitionManager>().StopCountdown();
        }

        private void Ball_OnGroundHit(TeamColor groundColor)
        {
            if (groundColor == TeamColor.BLUE)
                rightTeamLife--;
            else if (groundColor == TeamColor.RED)
                leftTeamLife--;
            OnScoreUpdated?.Invoke(leftTeamLife, rightTeamLife);

            if (rightTeamLife == 0 || leftTeamLife == 0)
            {
                Pause();
                GameOver();
            }
        }

        public void ResetManager()
        {
            CurrentlyInGame = false;
            isPlaying = false;
            gameTimer = 0f;

            for (int i = balls.Count - 1; i >= 0; i--)
                Destroy(balls[i].gameObject);

            balls = new List<Ball>();

            for (int i = 0; i < walls.Length; i++)
                walls[i].Reset();

            for (int i = objectsContainer.childCount - 1; i >= 0; i--)
                Destroy(objectsContainer.GetChild(i).gameObject);

            Wall.StopWallParticles();
        }

        override protected void OnDestroy()
        {
            base.OnDestroy();

            if (GetManager<TeamManager>() != null)
                GetManager<TeamManager>().OnTeamFullValueChange -= TeamManager_OnTeamFullValueChange;
            Ball.OnGroundHit -= Ball_OnGroundHit;
        }
    }
}
