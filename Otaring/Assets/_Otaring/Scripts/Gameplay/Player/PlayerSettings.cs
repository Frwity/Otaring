using UnityEngine;

namespace Com.IsartDigital.Otaring.Gameplay
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Player/PlayerSettings")]
    public class PlayerSettings : ScriptableObject
    {
        public float MaxMoveSpeed { get => maxMoveSpeed; }
        public float Acceleration { get => acceleration; }
        public float Deceleration { get => deceleration; }
        public float AccelerationShoot { get => accelerationShoot; }
        public float DecelerationShoot { get => decelerationShoot; }
        public float MaxSpeedShoot{ get => maxSpeedShoot; }

        [SerializeField] private float maxMoveSpeed;
        [SerializeField] private float acceleration;
        [SerializeField] private float deceleration;
        [SerializeField] private float accelerationShoot;
        [SerializeField] private float decelerationShoot;
        [SerializeField] private float maxSpeedShoot;
    }
}