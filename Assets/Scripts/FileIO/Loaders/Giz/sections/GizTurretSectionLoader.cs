using UnityEngine;

public class GizTurretSectionLoader : GizmoSectionLoader
{
    public override string Name => "GizTurret Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "GizTurret")) return;
        base.Load(bytes, ref index);
        _value = null;
    }
}
