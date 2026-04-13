using Items;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.MessageBox;

namespace Inv.UI
{
    public class InventoryUIController : MonoBehaviour
    {
        List<ItemInstance> itemInstanceList;
        Inventory inv;
        public void RefreshItemList(Inventory inv)
        {
            this.inv = inv;
            UIDocument doc = GetComponent<UIDocument>();
            VisualElement root = doc.rootVisualElement;
            ListView itemList = root.Q<ListView>("item-list");
            itemInstanceList = inv.GetItemList();
            itemList.itemsSource = itemInstanceList;
            itemList.bindItem = onBindItem;
            itemList.unbindItem = onUnbindItem;
        }

        void onBindItem(VisualElement visElem, int idx)
        {
            ItemInstance i = itemInstanceList[idx];
            Label itemNameLabel = visElem.Q<Label>("item-name");
            itemNameLabel.text = i.ItemName;
            //.style.unityFontStyleAndWeight. = FontStyle.Bold;
            //itemNameLabel.SetBinding("style.-unity-font-style");
        }
        void onUnbindItem(VisualElement visElem, int idx)
        {
            
        }
    }
}
