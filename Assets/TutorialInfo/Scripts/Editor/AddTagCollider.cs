using UnityEditor;
using UnityEngine;

public class AddCollidersByTag : EditorWindow
{
    private string targetTag = "POI_TagArt";
    private bool isTrigger = true;
    private Vector3 size = Vector3.one;

    [MenuItem("Tools/Add Box Colliders by Tag")]
    public static void ShowWindow()
    {
        GetWindow<AddCollidersByTag>("Add Colliders by Tag");
    }

    private void OnGUI()
    {
        GUILayout.Label("Add Box Collider to all objects with tag", EditorStyles.boldLabel);

        targetTag = EditorGUILayout.TagField("Tag", targetTag);
        isTrigger = EditorGUILayout.Toggle("Is Trigger", isTrigger);
        size = EditorGUILayout.Vector3Field("Size", size);

        GUILayout.Space(10);

        if (GUILayout.Button("Add to All in Scene"))
            AddColliders();
    }

    private void AddColliders()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        int count = 0;

        foreach (GameObject go in targets)
        {
            if (go.GetComponent<BoxCollider>() != null) continue;

            Undo.RecordObject(go, "Add Box Collider");
            BoxCollider col = Undo.AddComponent<BoxCollider>(go);
            col.isTrigger = isTrigger;
            col.size = size;
            EditorUtility.SetDirty(go);
            count++;
        }

        Debug.Log($"[AddColliders] Added Box Colliders to {count} objects with tag '{targetTag}'.");
    }
}