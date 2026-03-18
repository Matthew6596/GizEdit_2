using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

public class EditorGizmoManager : MonoBehaviour
{
    public static EditorGizmoManager Instance { get; private set; }

    private readonly static List<EditorGizmo> gizmos = new();

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static T Create<T>(object initialValue, UnityAction<object> callback) where T : EditorGizmo
    {
        GameObject gizmoObj = new("editor_gizmo");
        T giz = gizmoObj.AddComponent<T>();
        giz.Value = initialValue;
        giz.OnValueChange.AddListener(callback);
        gizmos.Add(giz);
        return giz;
    }

    public static void DestroyAllGizmos()
    {
        for(int i=gizmos.Count-1; i>=0; i--)
        {
            Destroy(gizmos[i].gameObject);
            gizmos.RemoveAt(i);
        }
    }
}