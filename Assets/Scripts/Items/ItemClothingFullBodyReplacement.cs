using Inv;
using UnityEngine;
namespace Items
{
    [CreateAssetMenu(fileName = "ItemClothingFullBodyReplacement", menuName = "Scriptable Objects/Item/Full Body Replacement Clothing")]
    public class ItemClothingFullBodyReplacement : Item
    {
        [SerializeField]
        public int CharacterID;
        [SerializeField]
        public Mesh BodyReplacement;
        [SerializeField]
        public Material[] MaterialOverrides;

        public override void OnUse(AnyCharacterController user, ItemInstance inst) {
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
            return user.CharacterID == CharacterID && user.ForceEquipFullBodyReplacementClothes(inst, BodyReplacement, MaterialOverrides);
        }

        public override void Uneqip(ClothingInventory user, ItemInstance itemInstance)
        {
            user.ForceUnequipFullBodyReplacementClothes(itemInstance);
        }
    }
}