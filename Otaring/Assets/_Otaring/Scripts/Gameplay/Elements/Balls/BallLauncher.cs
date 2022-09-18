using UnityEngine;

namespace Com.IsartDigital.Otaring.Gameplay.Elements.Balls
{
    public class BallLauncher : MonoBehaviour
    {
        [SerializeField] private GameObject ballPrefab = default;
        [SerializeField] private GameObject arenaCorner1 = default;
        [SerializeField] private GameObject arenaCorner2 = default;
        [SerializeField] private GameObject groundCirclePrefab = null;
        [SerializeField] private Transform objectsContainer = null;

        public Ball CreateNewBall(Vector3 velocity)
        {
            Ball ball = Instantiate(ballPrefab, transform.position, Quaternion.identity, objectsContainer).GetComponent<Ball>();

            ball.groundCircle = Instantiate(groundCirclePrefab, transform.position, Quaternion.identity, objectsContainer);

            ball.Init(arenaCorner1, arenaCorner2);
            ball.Launch(velocity);

            return ball;
        }
    }
}