using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Com.IsartDigital.Otaring.Otaring.Gameplay.Player
{
    public class PlayerNumber : MonoBehaviour
    {
        [SerializeField] private Image round = default;
        [SerializeField] private TextMeshProUGUI number = default;

        private Camera belongingCamera;

        public void SetCamera(Camera camera)
        {
            this.belongingCamera = camera;
        }

        public void SetNumber(int number)
        {
            this.number.text = number.ToString("0");
        }

        public void SetTeam(TeamColor team)
        {
            round.color = team == TeamColor.BLUE ? Color.blue : Color.red;
            transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Team" + team.ToString());
        }

        private void Update()
        {
            if (belongingCamera != null)
            {
                if (belongingCamera.gameObject.activeInHierarchy)
                    transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.position - belongingCamera.transform.position, Vector3.up).normalized);
                else
                    transform.rotation = Quaternion.LookRotation(-Vector3.right);
            }
        }
    }
}