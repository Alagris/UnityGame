using Inv;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Items
{
    [Serializable]
    public struct LootArity
    {
        [SerializeField]
        public Loot Item;

        [SerializeField]
        public int Count;
    }

    [CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Loot/All")]
    public class LootAll : Loot
    {
        [SerializeField]
        List<LootArity> Items;

        public override void DrawLoot(Inventory destination, int count)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].Item.DrawLoot(destination, count * Items[i].Count);
            }
        }

    }
}
