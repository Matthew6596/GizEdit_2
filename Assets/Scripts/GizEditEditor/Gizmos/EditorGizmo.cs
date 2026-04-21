using System;
using UnityEngine;
using UnityEngine.Events;

public class EditorGizmo : MonoBehaviour
{
    [NonSerialized]
    public UnityEvent<object> OnValueChange = new();
    public object Value { get; set; }

    public string[] validStates = new string[0];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetEditStates(params string[] states)
    {
        validStates = states;
        if (EditorGizmoManager.IsEditState(states)) Show();
        else Hide();
    }

    protected T GetValue<T>() => (T)Value;

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }
}
