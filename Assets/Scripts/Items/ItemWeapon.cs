using UnityEngine;

[CreateAssetMenu(fileName = "ItemWeapon", menuName = "Scriptable Objects/Item/Weapon")]
public class ItemWeapon : Item
{
    [SerializeField]
    public int CharacterID;

    [SerializeField]
    public string ParentBone;

    public override void OnAttack(AnyCharacterController user, ItemInstance inst)
    {
        user.InteractAttack(inst);
    }
    public override bool TryEquipInHand(ClothingInventory user, ItemInstance inst)
    {
        Debug.Assert(inst.CurrentMeshInstance == null);
        return user.ForceEquipWeapon(inst, ParentBone);
    }
    public override void Uneqip(ClothingInventory user, ItemInstance itemInstance)
    {
        itemInstance.Owner.ForceUnequipWeapon(itemInstance);
    }
}
