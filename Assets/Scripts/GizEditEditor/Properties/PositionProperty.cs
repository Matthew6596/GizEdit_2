using UnityEngine;
using UnityEngine.Events;

public class PositionProperty : Vector3Property
{
    public Transform target;
    private PositionGizmo posGiz;

    public PositionProperty(string name, Vector3 value, Transform target, string info = "", UnityAction<ChangeEventData> onValueChange = null, Vector3 defaultValue = default) : base(name, value, info, onValueChange, defaultValue)
    {
        this.target = target;
    }

    public override void GenerateField(Transform parent)
    {
        //generate vector3 field in property panel
        base.GenerateField(parent);

        //create position gizmo
        posGiz = EditorGizmoManager.Create<PositionGizmo>(Value, (e) => { Value = (Vector3)e; });
        posGiz.transform.SetParent(target);
        posGiz.transform.localPosition = Vector3.zero;
    }

    public override void RefreshValueDisplays(object value)
    {
        base.RefreshValueDisplays(value);
        target.localPosition = (Vector3)value;
        if (posGiz != null) posGiz.RefreshValues();
    }
}
