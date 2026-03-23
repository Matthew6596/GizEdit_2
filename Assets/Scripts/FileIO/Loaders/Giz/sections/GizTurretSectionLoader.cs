using UnityEngine;

public class GizTurretSectionLoader : GizmoSectionLoader
{
    public override string Name => "GizTurret Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "GizTurret")) return;

        GizTurretSection section = TTObjectManager.Create<GizTurretSection>(Name, 1);
        RawProperty.Add(section, bytes, ref index, _value);

        _value = section;
    }
}
