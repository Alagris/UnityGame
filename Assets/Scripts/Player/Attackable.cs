using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public interface IAttackable
{
    public void Attack(GameObject attacker, ItemInstance weapon, RaycastHit hit);
}

