using UnityEngine;

public class GizObstacleSectionLoader : GizmoSectionLoader
{
    public override string Name => "GizObstacle Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "GizObstacle")) return;

        GizObstacleSection section = TTObjectManager.Create<GizObstacleSection>(Name, 1);
        RawProperty.Add(section, bytes, ref index, _value);

        _value = section;
    }
}
