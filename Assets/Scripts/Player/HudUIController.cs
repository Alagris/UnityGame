using Inv;
using Inv.UI;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [Serializable]
    public class HudUIController : MonoBehaviour
    {
        [DoNotSerialize]
        public HudState State { get; private set; } = HudState.GAME;
        [DoNotSerialize]
        public bool IsPaused { get; private set; } = false;

        [SerializeField]
        public TextMeshProUGUI InteractionMessageText;
        [SerializeField]
        PlayerInput PlayerInput;
        [SerializeField]
        Canvas HUD;
        [SerializeField]
        InventoryUIController inventoryPrefab;
        [SerializeField]
        Inventory inventory;
        [SerializeField]
        string InventoryActionMap = "UI";
        [SerializeField]
        string GameActionMap = "Player";

        private InventoryUIController inventoryUiInstance = null;

        private void Start()
        {
            if (inventory == null) {
                inventory = GetComponent<Inventory>();
            }
            if(PlayerInput == null)
            {
                PlayerInput = GetComponent<PlayerInput>();
            }
            EnterGameState();
        }

        public void PauseGame(bool pause)
        {
            IsPaused = pause;
            Time.timeScale = pause ? 0 : 1;
        }

        
        private void EnterGameState()
        {
            State = HudState.GAME;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            PauseGame(false);
            PlayerInput.SwitchCurrentActionMap(GameActionMap);
            if (HUD != null)
            {
                HUD.enabled = true;
            }
        }

        
        public void OpenInventory()
        {
            if (inventoryPrefab == null)
            {
                Debug.LogError("Tried to open inventory but no inventory prefab is specified");
                return;
            }
            if (inventory == null)
            {
                Debug.LogError("Tried to open inventory but no inventory component exists");
                return;
            }
            if (inventoryUiInstance == null)
            {
                if (HUD != null)
                {
                    HUD.enabled = false;
                }
                
                inventoryUiInstance = Instantiate(inventoryPrefab);
                inventoryUiInstance.SetInventory(inventory);
                State = HudState.INVENTORY;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                PauseGame(true);
                PlayerInput.SwitchCurrentActionMap(InventoryActionMap);
            }
        }

        public void CloseInventory()
        {
            if (inventoryUiInstance != null)
            {
                Destroy(inventoryUiInstance.gameObject);
                inventoryUiInstance = null;
                EnterGameState();
            }
        }
        public bool IsInventoryOpen() => inventoryUiInstance != null;
        public void TriggerInventory()
        {
            if (IsInventoryOpen())
            {
                CloseInventory();
            }
            else
            {
                OpenInventory();
            }
        }

        public void OnTriggerInventory(InputAction.CallbackContext c)
        {
            if (c.performed)
            {
                TriggerInventory();
            }
        }
        public void OnTriggerEscape(InputAction.CallbackContext c)
        {
            if (c.performed)
            {
                TriggerEscape();
            }
        }

        public void OnDropItem(InputAction.CallbackContext c)
        {
            if (c.performed && IsInventoryOpen())
            {
                inventoryUiInstance.DropSelectedItem();
            }
        }
        public void OnUseItem(InputAction.CallbackContext c)
        {
            if (c.performed && IsInventoryOpen())
            {
                inventoryUiInstance.UseSelectedItem();
            }
        }
        public void TriggerEscape()
        {
            switch (State)
            {
                case HudState.GAME:
                    break;
                case HudState.INVENTORY:
                    CloseInventory();
                    break;
            }
        }

        public void SetInteractionMessage(string msg)
        {
            if (InteractionMessageText != null)
            {
                InteractionMessageText.enabled = msg != null;
                if (msg != null)
                {
                    InteractionMessageText.text = msg;
                }
            }
        }
    }
}
