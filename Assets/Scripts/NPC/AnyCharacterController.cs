using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.GridLayoutGroup;


public abstract class AnyCharacterController : MonoBehaviour
{

    [SerializeField]
    public CharacterType CharacterType;
    
    protected CharacterPrefabController CharacterInstance;

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
            inventory.SetBody(CharacterType.CharacterID, newCharacterInstance.Body);
            walkSpeed = CharacterType.SlowWalkSpeed;
            return newCharacterInstance;
        }
        return null;
    }

    
    public Inventory inventory;
    
    [SerializeField]
    public float gravity = 9.81f;
    [SerializeField]
    public float jumpForce = 10f;
    [SerializeField]
    public float walkSpeed = 4;
  
    protected Vector3 velocity;
    protected Vector2 movementDirection;



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
    
    public void Attack()
    {
        if (canAttack) // if in stage 3
        {
            CharacterInstance.TriggerAttack();
            bufferAttackRequests = false;
            canAttack = false;
            canMove = false;
            wantsToAttack = false;
            DebugLog("Attack started");
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


    public float locomotionSpeed
    {
        get
        {
            return walkSpeed;
        }
    }

    protected abstract Vector3 transformMovementDirection(Vector2 localMovemenetDirection);
    

    // Update is called once per frame
    protected virtual void Update()
    {
        if (CharacterInstance!=null)
        {
            Vector3 movementVector;
            Vector3 movementVector2d;
            float speed = 0;
            if (canMove)
            {
                Vector2 localMovemenetDirection = movementDirection * locomotionSpeed;
                movementVector = transformMovementDirection(localMovemenetDirection);
                movementVector2d = movementVector;
                movementVector2d.y = 0;

                if (CharacterInstance.characterController.isGrounded)
                {
                    velocity.y = Mathf.Max(velocity.y, 0);
                }
                else
                {
                    velocity.y -= gravity * Time.deltaTime;
                }
                movementVector += velocity;
                speed = localMovemenetDirection.magnitude;

            }
            else
            {
                movementVector = velocity;
                movementVector2d = Vector3.zero;
            }
            CharacterInstance.characterController.Move(movementVector * Time.deltaTime);
            if (speed != 0)
            {
                CharacterInstance.transform.rotation = Quaternion.LookRotation(movementVector2d);
            }
            CharacterInstance.SetAnimationWalkSpeed(speed);
        }
    }

}
