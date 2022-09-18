using UnityEngine;

namespace Com.IsartDigital.Otaring.Gameplay
{
    public class SpinningAnimator : MonoBehaviour
    {
        public float CurrentSpinningSpeed { get => currentSpinningSpeed; set { if (value == currentSpinningSpeed) return; currentSpinningSpeed = value; UpdateTargetAngle(); } }

        public Vector3 CurrentVelocity;

        [SerializeField] private AnimationCurve spinningAngleCurveMax = default;
        [SerializeField] private AnimationCurve spinningAngleCurveMin = default;

        [SerializeField] private AnimationCurve directionAngleCurve = default;

        [SerializeField] private float verticalOccilationSpeed = default;
        [SerializeField] private float movingDirectionOccilationSpeed = default;

        [SerializeField] private Transform spinningTop = default;
        [SerializeField] private Transform movementParent = default;

        private float currentSpinningSpeed;
        private float verticalOccilationTargetAngle;

        private Vector3 currentSpinningMainAxis = Vector3.up;

        private void Awake()
        {
            UpdateTargetAngle();
        }

        private void UpdateTargetAngle()
        {
            verticalOccilationTargetAngle = Random.Range(spinningAngleCurveMin.Evaluate(currentSpinningSpeed), spinningAngleCurveMax.Evaluate(currentSpinningSpeed));
        }

        private void Update()
        {
            Vector3 eulerAngles = spinningTop.localRotation.eulerAngles;
            float yRotation = eulerAngles.y + currentSpinningSpeed * Time.deltaTime;

            if (Mathf.Approximately(verticalOccilationTargetAngle, eulerAngles.z))
                UpdateTargetAngle();

            float zRotation = Mathf.MoveTowards(eulerAngles.z, verticalOccilationTargetAngle, verticalOccilationSpeed * Time.deltaTime);

            //Moving Direction Rotation
            Vector3 rotationAxis = -Vector3.Cross(CurrentVelocity.normalized, Vector3.up);
            Quaternion targetMovingDirectionRotation = Quaternion.AngleAxis(directionAngleCurve.Evaluate(CurrentVelocity.magnitude), rotationAxis);

            currentSpinningMainAxis = Vector3.Slerp(currentSpinningMainAxis, targetMovingDirectionRotation * Vector3.up, movingDirectionOccilationSpeed * Time.deltaTime);
            movementParent.up = currentSpinningMainAxis;

            spinningTop.localRotation = Quaternion.Euler(new Vector3(0, yRotation, zRotation));
        }
    }
}