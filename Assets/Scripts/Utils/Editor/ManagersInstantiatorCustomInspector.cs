// Mono Framework
using System.Collections.Generic;

// Unity Framework
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ManagersInstantiator))]
public class ManagersInstantiatorCustomInspector : Editor
{
    private List<GameObject> prefabs = new List<GameObject>();

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ManagersInstantiator manager = target as ManagersInstantiator;

        prefabs.Clear();

        bool dirty = false;

        if (manager.prefabsToInstantiate != null)
            prefabs.AddRange(manager.prefabsToInstantiate);

        for (int i = 0; i < prefabs.Count; i++)
        {
            GameObject prefab = prefabs[i];

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Prefab #" + i, GUILayout.MaxWidth(80));

            GameObject newPrefab = (GameObject) EditorGUILayout.ObjectField(prefab, typeof(GameObject), false);

            if (newPrefab != prefab)
            {
                prefabs[i] = newPrefab;
                dirty = true;
            }

            if (GUILayout.Button("Up"))
            {
                if (i > 0)
                {
                    prefabs.RemoveAt(i);
                    prefabs.Insert(i - 1, prefab);
                    dirty = true;
                }
            }

            if (GUILayout.Button("Down"))
            {
                if (i < prefabs.Count - 1)
                {
                    prefabs.RemoveAt(i);
                    prefabs.Insert(i + 1, prefab);
                    dirty = true;
                }
            }

            if (GUILayout.Button("Del."))
            {
                prefabs.RemoveAt(i);
                dirty = true;
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add New"))
        {
            prefabs.Add(null);
            dirty = true;
        }

        if (dirty)
        {
            Undo.RecordObject(manager, "Manager Instantiator Modification");

            manager.prefabsToInstantiate = prefabs.ToArray();

            EditorUtility.SetDirty(manager);
        }
    }
}