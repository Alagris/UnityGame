using Items;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEditor.Build.Reporting;
using UnityEngine;
namespace Inv
{
    public class ClothingInventory : Inventory
    {

        [SerializeField]
        List<ItemInstance> EquippedClothes;


        [SerializeField]
        SkinnedMeshRenderer Body;

        [DoNotSerialize]
        public byte EquippedSlots = 0;

        readonly Dictionary<string, Transform> originalBones = new Dictionary<string, Transform>();


        public override void SetBody(CharacterType characterType, CharacterPrefabController characterInstance)
        {
            Strip();
            base.SetBody(characterType, characterInstance);
            Body = characterInstance.Body;
            originalBones.Clear();
            foreach (var b in Body.bones)
            {
                originalBones.Add(b.name, b);
            }
            DrawLoot();
            AutoEquip();
        }
        public void StripClothes()
        {
            for (var i = 0; i < EquippedClothes.Count; i++)
            {
                EquippedClothes[i].Destroy();
            }
            EquippedClothes.Clear();
            EquippedSlots = 0;
        }
        public void Strip()
        {
            StripClothes();
            if (EquippedInHand != null)
            {
                EquippedInHand.Destroy();
                EquippedInHand = null;
            }

        }
        public void AutoEquip()
        {
            AutoEquipClothes();
            AutoEquipWeapons();
        }
        public void AutoEquipClothes()
        {
            foreach (var e in Items)
            {
                EquipClothes(e.Value);
            }
        }
        public void AutoEquipWeapons()
        {
            if (EquippedInHand == null)
            {
                foreach (var e in Items)
                {
                    if (EquipWeapon(e.Value))
                    {
                        return;
                    }
                }
            }
        }
        public bool EquipClothes(ItemInstance clothes)
        {
            return clothes.CurrentMeshInstance == null && clothes.TryEquipAsClothes(this);
        }
        public void UnEquipCollidingClothes(ItemInstance clothes) => UnEquipCollidingClothes(clothes.SlotRaw);
        public void UnEquipCollidingClothes(byte slots)
        {
            if (Item.Collides(EquippedSlots, slots))
            {
                for (int k = EquippedClothes.Count; k >= 0; k--)
                {
                    if (EquippedClothes[k].Collides(slots))
                    {
                        EquippedClothes[k].Uneqip(this);
                    }
                }
            }
        }
        public bool ForceEquipSkeletalClothes(ItemInstance clothes)
        {

            UnEquipCollidingClothes(clothes);
            GameObject clothesInstance = Instantiate(clothes.Mesh, this.transform);
            SkinnedMeshRenderer renderer = clothesInstance.GetComponentInChildren<SkinnedMeshRenderer>();
            AddSkeletalMesh(renderer);
            clothes.CurrentMeshInstance = clothesInstance;
            Debug.Assert((clothes.SlotRaw & EquippedSlots) == 0);
            EquippedSlots |= clothes.SlotRaw;
            return true;
        }
        public bool ForceEquipStaticClothes(ItemInstance clothes, string parentBone)
        {

            UnEquipCollidingClothes(clothes);
            GameObject clothesInstance = Instantiate(clothes.Mesh, this.transform);
            AddStaticMesh(clothesInstance, parentBone);
            clothes.CurrentMeshInstance = clothesInstance;
            Debug.Assert((clothes.SlotRaw & EquippedSlots) == 0);
            EquippedSlots |= clothes.SlotRaw;
            return true;
        }
        public void ForceUnequipClothes(ItemInstance itemInstance)
        {
            if (EquippedClothes.Remove(itemInstance))
            {
                Destroy(itemInstance.CurrentMeshInstance);
                itemInstance.CurrentMeshInstance = null;
                Debug.Assert((itemInstance.SlotRaw | EquippedSlots) == EquippedSlots);
                Debug.Assert((itemInstance.SlotRaw & EquippedSlots) != 0);
                EquippedSlots ^= itemInstance.SlotRaw;
            }
        }

        internal void AddSkeletalMesh(SkinnedMeshRenderer renderer)
        {
            Transform[] clothesBones = renderer.bones;
            for (int i = 0; i < clothesBones.Length; i++)
            {
                Transform originalBone = originalBones[clothesBones[i].name];
                if (originalBone != null)
                {
                    clothesBones[i] = originalBone;
                }
            }
            renderer.bones = clothesBones;
            renderer.rootBone = Body.rootBone;
        }
        public bool EquipWeapon(ItemInstance weapon)
        {
            Debug.Assert(weapon.Owner == this);
            return weapon.TryEquipInHand(this);
        }
        public GameObject ForceEquipWeapon(ItemInstance weapon, string parentBoneName)
        {
            GameObject weaponInstance = AddStaticMesh(weapon.Mesh, parentBoneName);
            if (weaponInstance != null)
            {
                EquippedInHand = weapon;
                Debug.Assert(EquippedInHand.Type != null);
                weapon.CurrentMeshInstance = weaponInstance;
            }
            return weaponInstance;
        }

        public override void ForceUnequipWeapon()
        {
            if (EquippedInHand != null)
            {

                Destroy(EquippedInHand.CurrentMeshInstance);
                EquippedInHand.CurrentMeshInstance = null;
                EquippedInHand = null;
            }
        }
        public GameObject AddStaticMesh(GameObject mesh, string parentBoneName)
        {
            Transform parentBone = originalBones[parentBoneName];
            if (parentBone != null)
            {
                return Instantiate(mesh, parentBone);
            }
            return null;
        }

        public override void RemoveAllItems()
        {
            Strip();
            base.RemoveAllItems();
        }
        public override void Unequip(ItemInstance item)
        {
            item.Uneqip(this);
        }
    }
}