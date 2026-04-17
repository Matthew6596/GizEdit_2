using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Events;

public class ChildProperty : TTProperty, IObjectProperty
{
    public Transform Parent { get=>(Value as TTObject).transform.parent; set { (Value as TTObject).transform.parent = value; } }

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

    public override void RefreshValueDisplays(object value)
    {
        if (Value == null) return;
        foreach(var prop in (Value as TTObject).properties) prop.RefreshValueDisplays(prop.Value);
    }

    public override IEnumerable<byte> ToBytes()
    {
        List<byte> bytes = new();
        foreach (var prop in ((TTObject)Value).properties) bytes.AddRange(prop.ToBytes());
        return bytes;
    }

    public override void ResetToDefault()
    {
        return; //TEMP
        if (!IsDefaultNull) base.ResetToDefault();
        foreach (var prop in (Value as TTObject).properties) prop.ResetToDefault();
    }

    public override void Destroy() => (Value as TTObject).Destroy();

    public void ParentObjects(Transform parent) => (Value as TTObject).transform.SetParent(parent);
}
