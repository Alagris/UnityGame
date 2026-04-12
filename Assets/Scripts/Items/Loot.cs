using Inv;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Items
{
    public class Loot : ScriptableObject
    {

        public virtual void DrawLoot(Inventory destination, int count) { }
    }
}