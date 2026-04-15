using Inter;
using Inv;
using System;
using UnityEditor;
using UnityEngine;

namespace Items
{
    [Serializable]
    public class ItemObject : MonoBehaviour, IInteractable, IInteractableMessage
    {
        
        [SerializeField]
        private ItemInstance Item;

        public void SetItem(ItemInstance Item)
        {
            this.Item = Item;
        }

        public Item getType() => Item==null?null:Item.Type;

        [MenuItem("GameObject/3D/Item", false, 10)]
        static void CreateItemObject(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Item");
            go.AddComponent<ItemObject>();
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

        public void Interact(GameObject interactor, ItemInstance tool, ref RaycastHit hit)
        {
            if(Item!=null && Item.IsValid() && interactor.TryGetComponent(out Inventory inv))
            {
                inv.AddItem(Item);
                Destroy(gameObject);
            }
        }

        public string InteractMessage(GameObject interactor, ItemInstance tool, ref RaycastHit hit)
        {
            return Item!=null && Item.IsValid() ? "Pick up " + Item.ItemName : "";
        }
    }
}
