using UnityEngine;

public class BombGeneratorSectionLoader : GizmoSectionLoader
{
    public override string Name => "BombGenerator Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "BombGenerator")) return;
        base.Load(bytes, ref index);
        _value = null;
    }
}
