using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    [SerializeField]
    public string ItemName;
    
    [SerializeField]
    public int ItemID;

    [SerializeField]
    public GameObject Mesh;
}
