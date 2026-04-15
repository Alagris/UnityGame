using Items;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.tvOS;
namespace Inv
{
    public interface InventoryListener
    {
        void OnItemAdded(Inventory inv, ItemInstance item) { }
        void OnItemChanged(Inventory inv, ItemInstance item) { }
        void OnItemRemoved(Inventory inv, ItemInstance item) { }
        void OnItemPutOn(Inventory inv, ItemInstance item) { }
        void OnItemTakenOff(Inventory inv, ItemInstance item) { }
        void OnItemEquippedInHand(Inventory inv, ItemInstance item) { }
        void OnItemUnequippedFromHand(Inventory inv, ItemInstance item) { }
        void OnInventoryStripped(Inventory inv) { }
        void OnInventoryCleared(Inventory inv) { }
    }

    public class Inventory : InteractableCharacter
    {

        public InventoryListener listener;

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
                    if(listener != null)
                    {
                        listener.OnItemChanged(this, prev);
                    }
                }
                else
                {
                    item.Owner = this;
                    Items.Add(item.Type, item);
                    if (listener != null)
                    {
                        listener.OnItemAdded(this, item);
                    }
                }

            }
        }
        public ItemInstance RemoveItem(ItemInstance item, bool drop=false)
        {
            return item==null?null:RemoveItem(item.Type, item.Count);
        }
        public ItemInstance RemoveItem(Item type, int count = 1, bool drop=false)
        {
            if (count>0 && Items.TryGetValue(type, out ItemInstance prev))
            {
                if (prev.Count > count)
                {
                    prev.Count -= count;
                    CarriedWeight -= count * prev.Weight;
                    if (listener != null)
                    {
                        listener.OnItemChanged(this, prev);
                    }
                    if (drop)
                    {
                        ItemInstance dropped = new ItemInstance();
                        dropped.Type = prev.Type;
                        dropped.Count = count;
                        return dropped;
                    }
                }
                else
                {
                    Unequip(prev);
                    prev.Owner = null;
                    Items.Remove(type);
                    CarriedWeight -= prev.TotalWeight;
                    if (listener != null)
                    {
                        listener.OnItemRemoved(this, prev);
                    }
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
            if (listener != null)
            {
                listener.OnInventoryCleared(this);
            }
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

        public List<ItemInstance> GetItemList()
        {
            return new List<ItemInstance>(Items.Values); 
        }

        public void DropItem()
        {
            if (EquippedInHand != null)
            {
                DropItem(EquippedInHand);
            }
        }
        public ItemObject DropItem(Item Type, int count=1)
        {
            return SpawnItemObject(RemoveItem(Type, count, true));
        }
        public ItemObject DropItem(ItemInstance i)
        {
            return SpawnItemObject(RemoveItem(i, true));
            
        }
        public ItemObject SpawnItemObject(ItemInstance i)
        {
            if (i != null)
            {
                ItemObject spawned = Instantiate(i.Type.ItemObject);
                spawned.SetItem(i);
                
                return spawned;
            }
            return null;
        }
    }
}