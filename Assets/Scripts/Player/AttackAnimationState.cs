using UnityEngine;

public class AttackAnimationState : StateMachineBehaviour
{
    [SerializeField]
    bool beginSheathing = false;
    [SerializeField]
    string debugString;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CharacterPrefabController p = animator.GetComponent<CharacterPrefabController>();
        if (p != null && p.Owner != null)
        {
            p.Owner.OnAttackStart(debugString);
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CharacterPrefabController p = animator.GetComponent<CharacterPrefabController>();
        if(p != null && p.Owner!=null)
        {
            p.Owner.OnAttackEnd(beginSheathing, debugString);
        }
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
}
