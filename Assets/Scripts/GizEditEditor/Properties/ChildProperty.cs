using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChildProperty : TTProperty
{
    public ChildProperty(string name, TTObject value, string info = "", UnityAction<ChangeEventData> onValueChange = null, TTObject defaultValue = null) : base(name, defaultValue, value, onValueChange, info)
    {

    }

    public override void GenerateField(Transform parent)
    {
        Transform newParent = EditorUIManager.Instance.CreateContentArea(parent, LayoutMode.Vertical);
        var obj = Value as TTObject;
        foreach(var prop in obj.properties)
        {
            prop.GenerateField(newParent);
        }
    }

    public override IEnumerable<byte> ToBytes()
    {
        List<byte> bytes = new();
        foreach (var prop in Value.Convert<TTObject>().properties) bytes.AddRange(prop.ToBytes());
        return bytes;
    }
}
