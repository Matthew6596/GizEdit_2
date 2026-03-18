using System;
using UnityEngine;
using UnityEngine.Events;

public class EditorGizmo : MonoBehaviour
{
    [NonSerialized]
    public UnityEvent<object> OnValueChange = new();
    public object Value { get; set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected T GetValue<T>() => (T)Value;
}
