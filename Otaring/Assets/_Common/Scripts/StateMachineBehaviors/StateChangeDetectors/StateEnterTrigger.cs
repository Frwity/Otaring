using UnityEngine;
using UnityEngine.Events;

namespace Com.RandomDudes.StateMachineBehaviors
{
    public class StateEnterTrigger : MonoBehaviour
    {
        [SerializeField] private string stateName = default;
        [SerializeField] private UnityEvent onStateEnter = new UnityEvent();

        public void OnStateEnter(int shortNameHash)
        {
            if (stateName == "" || shortNameHash == Animator.StringToHash(stateName))
                onStateEnter.Invoke();
        }
    }
}