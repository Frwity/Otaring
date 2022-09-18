using UnityEngine;

namespace Com.RandomDudes.StateMachineBehaviors
{
    public class StateChangeDetector : StateMachineBehaviour
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            StateEnterTrigger[] stateEnterDetectors = animator.gameObject.GetComponents<StateEnterTrigger>();

            for (int i = stateEnterDetectors.Length - 1; i >= 0; i--)
                stateEnterDetectors[i].OnStateEnter(stateInfo.shortNameHash);
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            StateExitTrigger[] stateExitDetectors = animator.gameObject.GetComponents<StateExitTrigger>();

            for (int i = stateExitDetectors.Length - 1; i >= 0; i--)
                stateExitDetectors[i].OnStateExit(stateInfo.shortNameHash);
        }
    }
}