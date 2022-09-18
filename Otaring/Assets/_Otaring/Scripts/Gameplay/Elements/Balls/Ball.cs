using Com.IsartDigital.Otaring.Otaring.Gameplay.Elements.Balls;
using UnityEngine;

namespace Com.IsartDigital.Otaring.Gameplay.Elements.Balls
{
    public class Ball : MonoBehaviour
    {
        public delegate void BallEventHandler(TeamColor currentTeam);
        static public BallEventHandler OnGroundHit;

        public bool IsSpinning { get { return isSpinning; } set { isSpinning = value; groundCircle.SetActive(!isSpinning); } }

        private GameObject arenaCorner1;
        private GameObject arenaCorner2;

        public GameObject groundCircle;
        [SerializeField] private SphereCollider sphereCollider = default;
        [SerializeField] private ParticleSystem ps = default;
        [SerializeField] private Gradient redColorOverLifetime = default;
        [SerializeField] private Gradient blueColorOverLifetime = default;
        [SerializeField] private Transform impactParticles = default;
        [SerializeField] private ImpactParticlesOrientation impactParticlesOrientation = default;

        [SerializeField] private AudioSource wallBounce = default;
        [SerializeField] private AudioSource groundBounce = default;
        public float gravity = 9.81f;

        private Vector3 velocity = default;
        private bool isMoving = true;
        private bool isSpinning = false;

        private TeamColor currentTeam = default;

        private void Awake()
        {
            if (!sphereCollider)
                sphereCollider = GetComponent<SphereCollider>();
        }

        public void Init(GameObject arenaCorner1, GameObject arenaCorner2)
        {
            this.arenaCorner1 = arenaCorner1;
            this.arenaCorner2 = arenaCorner2;
        }

        public void ChangeTeam(TeamColor team)
        {
            var col = ps.colorOverLifetime;

            col.color = team == TeamColor.RED ? redColorOverLifetime : blueColorOverLifetime;
        }

        public void Launch(Vector3 newVelocity, float newGravity = 9.81f)
        {
            velocity = newVelocity;

            gravity = newGravity;
            isMoving = true;
            IsSpinning = false;

            PlaceCircle();
        }

        private void PlaceCircle()
        {
            //newVelocity *= newSpeedMultiplicator;
            float x, z;
            float h = transform.position.y - arenaCorner1.transform.position.y - sphereCollider.radius;
            float delta = (velocity.y * velocity.y) - (2f * (-gravity) * h);
            float t = (-velocity.y - Mathf.Sqrt(delta)) / (-gravity);

            //Debug.Log("Angle : " + angle.ToString());

            if (t > 0f)
            {
                x = velocity.x * t + transform.position.x;
                z = velocity.z * t + transform.position.z;
            }
            else
            {
                t = (-velocity.y + Mathf.Sqrt(delta)) / (-gravity);
                x = velocity.x * t + transform.position.x;
                z = velocity.z * t + transform.position.z;
            }

            //Debug.Log("time : " + t.ToString());

            //Debug.Log(x.ToString() + " " + arenaCorner1.transform.position.y.ToString() + " " + z.ToString());
            groundCircle.transform.position = new Vector3(Mathf.Clamp(x, arenaCorner1.transform.position.x, arenaCorner2.transform.position.x), arenaCorner1.transform.position.y, Mathf.Clamp(z, arenaCorner1.transform.position.z, arenaCorner2.transform.position.z));
        }

        private void FixedUpdate()
        {
            if (isMoving && !IsSpinning)
            {
                velocity.y -= gravity * Time.fixedDeltaTime;
                transform.Translate(velocity * Time.fixedDeltaTime);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (IsSpinning)
                return;
            if (other.tag == "Ground")
            {
                //Debug.Log("Hit Time : " + (Time.time - countTimer).ToString());

                impactParticlesOrientation.SetBallVelocity(velocity);

                velocity = Vector3.zero;
                isMoving = false;

                TeamObject groundTeamObject = other.GetComponent<TeamObject>();

                if (groundTeamObject != null)
                    OnGroundHit?.Invoke(groundTeamObject.Team);
                
                StartCoroutine(other.GetComponent<Ground>().Sparkles(0.75f));

                for (int i = impactParticles.childCount - 1; i >= 0; i--)
                    impactParticles.GetChild(i).GetComponent<ParticleSystem>().Play();

                if (groundBounce)
                    groundBounce.Play();
            }
            if (other.tag == "Wall")
            {
                Vector3 pastPosition = transform.position - velocity * Time.fixedDeltaTime * 2;
                Vector3 normal = (pastPosition - other.ClosestPoint(pastPosition)).normalized;

                velocity = Vector3.Reflect(velocity, normal);

                PlaceCircle();

                if (wallBounce)
                    wallBounce.Play();
            }
        }
    }
}
