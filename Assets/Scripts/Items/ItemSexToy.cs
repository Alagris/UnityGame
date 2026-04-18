using Inv;
using Player;
using UnityEngine;
namespace Items
{
    [CreateAssetMenu(fileName = "ItemWeapon", menuName = "Scriptable Objects/Item/SexToy")]
    public class ItemSexToy : Item
    {
        [SerializeField]
        public string SexAnimationName;

        public override void OnUse(AnyCharacterController user, ItemInstance inst)
        {
            user.getAnimator().CrossFade(SexAnimationName, 0.2f);
            ItemObject i = inst.SpawnItemObject();
            i.animator.transform.parent = user.GetCharacterTransform();
            if (i.animator != null)
            {
                i.animator.CrossFade(SexAnimationName, 0.2f);
            }
            if (user.TryGetComponent(out HudUIController c))
            {
                c.CloseInventory();
            }

        }
    }
}