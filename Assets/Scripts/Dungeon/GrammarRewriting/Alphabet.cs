using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Alphabet", menuName = "Scriptable Objects/Alphabet")]
public class Alphabet: ScriptableObject
{
    [SerializeField]
    public List<string> symbols;
}
