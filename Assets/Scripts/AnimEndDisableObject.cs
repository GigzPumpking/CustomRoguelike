using UnityEngine;

public class AnimEndDisableObject : StateMachineBehaviour
{
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // if the object has the Explosion component, call the ExplosionPool's ReturnObject method
        Explosion explosion = animator.GetComponent<Explosion>();

        if (explosion != null)
        {
            ExplosionPool.Instance.ReturnObject(explosion.GetPrefab(), explosion);
        } else {
            // Disable the object when the animation ends
            animator.gameObject.SetActive(false);
        }
    }
}
