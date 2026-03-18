using UnityEngine;

public class ZipUpSectionLoader : GizmoSectionLoader
{
    public override string Name => "ZipUp Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "ZipUp")) return;
        base.Load(bytes, ref index);
        _value = null;
    }
}
