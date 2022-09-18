using Com.IsartDigital.Otaring.Gameplay.Elements;
using Com.IsartDigital.Otaring.Otaring.Gameplay.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Switch;

namespace Com.IsartDigital.Otaring.Gameplay
{
    public class Player : TeamObject
    {
        public float Speed { get; private set; } = 0;

        [SerializeField] private SpinZone spinningZone = default;
        [SerializeField] private PlayerNumber playerNumberPointer = default;
        [SerializeField] private PlayerSettings settings = default;
        [SerializeField] private CharacterController cc = default;
        [SerializeField] private SpinningAnimator spinningAnimator = default;
        [SerializeField] private GameObject shockParticle = default;
        [SerializeField] private AudioSource shockSound = default;
        [SerializeField] private List<ParticleSystem> boostParticles = new List<ParticleSystem>();

        [SerializeField] private Color[] redStartColor = new Color[0];
        [SerializeField] private Color[] blueStartColor = new Color[0];
        [SerializeField] private Gradient[] redColorOverLifeTime = new Gradient[0];
        [SerializeField] private Gradient[] blueColorOverLifeTime = new Gradient[0];

        [SerializeField] private float spinningAccelerationWhileCharging = default;
        [SerializeField] private float playerHitKnockackForce = 0.3f;

        private PlayerController controller;
        private Coroutine chargeCoroutine = default;
        private Coroutine dechargeCoroutine = default;

        private Vector3 velocity;

        private float spinningSpeed;
        private float acceleration;
        private float deceleration;
        private float maxMoveSpeed;

        private bool isCharging;

        private float chargeElapsedTime = 0f;
        [SerializeField] private float chargingDuration = 3f;
        [SerializeField] private float maxChargeDuration = 4f;

        [SerializeField] private float OnContactVibrationDuration = 0.15f;
        [SerializeField] private float OnContactVibrationIntensity= 0.4f;

        private void Start()
        {
            shockSound.Play();
            SetMoveMode();
            spinningSpeed = 250f;
        }

        public void SetPlayerNumber(int number)
        {
            playerNumberPointer.SetNumber(number);
        }

        public void SetPlayerController(PlayerController pc)
        {
            if (controller != null)
            {
                StopListeningControllerEvent();
            }
            controller = pc;
            ListenControllerEvent();
        }

        private void ListenControllerEvent()
        {
            controller.OnButtonPress += Controller_OnButtonPress;
            controller.OnButtonRealeased += Controller_OnButtonRealesed;
        }

        private void StopListeningControllerEvent()
        {
            controller.OnButtonPress -= Controller_OnButtonPress;
            controller.OnButtonRealeased -= Controller_OnButtonRealesed;
        }

        private void Controller_OnButtonPress()
        {
            if (!isCharging) SetChargeMode();
        }

        private void SetChargeMode()
        {
            isCharging = true;
            spinningZone.StartSpinning();

            if (dechargeCoroutine != null)
                StopCoroutine(dechargeCoroutine);

            if (chargeCoroutine != null)
                StopCoroutine(chargeCoroutine);

            chargeCoroutine = StartCoroutine(Charge());

            acceleration = settings.AccelerationShoot;
            deceleration = settings.DecelerationShoot;
            maxMoveSpeed = settings.MaxSpeedShoot;

            for (int i = boostParticles.Count - 1; i >= 0; i--)
                boostParticles[i].Play();
        }

        private void SetMoveMode()
        {
            if (isCharging)
            {
                isCharging = false;

                if (dechargeCoroutine != null)
                    StopCoroutine(dechargeCoroutine);

                dechargeCoroutine = StartCoroutine(Decharge());
            }

            spinningZone.Launch();

            acceleration = settings.Acceleration;
            deceleration = settings.Deceleration;
            maxMoveSpeed = settings.MaxMoveSpeed;

            for (int i = boostParticles.Count - 1; i >= 0; i--)
                boostParticles[i].Stop();
        }

        private void Controller_OnButtonRealesed()
        {
            SetMoveMode();
        }

        private IEnumerator Charge()
        {
            chargeElapsedTime = 0;
            while (isCharging && chargeElapsedTime < chargingDuration)
            {
                chargeElapsedTime += Time.deltaTime;
                spinningSpeed = Mathf.Clamp(spinningSpeed + spinningAccelerationWhileCharging * Time.deltaTime, 0, 720);
                yield return null;
            }

            while (isCharging && chargeElapsedTime < maxChargeDuration)
            {
                chargeElapsedTime += Time.deltaTime;
                spinningSpeed = Mathf.Clamp(spinningSpeed + spinningAccelerationWhileCharging * Time.deltaTime, 0, 720);
                yield return null;
            }
            SetMoveMode();
#if UNITY_SWITCH && !UNITY_EDITOR
            SwitchStopVibration();
#endif
        }

        private IEnumerator Decharge()
        {
            float counter = 0f;
            float currentSpeed = spinningSpeed;

            if (spinningZone.HasBall())
            {
                while ((counter += Time.deltaTime) <= 1f)
                {
                    spinningSpeed = Mathf.Lerp(currentSpeed, 250, counter / 0.5f);
                    yield return null;
                }
            }
            else
            {
                while ((counter += Time.deltaTime) <= 1.5f)
                {
                    spinningSpeed = Mathf.Lerp(currentSpeed, 250, counter / 1.5f);
                    yield return null;
                }
            }
        }

        public override void SetTeam(TeamColor team)
        {
            base.SetTeam(team);

            playerNumberPointer.SetTeam(team);
            spinningZone.team = team;

            Gradient[] colorOverLifeTime = team == TeamColor.BLUE ? blueColorOverLifeTime : redColorOverLifeTime;
            Color[] startColor = team == TeamColor.BLUE ? blueStartColor : redStartColor;

            for (int i = boostParticles.Count - 1; i >= 0; i--)
            {
                var col = boostParticles[i].colorOverLifetime;
                col.enabled = true;
                col.color = new ParticleSystem.MinMaxGradient(colorOverLifeTime[0], colorOverLifeTime[1]);
            }

            for (int i = boostParticles.Count - 1; i >= 0; i--)
            {
                var main = boostParticles[i].main;
                main.startColor = new ParticleSystem.MinMaxGradient(startColor[0], startColor[1]);
            }
        }

        private void Update()
        {
            Vector3 rawMoveInput = new Vector3(controller.moveInput.x, 0, controller.moveInput.y);
            Vector3 moveDirection = Quaternion.Euler(0, TeamCamera.GetTeamCamera(Team).transform.rotation.eulerAngles.y, 0) * rawMoveInput;

            float targetSpeed = maxMoveSpeed * moveDirection.magnitude;
            float velocityVariation = Mathf.Clamp(targetSpeed - Speed, -deceleration, acceleration);

            Speed += velocityVariation;

            velocity = Vector3.Lerp(velocity, moveDirection * Speed * 0.01f, Time.deltaTime / 0.3f);

            if(Time.deltaTime > 0)
                cc.Move(velocity * 100f * Time.deltaTime);
            
            spinningAnimator.CurrentVelocity = velocity;
            spinningAnimator.CurrentSpinningSpeed = spinningSpeed; //Todo Charge

            if (spinningZone.isSpinning)
            {
#if UNITY_SWITCH && !UNITY_EDITOR
                SwitchVibration();
#endif
            }

            transform.position = new Vector3(transform.position.x, 0.85f, transform.position.z);
            playerNumberPointer.SetCamera(TeamCamera.GetTeamCamera(Team).GetComponent<Camera>());
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject != gameObject && other.tag == "Player")
            {
                Instantiate(shockParticle, (transform.position - other.transform.position) / 2f + other.transform.position, Quaternion.identity);
                shockSound.pitch = 1.0f + Random.Range(0.0f, 0.25f);
                Invoke("PlayShockSound", Random.Range(0.0f, 0.08f));
                Vector3 knockImpulsion = (transform.position - other.transform.position).normalized;
                knockImpulsion.y = 0f;
                knockImpulsion *= playerHitKnockackForce;
                velocity = knockImpulsion;

                #if UNITY_SWITCH && !UNITY_EDITOR
                StartCoroutine(ContactVibration());
                #endif
            }
        }

        private void PlayShockSound()
        {
            shockSound.Play();
        }

#if UNITY_SWITCH && !UNITY_EDITOR

        private void OnDestroy()
        {
            SwitchStopVibration();
        }
#endif

#if UNITY_SWITCH

        private IEnumerator ContactVibration()
        {
            ((NPad)controller.gameObject.GetComponent<PlayerInput>().devices[0]).SetMotorSpeeds(OnContactVibrationIntensity, 0.0f);
            yield return new WaitForSecondsRealtime(OnContactVibrationDuration);
            ((NPad)controller.gameObject.GetComponent<PlayerInput>().devices[0]).SetMotorSpeeds(0.0f, 0.0f);
        }

        void SwitchVibration()
        {
           
            NPad pad = ((NPad)controller.gameObject.GetComponent<PlayerInput>().devices[0]);

            Vector2 spinVector = spinningZone.GetSpinVector();

            if (Team == TeamColor.BLUE)
                spinVector *= -1;

            if (!spinningZone.HasBall())
                spinVector *= 0.1f;

            float intensityOverCharge = Mathf.Max((chargeElapsedTime - chargingDuration) / maxChargeDuration, 0f); 

            float intensityLeft = -(spinningZone.GetSpinFactor() * Mathf.Min(spinVector.x, 0));
            float intensityRight = spinningZone.GetSpinFactor() * Mathf.Max(spinVector.x, 0);

            if (pad.styleMask == NPad.NpadStyles.JoyDual)
            {
                pad.SetMotorSpeedLeft(intensityOverCharge, 160, intensityLeft, 320);
                pad.SetMotorSpeedRight(intensityOverCharge, 160, intensityRight, 320);
            }
            else if (pad.styleMask == NPad.NpadStyles.JoyRight)
                pad.SetMotorSpeeds(intensityLeft, intensityOverCharge);
            else if (pad.styleMask == NPad.NpadStyles.JoyLeft)
                pad.SetMotorSpeeds(intensityRight, intensityOverCharge);
            else if (pad.styleMask == NPad.NpadStyles.FullKey)
                pad.SetMotorSpeeds(-Mathf.Min(spinVector.y, 0), -Mathf.Min(spinVector.y, 0));
        }

        void SwitchStopVibration()
        {
            ((NPad)controller.gameObject.GetComponent<PlayerInput>().devices[0]).SetMotorSpeeds(0.0f, 0.0f);

        }

#endif
    }
}
