using UnityEngine;

public class AnimEndDisableObject : StateMachineBehaviour
{
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Disable the object when the animation ends
        animator.gameObject.SetActive(false);
    }
}
