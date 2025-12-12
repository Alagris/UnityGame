using UnityEngine;

[CreateAssetMenu(fileName = "CharacterType", menuName = "Scriptable Objects/CharacterType")]
public class CharacterType : ScriptableObject
{
    [SerializeField]
    public int CharacterID;

    [SerializeField]
    public string Name;

    [SerializeField]
    public GameObject Prefab;

    [SerializeField]
    public float SlowWalkSpeed=1;

    [SerializeField]
    public float WalkSpeed=4;

    [SerializeField]
    public float RunSpeed=8;
}
