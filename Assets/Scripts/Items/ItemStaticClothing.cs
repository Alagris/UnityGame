using Inv;
using UnityEngine;
namespace Items
{
    [CreateAssetMenu(fileName = "ItemClothing", menuName = "Scriptable Objects/Item/Static Clothing")]
    public class ItemStaticClothing : Item
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
                    TryEquipAsClothes(c, inst);
                }
            }
        }

        public override bool TryEquipAsClothes(ClothingInventory user, ItemInstance inst)
        {
            Debug.Assert(inst.CurrentMeshInstance == null);
            return user.CharacterID == CharacterID && user.ForceEquipStaticClothes(inst, ParentBone);
        }

        public override void Uneqip(ClothingInventory user, ItemInstance itemInstance)
        {
            user.ForceUnequipClothes(itemInstance);
        }
    }
}