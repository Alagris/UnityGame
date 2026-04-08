using System;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item/Generic")]
public class Item : Loot
{
    [SerializeField]
    public string ItemName;

    [SerializeField]
    public GameObject Mesh;

    [SerializeField]
    public float Weight;

    [SerializeField]
    public ClothingSlot Slot;

    [DoNotSerialize]
    public byte SlotRaw { get => (byte) Slot; }

    public virtual IInteractable OnInteract(AnyCharacterController user, ItemInstance inst) { return null; }

    public virtual void OnAttack(AnyCharacterController user, ItemInstance inst) { }

    public virtual void OnUse(AnyCharacterController user, ItemInstance inst) { }

    public virtual bool TryEquipAsClothes(ClothingInventory user, ItemInstance inst) { return false;  }

    public virtual bool TryEquipInHand(ClothingInventory user, ItemInstance inst) { return false; }

    public override void DrawLoot(Inventory destination, int count) {
        destination.AddItem(this, count);
    }

    public virtual void Uneqip(ClothingInventory user, ItemInstance itemInstance)
    {
        
    }
    public static bool Collides(byte slot1, byte slot2)
    {
        return (slot1 & slot2) != 0;
    }
    public static bool Collides(ClothingSlot slot, byte slots)
    {
        return Collides((byte) slot, slots);
    }
    public bool Collides(byte slots)
    {
        return Collides(Slot, slots);
    }
    public bool Collides(ClothingSlot slots)
    {
        return Collides((byte)slots);
    }
}
