using UnityEngine;

namespace Com.IsartDigital.Otaring.Otaring.Gameplay.Elements.Balls
{

    public class ImpactParticlesOrientation : MonoBehaviour
    {
        public void SetBallVelocity(Vector3 velocity)
        {
            Vector3 direction = Vector3.ProjectOnPlane(velocity, Vector3.up);
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}