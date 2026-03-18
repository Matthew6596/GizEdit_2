using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

public class ChildrenProperty : TTProperty
{
    public ChildrenProperty(string name, ChildProperty[] value, string info = "", UnityAction<ChangeEventData> onValueChange = null, ChildProperty[] defaultValue = null) : base(name, defaultValue, value, onValueChange, info)
    {

    }

    public override void GenerateField(Transform parent)
    {
        Transform newParent = EditorUIManager.Instance.CreateContentArea(parent, LayoutMode.Vertical);
        var children = (ChildProperty[])Value;
        foreach(var child in children)
        {
            //TO-DO: ALSO NEED TO CREATE ARRAY MANAGEMENT
            child.GenerateField(newParent);
        }
    }

    public T[] GetChildrenValues<T>() where T : TTObject
    {
        List<T> values = new();
        foreach (var child in Value as ChildProperty[]) values.Add(child.Value as T);
        return values.ToArray();
    }

    public static ChildProperty[] LoadChildArray<T>(PropertyLoader loader, byte[] bytes, ref int index, int count, string name) where T : TTObject
    {
        List<ChildProperty> childProperties = new();
        for (int i = 0; i < count; i++)
        {
            loader.Load(bytes, ref index);
            childProperties.Add(new($"{name} {i}", loader.GetValue<T>()));
        }
        return childProperties.ToArray();
    }

    public override IEnumerable<byte> ToBytes()
    {
        List<byte> bytes = new();
        foreach (var child in Value as ChildProperty[]) bytes.AddRange(child.ToBytes());
        return bytes;
    }
}
