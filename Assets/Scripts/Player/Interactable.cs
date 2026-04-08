using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public interface IInteractable
{
    public void Interact(GameObject interactor, ItemInstance tool, RaycastHit hit);
}

