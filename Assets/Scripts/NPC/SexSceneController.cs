

using Items;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;



namespace Inv
{
    public class SexSceneController : MonoBehaviour
    {
        [SerializeField]
        PlayerInput PlayerInput;
        [SerializeField]
        AnyCharacterController controller;
        ItemObject[] props;
        [SerializeField]
        CinemachineOrbitalFollow sexCam;
        [SerializeField]
        string AnimationStateAfterSex;
        [SerializeField]
        string SexKeyMapName="";
        [SerializeField]
        string DefaultKeyMapName="Player";
        [SerializeField]
        float cameraMovementSpeed=1;
        [SerializeField]
        string SnapInitialCameraTargetToBone;
        int AnimationStateAfterSexId;
        public void EnterScene(string sexAnimationName, ItemInstance prop)
        { 
            EnterScene(sexAnimationName, prop.SpawnItemObject());
        }
        
        public void EnterScene(string sexAnimationName, ItemObject prop)
        {
            props = new ItemObject[] { prop };
            EnterScene(sexAnimationName);
            
            
        }
        const float fadeTime = 0.2f;
        public void EnterScene(string sexAnimationName)
        {
            if (sexCam.VirtualCamera.Follow != controller.getCameraTarget())
            {
                if (PlayerInput != null && SexKeyMapName.Length > 0)
                {
                    PlayerInput.SwitchCurrentActionMap(SexKeyMapName);
                }
                sexCam.VirtualCamera.Priority = 100;
                sexCam.VirtualCamera.Follow = controller.getCameraTarget();
                defaultEnableHeadIK = controller.enableHeadIK;
                defaultCameraTargetLocalPosition = controller.cameraTargetLocalPosition;
                Transform bone = controller.GetBone(SnapInitialCameraTargetToBone);
                if (bone != null)
                {
                    
                    controller.cameraTargetLocalPosition = bone.position - controller.GetCharacterPosition();
                }
                controller.enableHeadIK = false;
                controller.DisableMovement(AnyCharacterController.DISABLE_MOVMEMENT_BY_SEX);
                enabled = true;
            }
            controller.getAnimator().CrossFade(sexAnimationName, fadeTime);
            if (props != null)
            {
                for (int i = 0; i < props.Length; i++)
                {
                    props[i].animator.transform.parent = controller.GetCharacterTransform();
                    if (props[i].animator != null)
                    {
                        props[i].animator.CrossFade(sexAnimationName, fadeTime);
                    }
                }
            }
        }
        public void ExitScene(bool destroyProps=true)
        {
            if (sexCam.VirtualCamera.Follow != null)
            {
                sexCam.VirtualCamera.Follow = null;
                sexCam.VirtualCamera.Priority = defaultPriority;
                if (PlayerInput != null && DefaultKeyMapName.Length > 0)
                {
                    PlayerInput.SwitchCurrentActionMap(DefaultKeyMapName);
                }
                controller.enableHeadIK = defaultEnableHeadIK;
                controller.EnableMovement(AnyCharacterController.DISABLE_MOVMEMENT_BY_SEX);
                controller.cameraTargetLocalPosition = defaultCameraTargetLocalPosition;
                enabled = false;
            }
            controller.getAnimator().CrossFade(AnimationStateAfterSexId, fadeTime);
            if (destroyProps && props!=null)
            {
                for (int i = 0; i < props.Length; i++)
                {
                    props[i].Destroy();
                }
            }
            props = null;
        }
        
        private int defaultPriority;
        private bool defaultEnableHeadIK;
        private Vector3 defaultCameraTargetLocalPosition;
        private void Start()
        {
            if (controller == null)
            {
                controller = GetComponent<AnyCharacterController>();
            }
            if (PlayerInput == null)
            {
                PlayerInput = GetComponent<PlayerInput>();
            }
            defaultPriority = sexCam.VirtualCamera.Priority;
            AnimationStateAfterSexId = Animator.StringToHash(AnimationStateAfterSex);
            enabled = false;
        }

        public void OnExit(InputAction.CallbackContext c)
        {
            if (c.performed)
            {
                ExitScene();
            }
        }
        Vector2 inputMovement;
        private void Update()
        {
            Vector2 m = inputMovement * Time.deltaTime * sexCam.Radius;
            Vector3 sideways = sexCam.transform.right * m.x;
            controller.cameraTargetLocalPosition.y += m.y;
            controller.cameraTargetLocalPosition += sideways;

        }

        public void OnMoveCenter(InputAction.CallbackContext c)
        {
            inputMovement = c.ReadValue<Vector2>();
        }

    }
}
