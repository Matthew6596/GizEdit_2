using UnityEngine;

public class GizObstacleSectionLoader : GizmoSectionLoader
{
    public override string Name => "GizObstacle Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "GizObstacle")) return;
        base.Load(bytes, ref index);
        _value = null;
    }
}
