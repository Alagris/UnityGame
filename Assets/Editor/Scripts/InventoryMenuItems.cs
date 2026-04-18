using Items;
using UnityEditor;
using UnityEngine;

public class InventoryMenuItems
{
    [MenuItem("GameObject/3D/Item", false, 10)]
    static void CreateItemObject(MenuCommand menuCommand)
    {
        GameObject go = new GameObject("Item");
        go.AddComponent<ItemObject>();
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }
}
