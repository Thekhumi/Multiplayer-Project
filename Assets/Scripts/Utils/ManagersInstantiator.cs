// Mono Framework
using System;
using System.Collections;
using System.Collections.Generic;

// Unity Framework
using UnityEngine;

public class ManagersInstantiator : MonoBehaviour
{
    /// <summary>
    /// Prefab to instantiate
    /// </summary>
    public GameObject[] prefabsToInstantiate;

    /// <summary>
    /// Prefabs to not nest inside this manager
    /// </summary>
    public GameObject[] prefabsToNotNest;

    public void Awake()
    {
        //Some of the instantiated prefabs need to be at (0,0,0) / no rotation / no scale, so place
        //ourselves in that position before instantiating them
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        Debug.Log($"prefabsToInstantiate.Length: {prefabsToInstantiate.Length}");

        for (int i = 0; i < prefabsToInstantiate.Length; i++)
        {
            GameObject prefab = prefabsToInstantiate[i];

            Debug.Log("Instantiating " + prefab);

            GameObject go = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
            go.transform.SetParent(transform);

            go.name = prefab.name;

            Debug.Log(prefab + " Instantiated");
        }
    }
}
