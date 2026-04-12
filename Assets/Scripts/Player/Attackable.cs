using Items;
using UnityEngine;
namespace Inter
{
    public interface IAttackable
    {
        public void Attack(GameObject attacker, ItemInstance weapon, ref RaycastHit hit);
    }

}