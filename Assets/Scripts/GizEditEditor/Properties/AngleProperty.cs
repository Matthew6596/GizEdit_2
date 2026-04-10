using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AngleProperty : TTProperty
{
    private FloatProperty floatField;
    //private RotationGizmo rotGiz;
    private Transform target;
    public AngleProperty(string name, ushort value, Transform target, string info = "", UnityAction<ChangeEventData> onValueChange = null, long defaultValue = 0) : base(name, defaultValue, value, onValueChange, info)
    {
        floatField = new(name, ToFloatAng(value), FloatProperty.FloatType.Float, info, (e) =>
        {
            Value = ToShortAng(e.value.Convert<float>());
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
        float angf = ToFloatAng((ushort)value);
        floatField.RefreshValueDisplays(angf);
        target.rotation = Quaternion.Euler(0, angf, 0);
    }

    public override IEnumerable<byte> ToBytes() => BitConverter.GetBytes((ushort)Value);

    private float ToFloatAng(ushort ang) => (ang / 65536f) * 360;
    private ushort ToShortAng(float ang) => (ushort)((ang / 360f) * 65536f);
}
