using System;
using UnityEngine;
using UnityEngine.Events;

public class PositionProperty : Vector3Property
{
    public Transform target;
    private PositionGizmo posGiz;

    public bool isSecondaryPosGiz = false;
    public PositionProperty primaryPosProperty = null;
    private bool lowPriorityGizActive = false;

    public PositionProperty(string name, Vector3 value, Transform target, string info = "", UnityAction<ChangeEventData> onValueChange = null, Vector3 defaultValue = default) : base(name, value, info, onValueChange, defaultValue)
    {
        this.target = target;
    }

    public override void GenerateField(Transform parent)
    {
        //generate vector3 field in property panel
        base.GenerateField(parent);

        if (isSecondaryPosGiz)
        {
            lowPriorityGizActive = false;
            //create button to show position gizmo
            var posGizBtn = EditorUIManager.Instance.CreateIconButton(fieldTransform, EditorUIManager.Instance.moveGizIcon, generateOptions, 14);
            posGizBtn.onClick.AddListener(() =>
            {
                EditorGizmoManager.DestroyAllGizmos();

                if (lowPriorityGizActive) primaryPosProperty.CreatePositionGizmo();
                else CreatePositionGizmo();

                lowPriorityGizActive = !lowPriorityGizActive;
            });
        }
        else CreatePositionGizmo();
    }

    public void CreatePositionGizmo()
    {
        Vector3 pos = target.parent == null ? (Vector3)Value : target.parent.TransformPoint((Vector3)Value);
        posGiz = EditorGizmoManager.Create<PositionGizmo>(pos, (e) => 
        {
            Value = target.parent == null ? (Vector3)e : target.parent.InverseTransformPoint((Vector3)e);
        });
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
