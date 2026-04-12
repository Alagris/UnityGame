using Items;
using UnityEngine;
namespace Inter
{
    public interface IInteractable
    {
        public void Interact(GameObject interactor, ItemInstance tool, ref RaycastHit hit);

    }

}
