using UnityEngine;

public class GizForceSectionLoader : GizmoSectionLoader
{
    public override string Name => "GizForce Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "GizForce")) return;
        base.Load(bytes, ref index);
        _value = null;
    }
}
