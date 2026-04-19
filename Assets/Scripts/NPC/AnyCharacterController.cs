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
    public Vector3 GetCharacterPosition() => CharacterInstance.transform.position;
    protected Vector2 lookRotation;
    public Vector3 cameraTargetLocalPosition;
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
            lastMovementDirection2d= newCharacterInstance.transform.forward;
            lastMovementDirection2d.y = 0;
            lastMovementDirection2d.Normalize();
            return newCharacterInstance;
        }
        return null;
    }

    [SerializeField]
    public Inventory inventory;
    [SerializeField]
    public Combat combat;
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
    public bool enableHeadIK=true;
    private float smoothedLocomotionSpeed;
    private float desiredLocomotionSpeed;
    private float locomotionSpeedChangeVelocity;
    private float smoothedYaw = 0, currentYawVelocity = 0;
    private Vector3 lastMovementDirection2d;
    Vector3 movementVector;
    protected Vector3 velocity;
    protected Vector2 localMovemenetDirection;
    
    public bool isGrounded { get; private set;  }

    public const int DISABLE_MOVMEMENT_BY_COMBAT = 1;
    public const int DISABLE_MOVMEMENT_BY_SEX = 2;
    private int disableMovementSemaphore = 0;

    public void DisableMovement(int mask)
    {
        disableMovementSemaphore = disableMovementSemaphore | mask;
    }
    public void EnableMovement(int mask)
    {
        disableMovementSemaphore = disableMovementSemaphore & ~mask;
    }
    public void SetMovementEnabled(int mask, bool enable)
    {
        if (enable) EnableMovement(mask);
        else DisableMovement(mask);
    }
    public bool canMove {  get => disableMovementSemaphore == 0; }

    protected bool wantsToJump = false;

    

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

        if (combat == null)
        {
            TryGetComponent(out combat);
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
    static float ModularClamp(float angle, float min, float max)
    {
        if (angle > 180f) angle = angle - 360;
        else if (angle < -180f) angle = angle + 360;
        return Mathf.Clamp(angle, -90, 90);
    }
    static float ModularClampOrZero(float angle, float min, float max, float thresholdMin, float thresholdMax)
    {
        if (angle > 180f) angle = angle - 360;
        else if (angle < -180f) angle = angle + 360;
        if (angle < thresholdMin || angle > thresholdMax) return 0;
        return Mathf.Clamp(angle, -90, 90);
    }
    
    
    private void LateUpdate()
    {
        if (enableHeadIK)
        {
            Vector3 lookRot = getCameraTarget().rotation.eulerAngles;
            Vector3 frontRot = CharacterInstance.transform.rotation.eulerAngles;
            Vector3 difference = lookRot - frontRot;
            float desiredYaw = ModularClampOrZero(difference.y, -90, 90, -120, 120);

            float pitch = ModularClamp(difference.x, -70, 70);
            smoothedYaw = Mathf.SmoothDamp(smoothedYaw, desiredYaw, ref currentYawVelocity, 0.2f);
            //Quaternion differencePerBone = Quaternion.Slerp(Quaternion.identity, difference, 0.333f);
            //CharacterInstance.LowerNeckBone.transform.localRotation *= differencePerBone;
            Quaternion rot = Quaternion.Euler(0, smoothedYaw, 0);
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

    public Transform GetBone(string boneName)
    {
        foreach (Transform t in CharacterInstance.Body.bones)
        {
            if (t.name == boneName)
            {
                return t;
            }
        }
        return null;
    }
}
