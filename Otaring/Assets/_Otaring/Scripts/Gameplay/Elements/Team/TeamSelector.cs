using Com.RandomDudes.Events;
using UnityEngine;

namespace Com.IsartDigital.Otaring.Gameplay.Elements.Team
{
    public class TeamSelector : MonoBehaviour
    {
        public static ActionEvent<TeamSelector, Player> OnPlayerTrigger = new ActionEvent<TeamSelector, Player>();

        public TeamColor Team { get => teamColor; }

        [SerializeField] private TeamColor teamColor = default;
        [SerializeField] private string playerTag = default;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag))
                OnPlayerTrigger?.Invoke(this, other.GetComponent<Player>());
        }
    }
}