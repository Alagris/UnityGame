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
            if(user.TryGetComponent(out SexSceneController controller))
            {
                controller.EnterScene(SexAnimationName, inst);
                if (user.TryGetComponent(out HudUIController c))
                {
                    c.CloseInventory();
                }
            }
        }
    }
}