using UnityEngine;

[CreateAssetMenu(fileName = "ItemWeapon", menuName = "Scriptable Objects/ItemWeapon")]
public class ItemWeapon : Item
{
    [SerializeField]
    public int CharacterID;

    [SerializeField]
    public string ParentBone;
}
