using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class Loot : ScriptableObject
{

    public virtual void DrawLoot(Inventory destination, int count) { }
}