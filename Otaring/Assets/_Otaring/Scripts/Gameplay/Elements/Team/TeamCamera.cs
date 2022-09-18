using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.Otaring.Gameplay.Elements
{
    public class TeamCamera : MonoBehaviour
    {
        private static Dictionary<TeamColor, TeamCamera> teamToCamera = new Dictionary<TeamColor, TeamCamera>();

        [SerializeField] private TeamColor teamColor = default;

        public static TeamCamera GetTeamCamera(TeamColor teamColor)
        {
            return teamToCamera[teamColor];
        }

        private void Awake()
        {
            teamToCamera.Add(teamColor, this);
        }

        private void OnDestroy()
        {
            teamToCamera.Remove(teamColor);
        }
    }
}