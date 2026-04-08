using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ProcTerrainInstance : MonoBehaviour, ISpawnedInstance, IInteractable
{
    private ProcTerrainSection procTerrainSection;
    public void Interact(GameObject interactor, ItemInstance tool, RaycastHit hit)
    {
        Debug.Log("Interacted");
    }

    public void Setup(ProcTerrainSection procTerrainSection)
    {
        this.procTerrainSection = procTerrainSection;
    }
}
