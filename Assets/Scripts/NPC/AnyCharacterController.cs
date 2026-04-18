using Inter;
using Inv;
using Items;
using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.GridLayoutGroup;


public abstract class AnyCharacterController : MonoBehaviour
{

    [SerializeField]
    public CharacterType CharacterType;
    
    protected CharacterPrefabController CharacterInstance;
    public CharacterPrefabController GetCharacterInstance()=>CharacterInstance;
    public bool HasCharacter()=>CharacterInstance != null;
    public Transform GetCharacterTransform() => CharacterInstance.transform;
    public float3 GetCharacterPosition() => CharacterInstance.transform.position;
    protected Vector2 lookRotation;
    protected Vector3 cameraTargetLocalPosition;
    public Transform getCameraTarget() => CharacterInstance.CameraTarget;
    public void AddLookRotation(Vector2 rotation)
    {
        lookRotation += rotation;
        lookRotation.y = Mathf.Clamp(lookRotation.y, -90, 90);
        lookRotation.x = lookRotation.x % 360;
        getCameraTarget().rotation = Quaternion.Euler(-lookRotation.y, lookRotation.x, 0);
    }
    public void SetCharacterType(CharacterType characterType)
    {
        CharacterType = characterType;
        UpdateCharacterType();
    }
    public void UpdateCharacterType() {
        CharacterPrefabController newCharacterInstance = InstantiateCharacterType();
        if (CharacterInstance != null)
        {
            Destroy(CharacterInstance);
        }
        CharacterInstance = newCharacterInstance;
    }
    
    public virtual CharacterPrefabController InstantiateCharacterType()
    {
        if (CharacterType != null)
        {

            GameObject i = Instantiate(CharacterType.Prefab, transform);
            CharacterPrefabController newCharacterInstance = i.GetComponent<CharacterPrefabController>();
            newCharacterInstance.Owner = this;
            inventory.SetBody(CharacterType, newCharacterInstance);
            desiredLocomotionSpeed = smoothedLocomotionSpeed = CharacterType.WalkSpeed;
            locomotionSpeedChangeVelocity = 0;
            cameraTargetLocalPosition = newCharacterInstance.CameraTarget.transform.localPosition;
            newCharacterInstance.CameraTarget.transform.parent = transform;
            return newCharacterInstance;
        }
        return null;
    }

    [SerializeField]
    public Inventory inventory;
    [SerializeField]
    public float rotationSpeed = 15f;
    [SerializeField]
    public float weight = 1f;
    [SerializeField]
    public float jumpForce = 10f;
    [SerializeField]
    public float ReachDistance = 7;
    [SerializeField]
    public Transform EyeTransform;
    [SerializeField]
    public float MaxGroundedFeetDistance=1;
    [SerializeField]
    public float accelerationSpeed=0.3f;
    [SerializeField]
    LayerMask GroundCollisionLayer;
    [SerializeField]
    bool enableHeadIK=true;
    private float smoothedLocomotionSpeed;
    private float desiredLocomotionSpeed;
    private float locomotionSpeedChangeVelocity;

    protected Vector3 velocity;
    protected Vector2 localMovemenetDirection;
    protected float smoothedYaw;
    public bool isGrounded { get; private set;  }

    /**Every attack animation consists of 3 stages:
     1. animation plays and player cannot attack again. Clicking attack button is ignored.
     2. animation plays and player cannot attack again but if at this stage player clicks the attack button, the game registers that player wants to attack again.
     3. animation plays but player can already chose to execute the next attack, which would interrupt the current attack animation. If game registered any attack clicks in previous stage, then the next attack is launched immediately.
     4. end of animation. If this end is reached but player hasn't clicked the attack button teh whole time, then the idle animation starts playing.

    This is what variables are set at each stage:
    1. bufferAttackRequests=false,  canAttack=false, canMove=false, wantsToAttack=false
    2. bufferAttackRequests=true,  canAttack=false, canMove=false, wantsToAttack=maybe
    3. bufferAttackRequests=false,  canAttack=true, canMove=false, stage ends if wantsToAttack
    (between 3 and 4.) next attack plays if player clicks attack button
    4. bufferAttackRequests=false,  canAttack=true, canMove=true, wantsToAttack=false

    It is possible that the stage 2 is never entered. In which case no attack buffering occurs.
    */
    protected bool canMove = true;
    protected bool canAttack = true;
    protected bool bufferAttackRequests = true;
    protected bool wantsToAttack = false;
    protected bool wantsToJump = false;

    static string b(bool b) => b ? "T" : "F";
    void DebugLog(string msg)
    {
        
        Debug.Log(
            
            "[canMove=" +b(canMove)+
            " canAttack="+b(canAttack)+
            " bufferAttackRequests="+b(bufferAttackRequests)+
            " wantsToAttack="+b(wantsToAttack)
            +"]" + msg
        );
    }


    public void StartRun()
    {
        desiredLocomotionSpeed = CharacterType.RunSpeed;
    }

    public void StopRun()
    {
        desiredLocomotionSpeed = CharacterType.WalkSpeed;
    }
    public void Jump()
    {
        wantsToJump = true;
        
    }

    public void StopJump()
    {
        wantsToJump = false;
    }

    public void Attack()
    {
        if (canAttack) // if in stage 3
        {
            if (inventory != null && inventory.hasEquippedInHand()) {
                CharacterInstance.TriggerAttack();
                inventory.EquippedInHand.OnAttack(this);
                bufferAttackRequests = false;
                canAttack = false;
                canMove = false;
                wantsToAttack = false;
                DebugLog("Attack started");
            }
        }
        else if (bufferAttackRequests) // if in stage 2
        {
            wantsToAttack = true;
            DebugLog("Wants to attack");
        }
    }
    public void OnAttackCanListen() // enters stage 2
    {
        
        bufferAttackRequests = true;
        DebugLog("Attack buffer listening");
    }
    public void OnAttackReady() // enters stage 3
    {
        if (wantsToAttack)
        { // if attack was buffered, execute it


            CharacterInstance.TriggerAttack();
            wantsToAttack = false;
            // we leave bufferAttackRequests==true because the OnAttackEnd gets triggered even when the animation is interrupted.
            // We need some way to decide whetehr the animation ended normally or was interrupted. bufferAttackRequests is gonna be such flag
            DebugLog("Ready to attack, executing next");
        }
        else
        {
            
            canAttack = true; // enters stage 3
            bufferAttackRequests = false; // exists stage 2
            DebugLog("Ready to attack");
        }


    }
    public void OnSheathingStart(string debugString)
    {
        DebugLog("Sheathing animation started (" + debugString+")");
    }
    public void OnAttackStart(string debugString) 
    {
        DebugLog("Attack animation started (" + debugString + ")");
    }
    public void OnAttackEnd(bool beginSheathing, string debugString) // enters stage 4
    {

        canAttack = true;
        wantsToAttack = false;
        if (bufferAttackRequests) // if true, that means the animation was interrupted
        {
            bufferAttackRequests = false;
            DebugLog("Attack ended early (" + debugString + ")");
        }
        else
        {
            canMove = !beginSheathing;
            if (beginSheathing)
            {
                DebugLog("Sheathing weapon (" + debugString + ")"); 
            }
            else
            {
                DebugLog("Attack ended (" + debugString + ")");
            }
            
        }

    }

    public void OnSheathingEnd(string debugString)
    {
        canMove = true;
        DebugLog("Sheathing ended ("+ debugString+")");
        
    }


    public virtual bool LineTrace(out RaycastHit hit)
    {
        if (EyeTransform != null)
        {
            Ray r = new Ray(EyeTransform.position, EyeTransform.forward);

            return Physics.Raycast(r, out hit, ReachDistance);

        }
        else
        {
            hit = default;
        }
        return false;
        
    }
    public IInteractable InteractWithoutTool()
    {
        return Interact(null);
        
    }
    public IInteractable InteractWithTool()
    {

        if (inventory != null && inventory.hasEquippedInHand())
        {
            return inventory.EquippedInHand.OnInteract(this); 
        }
        else
        {
            return Interact(null);
        }
        
    }
    public IInteractable Interact(ItemInstance tool)
    {
        if (LineTrace(out RaycastHit hit))
        {
            if (hit.collider.gameObject.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact(gameObject, tool, ref hit);
            }
        }
        return null;
    }

    public IAttackable InteractAttack(ItemInstance weapon)
    {
        if (LineTrace(out RaycastHit hit))
        {
            if (hit.collider.gameObject.TryGetComponent(out IAttackable attackable))
            {
                attackable.Attack(gameObject, weapon, ref hit);
            }
        }
        return null;
    }

    protected virtual void Start()
    {

        if (inventory == null)
        {
            inventory = GetComponent<Inventory>();
        }
        
        UpdateCharacterType();

    }

    public void OnLeftFootInFront(bool leftInFront) // enters stage 3
    {
        CharacterInstance.SetLeftFootFront(leftInFront);
    }


    

    protected Vector3 transformMovementDirection(Vector2 localMovemenetDirection)
    {
        float angle = -Mathf.Deg2Rad * lookRotation.x;
        float x = localMovemenetDirection.x;
        float y = localMovemenetDirection.y;
        float c = Mathf.Cos(angle);
        float s = Mathf.Sin(angle);
        return new Vector3(c * x - s * y, 0, s * x + c * y);
    }

    Ray getDownwardsRay()
    {
        Vector3 pos = GetCharacterPosition();
        float offset = CharacterInstance.characterController.center.y;
        pos.y += offset;
        return new Ray(pos, Vector3.down);
        
    }
    private Vector3 lastMovementDirection2d;
    Vector3 movementVector;
    // Update is called once per frame
    protected virtual void Update()
    {
        if (CharacterInstance!=null)
        {
            
            Vector3 movementVector2d;

            Ray downwardsRay = getDownwardsRay();
            float feetMaxDistance = CharacterInstance.characterController.center.y + MaxGroundedFeetDistance;
            bool isGrounded = Physics.SphereCast(downwardsRay, CharacterInstance.characterController.radius, feetMaxDistance, GroundCollisionLayer);
            
            if (canMove)
            {
                movementVector2d = transformMovementDirection(localMovemenetDirection.normalized);
                float mag = movementVector2d.magnitude;
                if (mag > 0)
                {
                    lastMovementDirection2d = movementVector2d;
                }
                smoothedLocomotionSpeed = Mathf.SmoothDamp(smoothedLocomotionSpeed, desiredLocomotionSpeed * mag, ref locomotionSpeedChangeVelocity, accelerationSpeed);

                if (isGrounded && wantsToJump && velocity.y <= 0)
                {
                    velocity.y += jumpForce;
                    CharacterInstance.TriggerJump();
                }
                
                if (CharacterInstance.characterController.isGrounded && velocity.y < 0)
                {
                    velocity.y = 0;
                }
                else
                {
                    velocity.y += weight * Physics.gravity.y * Time.deltaTime;
                }
                movementVector = lastMovementDirection2d * smoothedLocomotionSpeed + velocity;
                

            }
            else
            {
                smoothedLocomotionSpeed = 0;
                movementVector = velocity;
                lastMovementDirection2d = Vector3.zero;
                movementVector2d = Vector2.zero;
            }
            
            CharacterInstance.characterController.Move(movementVector * Time.deltaTime);
            if (smoothedLocomotionSpeed != 0)
            {
                CharacterInstance.transform.rotation = Quaternion.RotateTowards(CharacterInstance.transform.rotation, Quaternion.LookRotation(lastMovementDirection2d), rotationSpeed);
            }
            CharacterInstance.SetAnimationWalkSpeed(smoothedLocomotionSpeed);
            CharacterInstance.SetIsGrounded(isGrounded);
            
            getCameraTarget().transform.position = cameraTargetLocalPosition + CharacterInstance.transform.position;
            if (enableHeadIK)
            {
                //CharacterInstance.HeadBone.transform.localRotation = Quaternion.identity;
                //CharacterInstance.NeckBone.transform.localRotation = Quaternion.identity;
                //CharacterInstance.LowerNeckBone.transform.localRotation = Quaternion.identity;
            }
        }
    }

    private void LateUpdate()
    {
        if (enableHeadIK)
        {
            Vector3 lookRot = getCameraTarget().rotation.eulerAngles;
            Vector3 frontRot = CharacterInstance.transform.rotation.eulerAngles;
            Vector3 difference = lookRot - frontRot;
            float yaw = Mathf.Clamp(difference.y, -90, 90); 
            float pitch = Mathf.Clamp(difference.x, -70, 70);
            //Quaternion differencePerBone = Quaternion.Slerp(Quaternion.identity, difference, 0.333f);
            //CharacterInstance.LowerNeckBone.transform.localRotation *= differencePerBone;
            Quaternion rot = Quaternion.Euler(0, yaw , 0);
            CharacterInstance.HeadBone.transform.localRotation = CharacterInstance.HeadBone.transform.localRotation * rot;
            //CharacterInstance.NeckBone.transform.localRotation = CharacterInstance.NeckBone.transform.localRotation * rot;
            //CharacterInstance.LowerNeckBone.transform.localRotation = CharacterInstance.LowerNeckBone.transform.localRotation * rot;
            
            
        }
    }

    public ItemObject  DropItem(Item type, int count=1)
    {
        if (inventory != null && CharacterInstance!=null)
        {
            ItemObject i = inventory.DropItem(type, count);
            if(i != null)
            {
                i.transform.position = CharacterInstance.transform.position;
                return i;
            }
        }
        return null;
    }
    public ItemObject DropItem(ItemInstance i)=>i==null?null:DropItem(i.Type, i.Count);
    public ItemObject DropItem() => inventory==null?null: DropItem(inventory.EquippedInHand);

    public Animator getAnimator() => CharacterInstance.animator;
}
