using UnityEditor;
using UnityEngine;

public class HierarchyLabelMenu
{
    [MenuItem("GameObject/Hierarchy Label", false, 20)]
    private static void AddHierarchyLabelComponent(MenuCommand menuCommand)
    {
        GameObject selectedObject = menuCommand.context as GameObject;
        GameObject newObject = null;

        if (selectedObject != null)
        {
            newObject = new GameObject("Hierarchy Label");
            newObject.transform.parent = selectedObject.transform;
        }
        else
        {
            newObject = new GameObject("Hierarchy Label");
        }
        
        HierarchyLabel hierarchyLabel = newObject.GetComponent<HierarchyLabel>();
        if (hierarchyLabel == null)
        {
            hierarchyLabel = newObject.AddComponent<HierarchyLabel>();
        }
        
        Undo.RegisterCreatedObjectUndo(newObject, "Add Hierarchy Label");
        
        Selection.activeGameObject = newObject;
    }
}
