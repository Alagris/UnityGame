using Items;
using UnityEngine;
namespace Inter
{
    public interface IInteractableMessage
    {
        public string InteractMessage(GameObject interactor, ItemInstance tool, ref RaycastHit hit);

    }

}
