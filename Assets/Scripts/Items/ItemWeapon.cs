using Inv;
using UnityEngine;
namespace Items
{
    [CreateAssetMenu(fileName = "ItemWeapon", menuName = "Scriptable Objects/Item/Weapon")]
    public class ItemWeapon : Item
    {
        [SerializeField]
        public int CharacterID;

        [SerializeField]
        public string ParentBone;

        public override void OnUse(AnyCharacterController user, ItemInstance inst)
        {
            if (inst.Owner is ClothingInventory)
            {
                ClothingInventory c = (ClothingInventory)inst.Owner;
                if (inst.IsEquipped())
                {
                    Uneqip(c, inst);
                }
                else
                {
                    TryEquipInHand(c, inst);
                }
            }
        }
        public override void OnAttack(AnyCharacterController user, ItemInstance inst)
        {
            user.combat.ForceAttack();
            user.InteractAttack(inst);
        }
        public override bool TryEquipInHand(ClothingInventory user, ItemInstance inst)
        {
            Debug.Assert(inst.CurrentMeshInstance == null);
            return user.CharacterID == CharacterID &&  user.ForceEquipWeapon(inst, ParentBone);
        }
        public override void Uneqip(ClothingInventory user, ItemInstance itemInstance)
        {
            itemInstance.Owner.ForceUnequipWeapon(itemInstance);
        }
    }
}