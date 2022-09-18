using Com.RandomDudes.UI;
using UnityEngine;

namespace Com.RandomDudes.StateMachineBehaviors
{
    public class AnimationEndedScreen : StateMachineBehaviour
    {
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.GetComponent<UIScreen>().EndAnimation();
        }
    }
}