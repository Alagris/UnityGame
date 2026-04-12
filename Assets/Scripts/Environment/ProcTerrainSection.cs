
using Env.Runtime;
using Inter;
using Items;
using UnityEngine;
public class ProcTerrainSection : ProcSection, IInteractable
{
    
    public void Interact(GameObject interactor, ItemInstance tool, ref RaycastHit hit)
    { 
        Debug.Log("Interacted ");
    }

    public override GameObject SpawnInstance(GameObject prefab, Trans transform)
    {
        GameObject inst = base.SpawnInstance(prefab, transform);
        if (inst.TryGetComponent(out ISpawnedInstance i))
        {
            i.Setup(this);
        }
        return inst;
    }
}