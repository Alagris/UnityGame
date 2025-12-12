using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "RuleSet", menuName = "Scriptable Objects/RuleSet")]
public class RuleSet : ScriptableObject
{
    [SerializeField]
    public int initial;

    [SerializeField]
    public Dictionary<string, Rules> symbols;

    [System.Serializable]
    [IncludeInSettings(true)]
    public class Rules
    {
        [SerializeField]
        public string superNeem;

        [SerializeField]
        public string Feem;

    }


}
