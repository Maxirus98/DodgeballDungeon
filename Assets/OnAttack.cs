using System.Collections;
using UnityEngine;

public class OnAttack : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       var upper = animator.GetLayerIndex("Upper Body");
       animator.SetLayerWeight(upper, 0);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // TODO: Animation blending issue here
        var upperIndex = animator.GetLayerIndex("Upper Body");
        var fadeDuration = 0.5f;
        animator.GetComponent<PlayerController>().StartCoroutine(FadeInUpperLayer(animator, upperIndex, fadeDuration));
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}

    IEnumerator FadeInUpperLayer(Animator animator, int layerIndex, float duration)
    {
        float time = 0f;
        // Assume a starting weight of 0f.
        float startingWeight = 0f;
        float targetWeight = 1f;

        while (time < duration)
        {
            // Get the value of the weight between 0 and 1 based on the accumulated time.
            float layerWeight = Mathf.Lerp(startingWeight, targetWeight, time / duration);
            // Set the weight of the layer based on its index.
            animator.SetLayerWeight(layerIndex, layerWeight);
            time += Time.deltaTime;
            // this pauses the coroutine until the next frame.
            yield return null;
        }
        // Finish the coroutine and make sure to set the exact target weight.
        animator.SetLayerWeight(layerIndex, targetWeight);
    }
}
