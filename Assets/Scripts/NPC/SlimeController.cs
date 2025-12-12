using UnityEngine;

public class SlimeController : MonoBehaviour
{
    CharacterController characterController;
    Vector3 velocity;
    Vector3 lookDirection;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        velocity = hit.normal;
        Debug.Log(hit);
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();  
    }

    public void Jump(Vector3 jumpDirection, Vector3 lookDirection)
    {
        velocity = jumpDirection;
        this.lookDirection = lookDirection;
    }

    void Update()
    {
        
    }
}
