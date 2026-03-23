using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

public class Vector3Property : TTProperty
{
    public FloatProperty X, Y, Z;
    protected Transform fieldTransform;

    public Vector3Property(string name, Vector3 value, string info="", UnityAction<ChangeEventData> onValueChange = null, Vector3 defaultValue = default) : base(name, defaultValue, value, onValueChange, info)
    {
        X = new("X", value.x, FloatProperty.FloatType.Float, "", (e) =>
        {
            Vector3 val = (Vector3)Value;
            val.x = e.value.Convert<float>();
            Value = val;
        }) 
        { generateOptions=FieldGenerateOptions.None };
        Y = new("Y", value.y, FloatProperty.FloatType.Float, "", (e) =>
        {
            Vector3 val = (Vector3)Value;
            val.y = e.value.Convert<float>();
            Value = val;
        })
        { generateOptions = FieldGenerateOptions.None };
        Z = new("Z", value.z, FloatProperty.FloatType.Float, "", (e) =>
        {
            Vector3 val = (Vector3)Value;
            val.z = e.value.Convert<float>();
            Value = val;
        })
        { generateOptions = FieldGenerateOptions.None };
    }

    public override void GenerateField(Transform parent)
    {
        fieldTransform = EditorUIManager.Instance.CreateLabeledField(parent, name, generateOptions, 100);
        X.GenerateField(fieldTransform);
        Y.GenerateField(fieldTransform);
        Z.GenerateField(fieldTransform);
    }

    public override void RefreshValueDisplays(object value)
    {
        Vector3 val = (Vector3)value;
        if (X.input != null) X.input.SetTextWithoutNotify(val.x.ToString());
        if (Y.input != null) Y.input.SetTextWithoutNotify(val.y.ToString());
        if (Z.input != null) Z.input.SetTextWithoutNotify(val.z.ToString());
    }

    public override IEnumerable<byte> ToBytes()
    {
        List<byte> bytes = new();
        bytes.AddRange(X.ToBytes());
        bytes.AddRange(Y.ToBytes());
        bytes.AddRange(Z.ToBytes());
        return bytes;
    }
}
