using Inter;
using Items;
using UnityEngine;

public class ProcTerrainInstance : MonoBehaviour, ISpawnedInstance, IInteractable, IInteractableMessage
{
    private ProcTerrainSection procTerrainSection;
    [SerializeField]
    string InteractionMessage="";
    public void Interact(GameObject interactor, ItemInstance tool, ref RaycastHit hit)
    {
        Debug.Log("Interacted");
    }

    public string InteractMessage(GameObject interactor, ItemInstance tool, ref RaycastHit hit)
    {
        return InteractionMessage.Length == 0 ? null : InteractionMessage;
    }

    public void Setup(ProcTerrainSection procTerrainSection)
    {
        this.procTerrainSection = procTerrainSection;
    }
}
