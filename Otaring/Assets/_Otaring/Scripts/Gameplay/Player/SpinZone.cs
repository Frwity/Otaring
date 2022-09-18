using UnityEngine;
using Com.IsartDigital.Otaring.Gameplay.Elements.Balls;

namespace Com.IsartDigital.Otaring.Gameplay
{
    public class SpinZone : MonoBehaviour
    {
        [SerializeField] MeshRenderer radialProgress = default;
        [SerializeField] GameObject ballAnchor = default;
        [SerializeField] float upLaunchVelocity = 7f;
        [SerializeField] float forwardLaunchVelocity = 10f;
        [SerializeField] float maxSpinTime = 3.0f;
        [SerializeField] AudioSource launchBall;
        [SerializeField] ParticleSystem particle;

        public TeamColor team = TeamColor.NONE;
        public bool isSpinning = false;

        private Ball ball = null;

        private float spinningTime = 0.0f;
        private float ballPossesionCounter = 0.0f;

        void Update()
        {
            if (isSpinning)
            {
                
                spinningTime += Time.deltaTime;
                if (spinningTime >= maxSpinTime)
                {
                    radialProgress.material.SetFloat("_BlinkAlpha", (Mathf.Sin(Time.time * 30.0f) + 1.0f) / 2.0f);
                    spinningTime = maxSpinTime;
                }
                radialProgress.material.SetFloat("_Frac", spinningTime / maxSpinTime);
                particle.startColor = new Color(1.0f * spinningTime / maxSpinTime, 1.0f + ((-1.0f) * (spinningTime / maxSpinTime)), 0.0f);
                particle.startSpeed = 5f + (10f - 5f) * spinningTime / maxSpinTime;
            }

            if (ball != null)
            {
                ball.transform.position = Vector3.Lerp(ball.transform.position, ballAnchor.transform.position, Mathf.Lerp(0.05f, 1f, ballPossesionCounter / 1.5f));
                ballPossesionCounter += Time.deltaTime;
            }
        }

        public void StartSpinning()
        {
            isSpinning = true;
            particle.Play();
        }

        public void Launch()
        {
            isSpinning = false;

            if (ball != null)
            {
                Vector3 launchVelocity; 
                if (spinningTime < maxSpinTime / 2f)
                {
                    launchVelocity = ballAnchor.transform.right * forwardLaunchVelocity / 2f;
                    launchVelocity.y = upLaunchVelocity * 2f;
                    ball.Launch(launchVelocity, 4.4f);
                }
                else
                {
                    launchVelocity = ballAnchor.transform.right * forwardLaunchVelocity;
                    launchVelocity.y = spinningTime * upLaunchVelocity;
                    ball.Launch(launchVelocity);
                }
                ball.ChangeTeam(team);
                launchBall.Play();
                ball = null;
            }
            particle.Stop();
            spinningTime = 0.0f;
            ballPossesionCounter = 0.0f;
            radialProgress.material.SetFloat("_Frac", 0.0f);
            radialProgress.material.SetFloat("_BlinkAlpha", 1);
        }

        public float GetSpinFactor()
        {
            return spinningTime / maxSpinTime;
        }

        public Vector2 GetSpinVector()
        {
            return new Vector2(ballAnchor.transform.forward.x, ballAnchor.transform.forward.z).normalized;
        }

        public bool HasBall()
        {
            return ball;
        }

        private void OnTriggerStay(Collider other)
        {
            if (isSpinning && ball == null && other.tag == "Ball")
            {
                ball = other.GetComponent<Ball>();
                ball.IsSpinning = true;
            }
        }
    }
}
