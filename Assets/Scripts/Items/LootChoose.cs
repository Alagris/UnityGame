using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Loot/Choose")]
public class LootChoose : Loot
{

    [SerializeField]
    int Count;
    [SerializeField]
    List<Loot> Items;

    public override void DrawLoot(Inventory destination, int count)
    {
        for (int i = 0; i < Count; i++)
        {
            int idx = UnityEngine.Random.Range(0, Items.Count);
            Items[idx].DrawLoot(destination, count);
        }
    }

}

