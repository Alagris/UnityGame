using Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.MessageBox;

namespace Inv.UI
{
    public class InventoryUIController : MonoBehaviour, InventoryListener
    {
        
        private Inventory inv;
        private ListView itemList;
        private Dictionary<ItemInstance, VisualElement> itemToVisElem = new Dictionary<ItemInstance, VisualElement>();
        private void RefreshList()
        {
            itemList.itemsSource = inv.GetItemList();
        }
        public void SetInventory(Inventory inv)
        {
            this.inv = inv;
            if (itemList == null)
            {
                UIDocument doc = GetComponent<UIDocument>();
                VisualElement root = doc.rootVisualElement;
                itemList = root.Q<ListView>("item-list");
                itemList.bindItem = onBindItem;
                itemList.unbindItem = onUnbindItem;
                
            }
            RefreshList();
            inv.listener = this;
        }

       

        void onBindItem(VisualElement visElem, int idx)
        {
            ItemInstance i = (ItemInstance)itemList.itemsSource[idx];
            itemToVisElem[i] = visElem;
            visElem.dataSource = i;
            visElem.RegisterCallback<MouseOverEvent>(OnMouseHoverOverItem);
            visElem.RegisterCallback<ClickEvent>(OnMouseClickOverItem);
            UpdateItem(i, visElem);
        }
        void onUnbindItem(VisualElement visElem, int idx)
        {
            itemToVisElem.Remove((ItemInstance)visElem.dataSource);
            visElem.UnregisterCallback<MouseOverEvent>(OnMouseHoverOverItem);
            visElem.UnregisterCallback<ClickEvent>(OnMouseClickOverItem);
            visElem.dataSource = null;
        }
        public void DropSelectedItem()
        {
            DropItem(itemList.selectedItem);
        }
        public void UseSelectedItem()
        {
            UseItem(itemList.selectedItem);
        }
        private void UseItem(object o)
        {
            if (o != null)
            {
                ItemInstance i = (ItemInstance)o;
                if (inv.TryGetComponent(out AnyCharacterController c))
                {
                    i.OnUse(c);
                }
            }
        }
        private void DropItem(object o)
        {
            if (o != null)
            {
                ItemInstance i = (ItemInstance)o;
                if (inv.TryGetComponent(out AnyCharacterController c))
                {
                    c.DropItem(i.Type);
                }
            }
        }
        private void OnMouseClickOverItem(ClickEvent evt)
        {
            VisualElement e = (VisualElement)evt.currentTarget;
            UseItem(e.dataSource);
           
        }
        
        private void OnMouseHoverOverItem(MouseOverEvent evt)
        {
            VisualElement e = (VisualElement)evt.currentTarget;
            object i = e.dataSource;
            int idx = itemList.itemsSource.IndexOf(i);
            itemList.SetSelection(idx);
        }


        public void OnItemAdded(Inventory inv, ItemInstance item) {
            RefreshList();
        }
        public void OnItemChanged(Inventory inv, ItemInstance item) {
            UpdateItem(item);
        }
        public void OnItemRemoved(Inventory inv, ItemInstance item) {
            RefreshList();
        }
        public void OnItemPutOn(Inventory inv, ItemInstance item) {
            UpdateItem(item);
        }
        public void OnItemTakenOff(Inventory inv, ItemInstance item) {
            UpdateItem(item);
        }
        public void OnItemEquippedInHand(Inventory inv, ItemInstance item) {
            UpdateItem(item);
        }
        public void OnItemUnequippedFromHand(Inventory inv, ItemInstance item) {
            UpdateItem(item);
        }
        public void OnInventoryStripped(Inventory inv) {
            UpdateAllItems();
        }
        public void OnInventoryCleared(Inventory inv) {
            RefreshList();
        }
        private void UpdateAllItems()
        {
            itemList.RefreshItems();
        }
        private void UpdateItem(ItemInstance i)
        { 
            if(itemToVisElem.TryGetValue(i, out VisualElement visElem))
            {
                UpdateItem(i, visElem);
            }
        }
        private void UpdateItem(ItemInstance i, VisualElement visElem)
        {
            Label itemNameLabel = visElem.Q<Label>("item-name");
            itemNameLabel.text = i.ItemName;
            if (i.IsEquipped())
            {
                itemNameLabel.AddToClassList("worn-item");
            }
            else
            {
                itemNameLabel.RemoveFromClassList("worn-item");
            }
        }
    }
}
