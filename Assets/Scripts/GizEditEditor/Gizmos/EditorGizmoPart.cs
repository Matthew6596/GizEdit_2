using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class EditorGizmoPart : MouseInteractable { }

public class EditorGizmoPart<T> : EditorGizmoPart
{
    public override CursorType CursorType => CursorType.Normal;

    [NonSerialized]
    public UnityEvent<T> OnValueChange = new();

    public T value;
}
