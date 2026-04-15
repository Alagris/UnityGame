using Inter;
using Inv;
using Unity.VisualScripting;
using UnityEngine;
namespace Items
{
    [CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item/Generic")]
    public class Item : Loot
    {
        [SerializeField]
        public string ItemName;

        [SerializeField]
        public GameObject Mesh;

        [SerializeField]
        public ItemObject ItemObject;

        [SerializeField]
        public float Weight;

        [SerializeField]
        public ClothingSlot Slot;

        [DoNotSerialize]
        public uint SlotRaw { get => (uint)Slot; }

        public virtual IInteractable OnInteract(AnyCharacterController user, ItemInstance inst) { return user.Interact(inst); }

        public virtual void OnAttack(AnyCharacterController user, ItemInstance inst) { }

        public virtual void OnUse(AnyCharacterController user, ItemInstance inst) { }

        public virtual bool TryEquipAsClothes(ClothingInventory user, ItemInstance inst) { return false; }

        public virtual bool TryEquipInHand(ClothingInventory user, ItemInstance inst) { return false; }

        public override void DrawLoot(Inventory destination, int count)
        {
            destination.AddItem(this, count);
        }

        public virtual void Uneqip(ClothingInventory user, ItemInstance itemInstance)
        {

        }
        public static bool Collides(uint slot1, uint slot2)
        {
            return (slot1 & slot2) != 0;
        }
        public static bool Collides(ClothingSlot slot, uint slots)
        {
            return Collides((uint)slot, slots);
        }
        public bool Collides(uint slots)
        {
            return Collides(Slot, slots);
        }
        public bool Collides(ClothingSlot slots)
        {
            return Collides((uint)slots);
        }
    }
}