using Inter;
using Inv.UI;
using TMPro;

using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : AnyCharacterController
    {


        [SerializeField]
        Camera cam;
        [SerializeField]
        CinemachineCamera cinecam;
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
        float TimeToTriggerInteract = 1;

        private CinemachineOrbitalFollow cinecamOrbitalFollow;
        private float DesiredZoomValue;
        private float ZoomVelocity;
        private RaycastHit CurrentRaycastHit;
        private bool HasRaycastHit = false;
        public override CharacterPrefabController InstantiateCharacterType()
        {
            CharacterPrefabController i = base.InstantiateCharacterType();
            if (i != null)
            {
                cinecam.transform.parent = i.transform;
                cinecam.Follow = i.CameraTarget;
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
            cinecamOrbitalFollow = cinecam.GetComponent<CinemachineOrbitalFollow>();
            DesiredZoomValue = cinecamOrbitalFollow.Radius;
            
            EyeTransform = cam.transform;
        }
        Ray getEyeRay(float distance = 0)
        {
            Transform eyeTransform = cam.transform;
            Vector3 eyePos = eyeTransform.position;
            float eyeDist = cinecam.Follow == null ? 0 : Vector3.Distance(cinecam.Follow.position, eyePos);
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
        public void HoldStart()
        {
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
                Debug.Log("Interact ");
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
                HoldEnd();
                if (c.duration < TimeToTriggerInteract)
                {
                    InteractWithTool();
                }
            }
        }
        public void OnDropItem(InputAction.CallbackContext c)
        {
            if (c.performed)
            {
                inventory.DropItem();
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
        public void OnMove(InputAction.CallbackContext c)
        {
            movementDirection = c.ReadValue<Vector2>();
        }
        public void OnZoom(InputAction.CallbackContext c)
        {
            float zoom = c.ReadValue<float>();
            DesiredZoomValue = Mathf.Clamp(DesiredZoomValue + zoom * ZoomSensitivity, MinZoom, MaxZoom);

        }
        private void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 100, 20), (wantsToAttack ? "W" : "") + (canAttack ? "C" : "") + (bufferAttackRequests ? "B" : ""));
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
            cinecamOrbitalFollow.Radius = Mathf.SmoothDamp(cinecamOrbitalFollow.Radius, DesiredZoomValue, ref ZoomVelocity, 0.2f);
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