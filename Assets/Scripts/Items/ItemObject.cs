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
        public Animator animator;

        [SerializeField]
        private ItemInstance Item;

        public void SetItem(ItemInstance Item)
        {
            this.Item = Item;
        }

        public Item getType() => Item==null?null:Item.Type;

        

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
