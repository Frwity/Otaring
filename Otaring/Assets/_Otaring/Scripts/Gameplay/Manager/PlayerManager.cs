using Com.IsartDigital.Otaring.Gameplay;
using Com.RandomDudes.Events;
using Com.RandomDudes.Managers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Switch;
using nn.hid;
using Com.RandomDudes;

namespace Com.IsartDigital.Otaring.Managers
{
    public class PlayerManager : Manager
    {
        public List<PlayerController> PlayerControllers { get; private set; } = new List<PlayerController>();
        private List<Player> players = new List<Player>();

        public PlayerInputManager inputManager = default;

        [SerializeField] private Player playerPrefab = default;
        [SerializeField] private Transform objectsContainer = default;

        enum MaxPlayerType : byte
        {
            Four = 4,
            Eight = 8
        }

        [SerializeField] MaxPlayerType maxPlayer = MaxPlayerType.Four;

        private int numberOfPlayerLocked;

        private void Start()
        {
            ResetManager();

#if UNITY_SWITCH && !UNITY_EDITOR

                StartSwitch();
            
#endif

        }

        public void OnNewInput(PlayerInput input)
        {
            if (players.Count >= 4)
                maxPlayer = MaxPlayerType.Eight;

            bool bValidInput = true;

#if UNITY_SWITCH && !UNITY_EDITOR
                bValidInput = CheckSwitchDevice(input);
#endif
            if (bValidInput && !GetManager<LevelManager>().CurrentlyInGame && Time.timeScale > 0)
                CreateNewInputHandler(input);
            else
                Destroy(input.gameObject);
        }

        public void ResetManager()
        {
            for (int i = 0; i < PlayerControllers.Count; i++)
                Destroy(PlayerControllers[i].gameObject);

            PlayerControllers = new List<PlayerController>();

            for (int i = 0; i < players.Count; i++)
                Destroy(players[i].gameObject);
            
            players = new List<Player>();
        }

        private void CreateNewInputHandler(PlayerInput input)
        {
            PlayerController playerInputHandler = input.GetComponent<PlayerController>();

            playerInputHandler.transform.SetParent(objectsContainer);
            playerInputHandler.OnStartPress += OnPlayerPause;
            PlayerControllers.Add(playerInputHandler);

            Vector3 spawnPosition = transform.position;

            while (Physics.CheckSphere(spawnPosition, 2f, LayerMask.GetMask("Player")))
                spawnPosition += Vector3.right;

            players.Add(Instantiate(playerPrefab, spawnPosition, Quaternion.identity));
            players[players.Count - 1].SetPlayerController(playerInputHandler);
            players[players.Count - 1].SetPlayerNumber(players.Count);

#if UNITY_SWITCH && !UNITY_EDITOR
            if (players.Count > 6) GetManager<TeamManager>().minPlayersPerTeam = 4;
            else if (players.Count > 4) GetManager<TeamManager>().minPlayersPerTeam = 3;
            else GetManager<TeamManager>().minPlayersPerTeam = 2;
#endif
        }

        override protected void OnDestroy()
        {
            base.OnDestroy();

            PlayerControllers = null;
        }

        private void OnPlayerPause()
        {
            if (GetManager<LevelManager>().CurrentlyInGame)
            {
                if (Time.timeScale > 0)
                {
                    GetManager<UIManager>().AddScreen<PauseScreen>().StartInteraction();
                    GetManager<LevelManager>().Pause();
                }
                else
                    GetManager<UIManager>().AddScreen<PauseScreen>().OnResumeButton();
            }
        }

#if UNITY_SWITCH && !UNITY_EDITOR
        private void StartSwitch() // Initialize controllers for the switch
        {
            Npad.Initialize();
            Npad.SetSupportedStyleSet(NpadStyle.FullKey | NpadStyle.JoyDual | NpadStyle.JoyLeft | NpadStyle.JoyRight | NpadStyle.Handheld);
            NpadJoy.SetHoldType(NpadJoyHoldType.Horizontal);
            NpadJoy.SetHandheldActivationMode(NpadHandheldActivationMode.None);
            NpadId[] npadIds = { NpadId.Handheld, NpadId.No1, NpadId.No2, NpadId.No3, NpadId.No4, NpadId.No5, NpadId.No6, NpadId.No7, NpadId.No8 };
            Npad.SetSupportedIdType(npadIds);

            SetDevicesUsageSwitch();
        }

        public void SetDevicesUsageSwitch()
        {
            // Set the Arguments For the Applet
            ControllerSupportArg controllerSupportArgs = new ControllerSupportArg();
            controllerSupportArgs.SetDefault();

            controllerSupportArgs.playerCountMax = (byte)maxPlayer; // This will show 4 or 8 controller setup boxes in the in the applet 
            controllerSupportArgs.playerCountMin = 4; // Applet requires at least 4 players

            // Suspend Unity Processes to Call the Applet 
            UnityEngine.Switch.Applet.Begin(); //call before calling a system applet to stop all Unity threads (including audio and networking)

            // Call the Applet
            nn.hid.ControllerSupport.Show(controllerSupportArgs);

            // Resume the Suspended Unity Processes
            UnityEngine.Switch.Applet.End();

        }

        bool CheckSwitchDevice(PlayerInput input) //Check if the player is using a two joycon and if both are on
        {
            NPad pad = (NPad)input.devices[0];

            if (pad.styleMask == NPad.NpadStyles.JoyDual
               && (!pad.isRightConnected || !pad.isLeftConnected))
            {
                Debug.Log("Only 1 joycon is connected");

                SetDevicesUsageSwitch();

                return false;
            }
            return true;
        }

#endif
    }
}