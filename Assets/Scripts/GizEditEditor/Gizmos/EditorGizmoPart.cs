using System;
using UnityEngine;
using UnityEngine.Events;

public class EditorGizmoPart<T> : MouseInteractable
{
    public override CursorType CursorType => CursorType.Normal;

    [NonSerialized]
    public UnityEvent<T> OnValueChange = new();

    public T value;
}
