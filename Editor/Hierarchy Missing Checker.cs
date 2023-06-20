using UnityEditor;
using UnityEngine;

namespace Praecipua.EE
{
	[InitializeOnLoad]
	public static class MissingScriptWarning
	{
	    static MissingScriptWarning()
	    {
	        EditorApplication.hierarchyWindowItemOnGUI += CheckForMissingScript;
	    }

	    private static void CheckForMissingScript(int instanceID, Rect selectionRect)
	    {
	        var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

	        if (gameObject == null) return;

	        var components = gameObject.GetComponents<Component>();

	        foreach (var component in components)
	        {
	            if (component == null)
	            {
	                var rect = new Rect(selectionRect);
	                rect.x = selectionRect.xMax - 16 - 16;
	                rect.y = selectionRect.yMin - 1;
	                rect.width = 18;
	                rect.height = 18;

	                var WarnIcon = EditorGUIUtility.isProSkin ? "d_console.warnicon.sml" : "console.warnicon.sml";
	                GUI.Label(rect, EditorGUIUtility.IconContent(WarnIcon));
	                break;
	            }
	        }
	    }
	}
}