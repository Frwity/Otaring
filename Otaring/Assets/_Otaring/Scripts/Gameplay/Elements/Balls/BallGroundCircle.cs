using UnityEngine;

namespace Com.IsartDigital.Otaring.Gameplay.Elements.Balls
{
    public class BallGroundCircle : MonoBehaviour
    {
        [SerializeField] private Transform graph = default;
        [SerializeField] private float minSize = default;
        [SerializeField] private float maxSize = default;
        [SerializeField] private float speed = default;

        private float counter = 0.0f;

        private void Update()
        {
            graph.localScale = Vector3.one * (minSize + Mathf.PingPong(counter * speed, maxSize - minSize));

            counter += Time.deltaTime;
        }
    }
}