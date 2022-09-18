using UnityEngine;
using UnityEngine.Events;

namespace Com.RandomDudes.StateMachineBehaviors
{
    public class StateExitTrigger : MonoBehaviour
    {
        [SerializeField] private string stateName = default;
        [SerializeField] private UnityEvent onStateExit = new UnityEvent();

        public void OnStateExit(int shortNameHash)
        {
            if (stateName == "" || shortNameHash == Animator.StringToHash(stateName))
                onStateExit.Invoke();
        }
    }
}