using Assets.Scripts.Environment;
using Env.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
public class ProcTerrainSection : ProcSection, IInteractable
{
    
    public void Interact(GameObject interactor, ItemInstance tool, RaycastHit hit)
    {
        
    }

    public override GameObject SpawnInstance(GameObject prefab, Matrix4x4 transform)
    {
        GameObject inst = base.SpawnInstance(prefab, transform);
        if (inst.TryGetComponent(out ISpawnedInstance i))
        {
            i.Setup(this);
        }
        return inst;
    }
}