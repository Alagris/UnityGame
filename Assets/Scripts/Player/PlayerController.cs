using NUnit.Framework;
using System;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : AnyCharacterController
{
 
   
    [SerializeField]
    Camera cam;
    [SerializeField]
    CinemachineCamera cinecam;
    CinemachineOrbitalFollow cinecamOrbitalFollow;
    [SerializeField]
    float ZoomSensitivity=-0.5f;
    [SerializeField]
    float MinZoom=1;
    [SerializeField]
    float MaxZoom = 3;
    float DesiredZoomValue;
    float ZoomVelocity;
    public override CharacterPrefabController InstantiateCharacterType()
    {
        CharacterPrefabController i = base.InstantiateCharacterType();
        if (i!=null) {
            cinecam.transform.parent = i.transform;
            cinecam.Follow = i.CameraTarget;
        }
        return i;
    }

    protected override void Start() 
    {
        base.Start();
        cinecamOrbitalFollow = cinecam.GetComponent<CinemachineOrbitalFollow>();
        DesiredZoomValue = cinecamOrbitalFollow.Radius;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }
    
    public void OnAttack(InputAction.CallbackContext c)
    {
        if (c.performed)
        {
            Attack();
        }
    }
    public void OnMove(InputAction.CallbackContext c)
    {
        movementDirection = c.ReadValue<Vector2>();
    }
    public void OnZoom(InputAction.CallbackContext c)
    {
        float zoom = c.ReadValue<float>();
        DesiredZoomValue = Mathf.Clamp(DesiredZoomValue + zoom* ZoomSensitivity, MinZoom, MaxZoom);
        
    }
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), (wantsToAttack?"W":"") + (canAttack ? "C" : "") + (bufferAttackRequests ? "B" : ""));
    }

    protected override Vector3 transformMovementDirection(Vector2 localMovemenetDirection)
    {
        float angle = - Mathf.Deg2Rad * cam.transform.eulerAngles.y;
        float x = localMovemenetDirection.x;
        float y = localMovemenetDirection.y;
        float c = Mathf.Cos(angle);
        float s = Mathf.Sin(angle);
        return new Vector3(c*x-s*y, 0, s*x+c*y);
        //return cam.transform.TransformDirection(new Vector3(localMovemenetDirection.x, 0, localMovemenetDirection.y));
    }
    protected override void Update()
    {
        base.Update();
        cinecamOrbitalFollow.Radius = Mathf.SmoothDamp(cinecamOrbitalFollow.Radius, DesiredZoomValue, ref ZoomVelocity, 0.2f);
    }
}
