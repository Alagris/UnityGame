using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    [SerializeField]
    int CharacterID;

    [SerializeField]
    List<ItemClothing> EquippedClothes;

    [SerializeField]
    List<ItemWeapon> EquippedWeapons;

    List<GameObject> EquippedClothesInstances = new List<GameObject>();

    [SerializeField]
    List<Item> Items;

    [SerializeField]
    SkinnedMeshRenderer Body;

    readonly Dictionary<string, Transform> originalBones = new Dictionary<string, Transform>();
    public void SetBody(int characterID, SkinnedMeshRenderer body)
    {
        CharacterID = characterID;
        Body = body;
        originalBones.Clear();
        foreach (var b in Body.bones)
        {
            originalBones.Add(b.name, b);
        }
        EquipClothes();
        EquipWeapons();
    }
    public void EquipClothes()
    {


        for (int k = 0; k < EquippedClothes.Count; k++)
        {

            ItemClothing clothes = EquippedClothes[k];
            if (clothes.CharacterID == CharacterID)
            {
                GameObject clothesInstance = Instantiate(clothes.Mesh, this.transform);
                SkinnedMeshRenderer renderer = clothesInstance.GetComponentInChildren<SkinnedMeshRenderer>();
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
                EquippedClothesInstances.Add(clothesInstance);
            }

        }
    }
    public void EquipWeapons() { 
        for (int k = 0; k < EquippedWeapons.Count; k++)
        {

            ItemWeapon weapon = EquippedWeapons[k];
            Transform parentBone = originalBones[weapon.ParentBone];
            if (parentBone != null)
            {
                GameObject weaponInstance = Instantiate(weapon.Mesh, parentBone);
                EquippedClothesInstances.Add(weaponInstance);
            }

        }

    }

    // Update is called once per frame
  
}
