using Inter;
using Inv.UI;
using TMPro;

using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Player
{
    public class PlayerController : AnyCharacterController
    {


        [SerializeField]
        Camera cam;
        [SerializeField]
        CinemachineThirdPersonFollow cinecam;
        [SerializeField]
        HudUIController hudController;
        [SerializeField]
        float ZoomSensitivity = -0.5f;
        [SerializeField]
        float MinZoom = 1;
        [SerializeField]
        float MaxZoom = 3;
        [SerializeField]
        float MaxPhysicsHandleSpeed = 10;
        [SerializeField]
        float TimeToTriggerInteract = 0.2f;
        [SerializeField]
        float LookSensitivity = 0.1f;

        bool IsFirstPersonCam() => DesiredZoomValue < 0;

        
        private float DesiredZoomValue;
        private float ZoomVelocity;
        private RaycastHit CurrentRaycastHit;
        private bool HasRaycastHit = false;
        
        public override CharacterPrefabController InstantiateCharacterType()
        {
            CharacterPrefabController i = base.InstantiateCharacterType();
            if (i != null)
            {
                cinecam.VirtualCamera.Follow = i.CameraTarget;
            }
            return i;
        }

        protected override void Start()
        {
            base.Start();

            if (hudController == null)
            {
                hudController = GetComponent<HudUIController>();
            }
            
            DesiredZoomValue = cinecam.CameraDistance;
            EyeTransform = cam.transform;
        }
        Ray getEyeRay(float distance = 0)
        {
            Transform eyeTransform = cam.transform;
            Vector3 eyePos = eyeTransform.position;
            float eyeDist = IsFirstPersonCam() ? 0 : Vector3.Distance(getCameraTarget().position, eyePos);
            Vector3 fwd = eyeTransform.forward;
            return new Ray(eyePos + fwd * (eyeDist + distance), fwd);
        }
        Vector3 getPhysicsHandleDesiredLocation() => getEyeRay(HeldObjectDistance).origin;
        public void DoNewLineTrace()
        {
            HasRaycastHit = Physics.Raycast(getEyeRay(), out CurrentRaycastHit, ReachDistance);
        }
        public override bool LineTrace(out RaycastHit hit)
        {
            hit = CurrentRaycastHit;
            return HasRaycastHit;
        }
        private Rigidbody CurrentlyHeldObject;
        private float HeldObjectDistance = 0;
        double holdStartTime;
        public void HoldStart()
        {
            holdStartTime = Time.time;
            CurrentlyHeldObject = null;
            if (HasRaycastHit)
            {
                CurrentlyHeldObject = CurrentRaycastHit.rigidbody;
                HeldObjectDistance = CurrentRaycastHit.distance;
            }
        }

        public void HoldEnd()
        {
            CurrentlyHeldObject = null;
        }

        public void OnInteract(InputAction.CallbackContext c)
        {
            if (c.performed)
            {
                InteractWithoutTool();
            }
        }
        
        public void OnInteractWithTool(InputAction.CallbackContext c)
        {
            if (c.performed)
            {
                HoldStart();

            }
            else if (c.canceled)
            {
                double duration = Time.time - holdStartTime;
                HoldEnd();
                if (duration < TimeToTriggerInteract)
                {
                    InteractWithTool();
                }
            }
        }
        public void OnDropItem(InputAction.CallbackContext c)
        {
            if (c.performed)
            {
                DropItem();
            }
        }
        
        public void OnAttack(InputAction.CallbackContext c)
        {
            if (c.performed)
            {
                Attack();
            }
        }
        public void OnJump(InputAction.CallbackContext c)
        {
            if (c.performed)
            {
                Jump();
            }
            else if (c.canceled)
            {
                StopJump();
            }
        }
        public void OnRun(InputAction.CallbackContext c)
        {
            if (c.performed)
            {
                StartRun();
            }
            else if(c.canceled)
            {
                StopRun();
            }
        }
        public void OnLook(InputAction.CallbackContext c)
        {
            AddLookRotation(c.ReadValue<Vector2>() * LookSensitivity);

        }
        public void OnMove(InputAction.CallbackContext c)
        {
            movementDirection = c.ReadValue<Vector2>();
        }
        public void SwitchToFirstPerson()
        {
            DesiredZoomValue = -1;
            cinecam.CameraDistance = 0;
            cinecam.CameraSide = 0.5f;
            ZoomVelocity = 0;

        }
        public void SwitchToThirdPerson()
        {
            cinecam.CameraDistance = DesiredZoomValue = MinZoom;
            cinecam.CameraSide = 1;
            ZoomVelocity = 0;
        }
        public void OnZoom(InputAction.CallbackContext c)
        {
            float zoom = c.ReadValue<float>() * ZoomSensitivity;
            if (IsFirstPersonCam())
            {
                if (zoom > 0)
                {
                    SwitchToThirdPerson();
                }
            }
            else
            {
                if (DesiredZoomValue == MinZoom && zoom < 0)
                {
                    SwitchToFirstPerson();
                }
                else
                {
                    DesiredZoomValue = Mathf.Clamp(DesiredZoomValue + zoom, MinZoom, MaxZoom);
                }
                
            }
            

        }
        private void OnGUI()
        {
            //GUI.Label(new Rect(10, 10, 100, 20), (wantsToAttack ? "W" : "") + (canAttack ? "C" : "") + (bufferAttackRequests ? "B" : ""));
        }

        protected override Vector3 transformMovementDirection(Vector2 localMovemenetDirection)
        {
            float angle = -Mathf.Deg2Rad * cam.transform.eulerAngles.y;
            float x = localMovemenetDirection.x;
            float y = localMovemenetDirection.y;
            float c = Mathf.Cos(angle);
            float s = Mathf.Sin(angle);
            return new Vector3(c * x - s * y, 0, s * x + c * y);
            //return cam.transform.TransformDirection(new Vector3(localMovemenetDirection.x, 0, localMovemenetDirection.y));
        }
        protected override void Update()
        {
            base.Update();
            
            if (IsFirstPersonCam()) {

            } else { 
                cinecam.CameraDistance = Mathf.SmoothDamp(cinecam.CameraDistance, DesiredZoomValue, ref ZoomVelocity, 0.1f);

            }
            if (CurrentlyHeldObject == null)
            {
                DoNewLineTrace();
                SetInteractMessage();
            }

        }
        private void FixedUpdate()
        {
            if (CurrentlyHeldObject != null)
            {
                Vector3 position = CurrentlyHeldObject.transform.position;
                Vector3 destination = getPhysicsHandleDesiredLocation();
                Vector3 direction = destination - position;
                float dist = Vector3.Magnitude(direction);
                float speed = Mathf.SmoothStep(0, MaxPhysicsHandleSpeed, dist);
                CurrentlyHeldObject.linearVelocity = direction.normalized * speed;
            }
        }
        private void SetInteractMessage()
        {
            if (hudController != null && hudController.InteractionMessageText!=null)
            {
                string msg = null;
                if (HasRaycastHit && CurrentRaycastHit.collider.gameObject.TryGetComponent(out IInteractableMessage interactable))
                {
                    msg = interactable.InteractMessage(gameObject, inventory.EquippedInHand, ref CurrentRaycastHit);

                }
                hudController.SetInteractionMessage(msg);
                
            }
        }
    }
}