using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class ItemInstance 
{
    [SerializeField]
    public Item Type;

    [SerializeField]
    public int Count;

    [DoNotSerialize]
    public Inventory Owner;

    [DoNotSerialize]
    public GameObject CurrentMeshInstance;

    public ItemInstance() { }

    public ItemInstance(Item item, int count=1)
    {
        this.Type = item;
        this.Count = count;
    }

    [DoNotSerialize]
    public ClothingSlot Slot { get => Type.Slot; }

    [DoNotSerialize]
    public float Weight { get => Type.Weight; }

    [DoNotSerialize]
    public string ItemName { get => Type.ItemName; }

    [DoNotSerialize]
    public GameObject Mesh { get => Type.Mesh; }

    [DoNotSerialize]
    public float TotalWeight { get => Type.Weight*Count; }
    [DoNotSerialize]
    public byte SlotRaw { get => Type.SlotRaw; }

    public IInteractable OnInteract(AnyCharacterController user) => Type.OnInteract(user, this);

    public void OnAttack(AnyCharacterController user) => Type.OnAttack(user, this);

    public void OnUse(AnyCharacterController user) => Type.OnUse(user, this);

    public bool TryEquipAsClothes(ClothingInventory user) {
        Debug.Assert(user == Owner);
        return Owner != null && CurrentMeshInstance == null && Slot != ClothingSlot.NONE && Type.TryEquipAsClothes(user, this);
    }

    public bool TryEquipInHand(ClothingInventory user) {
        Debug.Assert(user == Owner);
        return Owner!=null && CurrentMeshInstance == null && Type.TryEquipInHand(user, this); 
    }

    public void Stack(ItemInstance item)
    {
        Debug.Assert(item.Type == Type);
        Count += item.Count;
    }

    public void Uneqip(ClothingInventory user)
    {
        if (CurrentMeshInstance != null)
        {
            Type.Uneqip(user, this);
        }
    }
    public bool EquipWeapon(ClothingInventory user,ItemInstance weapon)
    {
        Debug.Assert(Owner == user);
        return weapon.CurrentMeshInstance == null && Type.TryEquipInHand(user, this);
    }

    public bool Collides(byte slot)
    {
        return Type.Collides(slot);
    }
    public bool Collides(ClothingSlot slot)
    {
        return Type.Collides(slot);
    }

    public bool Collides(ItemInstance clothes) => Collides(clothes.Slot);

    internal void Destroy()
    {
        if (CurrentMeshInstance != null)
        {
            UnityEngine.Object.Destroy(CurrentMeshInstance);
            CurrentMeshInstance = null;
        }
        Owner = null;
    }

    public bool IsValid() => Type != null;
}
