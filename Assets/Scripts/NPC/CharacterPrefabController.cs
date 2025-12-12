using System;
using UnityEngine;

public class CharacterPrefabController : MonoBehaviour
{
    internal AnyCharacterController Owner;
    [SerializeField]
    public SkinnedMeshRenderer Body;
    [SerializeField]
    public CharacterController characterController;
    [SerializeField]
    public Animator animator;
    [SerializeField]
    public Transform CameraTarget;
    [SerializeField]
    string velocityAnimationParam = "velocity";
    [SerializeField]
    string attackAnimationTrigger = "attack";
    [SerializeField]
    string leftFootFrontAnimationParam = "leftFootFront";


    protected int velocityParamId;
    protected int attackTriggerId;
    protected int leftFootParamId;
    public void OnAttackCanListen() // enters stage 2
    {
        Owner.OnAttackCanListen();
    }
    public void OnAttackReady() // enters stage 3
    {
        Owner.OnAttackReady();
    }

    public void OnLeftFootInFront() // enters stage 3
    {
        SetLeftFootFront(true);
    }
    public void OnRightFootInFront() // enters stage 3
    {
        SetLeftFootFront(false);
    }

    void Start()
    {
        velocityParamId = Animator.StringToHash(velocityAnimationParam);
        attackTriggerId = Animator.StringToHash(attackAnimationTrigger);
        leftFootParamId = Animator.StringToHash(leftFootFrontAnimationParam);
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        if (Body == null)
        {
            Body = transform.Find("Body").GetComponent<SkinnedMeshRenderer>();
        }
        if (CameraTarget == null)
        {
            CameraTarget = transform.Find("CameraTarget");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    internal void SetLeftFootFront(bool isLeftFront)
    {
        animator.SetBool(leftFootParamId, isLeftFront);
    }
    internal void TriggerAttack()
    {
        animator.SetTrigger(attackTriggerId);
    }

    internal void SetAnimationWalkSpeed(float speed)
    {
        animator.SetFloat(velocityParamId, speed);
    }
}
