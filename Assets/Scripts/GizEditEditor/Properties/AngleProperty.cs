using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;

public class AngleProperty : TTProperty
{
    private FloatProperty floatField;
    //private RotationGizmo rotGiz;
    private Transform target;
    public AngleProperty(string name, ushort value, Transform target, string info = "", UnityAction<ChangeEventData> onValueChange = null, long defaultValue = 0) : base(name, defaultValue, ((float)value/(float)ushort.MaxValue)*360, onValueChange, info)
    {
        floatField = new(name, (value / (float)ushort.MaxValue)*360, FloatProperty.FloatType.Float, info, (e) =>
        {
            Value = e.value.Convert<float>();
        });
        this.target = target;
    }

    public override void GenerateField(Transform parent)
    {
        floatField.GenerateField(parent);

        //generate rotation gizmo
        //rotGiz = EditorGizmoManager.Create<RotationGizmo>(Value.Convert<float>(), (e) => { Value = e.Convert<float>(); });
        //rotGiz.transform.SetParent(target);
        //rotGiz.transform.localEulerAngles = Vector3.zero;
    }

    public override void RefreshValueDisplays(object value)
    {
        floatField.RefreshValueDisplays(value);
    }

    public override IEnumerable<byte> ToBytes()
    {
        float fval = Value.Convert<float>();
        ushort sval = (ushort)((fval / 360) * ushort.MaxValue);
        return BitConverter.GetBytes(sval);
    }
}
