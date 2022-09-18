using Com.IsartDigital.Otaring.Gameplay;
using Com.IsartDigital.Otaring.Gameplay.Elements.Team;
using Com.RandomDudes.Events;
using Com.RandomDudes.Managers;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Com.IsartDigital.Otaring.Managers
{
    public class TeamManager : Manager
    {
        public ActionEvent<bool, int> OnTeamFullValueChange = new ActionEvent<bool, int>();
        public bool AreTeamsFull { get; private set; } = false;

        [SerializeField] public int minPlayersPerTeam = 4;
        [SerializeField] public int maxPlayersPerTeam = 4;

        [SerializeField] public TextMeshProUGUI bluePlayerRemaining = default;
        [SerializeField] public TextMeshProUGUI redPlayerRemaining = default;

        private Dictionary<TeamColor, List<Player>> teams;
        private Dictionary<TeamColor, TextMeshProUGUI> teamTextMesh = new Dictionary<TeamColor, TextMeshProUGUI>();

        private int numberOfPlayerInTeams;

        private void Start()
        {
            SetData();
            ResetManager();

            TeamSelector.OnPlayerTrigger += TeamSelector_OnPlayerTrigger;
        }

        private void SetData()
        {
            teamTextMesh.Add(TeamColor.RED, redPlayerRemaining);
            teamTextMesh.Add(TeamColor.BLUE, bluePlayerRemaining);

            SetText();
        }

        private void SetText()
        {
            teamTextMesh[TeamColor.RED].text = "" + minPlayersPerTeam + " players remaining";
            teamTextMesh[TeamColor.BLUE].text = "" + minPlayersPerTeam + " players remaining";
        }

        public void ResetManager()
        {
            ClearTeams();
            SetText();
            AreTeamsFull = false;
            numberOfPlayerInTeams = 0;
        }

        private void TeamSelector_OnPlayerTrigger(TeamSelector teamSelector, Player player)
        {
            if (GetManager<LevelManager>().CurrentlyInGame || teamSelector.Team == player.Team)
                return;

            RemovePlayerFromTeam(player, player.Team);
            player.SetTeam(teamSelector.Team);

            teams[player.Team].Add(player);

            ChangeText();
            CheckIfTeamsAreFull();
        }

        private void ChangeText()
        {
            int nbOfRedRemaining = minPlayersPerTeam - teams[TeamColor.RED].Count;
            int nbOfBlueRemaining = minPlayersPerTeam - teams[TeamColor.BLUE].Count;

            if (nbOfRedRemaining <= 0)
                teamTextMesh[TeamColor.RED].text = "TEAM FULL";
            else
                teamTextMesh[TeamColor.RED].text = "" + nbOfRedRemaining + " players remaining";

            if (nbOfBlueRemaining <= 0)
                teamTextMesh[TeamColor.BLUE].text = "TEAM FULL";
            else
                teamTextMesh[TeamColor.BLUE].text = "" + nbOfBlueRemaining + " players remaining";
        }

        private void CheckIfTeamsAreFull()
        {
            List<Player> blueTeam = GetTeam(TeamColor.BLUE);
            List<Player> redTeam = GetTeam(TeamColor.RED);

            numberOfPlayerInTeams = blueTeam.Count + redTeam.Count;

            AreTeamsFull = (blueTeam.Count >= minPlayersPerTeam && blueTeam.Count <= maxPlayersPerTeam && redTeam.Count >= minPlayersPerTeam && redTeam.Count <= maxPlayersPerTeam);
            OnTeamFullValueChange.Invoke(AreTeamsFull, numberOfPlayerInTeams);
        }

        private List<Player> GetTeam(TeamColor team)
        {
            return teams[team];
        }

        private void RemovePlayerFromTeam(Player player, TeamColor team)
        {
            if (teams.ContainsKey(team))
                teams[team].Remove(player);
        }

        private void ClearTeams()
        {
            teams = new Dictionary<TeamColor, List<Player>>() { { TeamColor.BLUE, new List<Player>() }, { TeamColor.RED, new List<Player>() } };
        }

        override protected void OnDestroy()
        {
            base.OnDestroy();

            TeamSelector.OnPlayerTrigger -= TeamSelector_OnPlayerTrigger;
        }
    }
}