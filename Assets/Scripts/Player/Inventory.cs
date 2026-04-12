using Items;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace Inv
{
    public class Inventory : InteractableCharacter
    {

        [SerializeField]
        public Loot Loot;

        [SerializeField]
        public ItemInstance EquippedInHand = null;

        [SerializeField]
        protected Dictionary<Item, ItemInstance> Items = new Dictionary<Item, ItemInstance>();


        [DoNotSerialize]
        protected float CarriedWeight = 0;


        public void AddItem(Item item, int count = 1)
        {
            AddItem(new ItemInstance(item, count));
        }
        public void AddItem(ItemInstance item)
        {
            if (item.Owner == null)
            {
                CarriedWeight += item.Weight;
                if (Items.TryGetValue(item.Type, out ItemInstance prev))
                {
                    prev.Stack(item);
                }
                else
                {
                    item.Owner = this;
                    Items.Add(item.Type, item);
                }

            }
        }
        public ItemInstance RemoveItem(ItemInstance item)
        {
            return RemoveItem(item.Type, item.Count);
        }
        public ItemInstance RemoveItem(Item type, int count = 1)
        {
            if (Items.TryGetValue(type, out ItemInstance prev))
            {
                if (prev.Count > count)
                {
                    prev.Count -= count;
                    CarriedWeight -= count * prev.Weight;
                }
                else
                {
                    Unequip(prev);
                    prev.Owner = null;
                    Items.Remove(type);
                    CarriedWeight -= prev.TotalWeight;
                }
                return prev;
            }
            else
            {
                return null;
            }

        }
        public virtual void Unequip(ItemInstance item)
        {
            item.Uneqip(null);
        }
        public virtual void ForceUnequipWeapon()
        {
            EquippedInHand = null;
        }

        public void ForceUnequipWeapon(ItemInstance itemInstance)
        {
            if (EquippedInHand == itemInstance)
            {
                ForceUnequipWeapon();
            }
        }
        public virtual void SetBody(CharacterType characterType, CharacterPrefabController newCharacterInstance)
        {
            CharacterID = characterType.CharacterID;
            CharacterName = characterType.Name;
        }

        public virtual void RemoveAllItems()
        {
            foreach (var i in Items)
            {
                i.Value.Owner = null;
            }
            CarriedWeight = 0;
            Items.Clear();
            ForceUnequipWeapon();
        }
        public void DrawLoot()
        {
            DrawLoot(Loot);
        }
        public void DrawLoot(Loot loot, int count = 1)
        {
            if (Loot != null)
            {
                loot.DrawLoot(this, count);
            }
        }
        public void ResetInventory()
        {
            RemoveAllItems();
            DrawLoot();
        }

        public bool hasEquippedInHand() => EquippedInHand != null && EquippedInHand.IsValid();
    }
}