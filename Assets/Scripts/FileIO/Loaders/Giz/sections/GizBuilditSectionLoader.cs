using UnityEngine;

public class GizBuilditSectionLoader : GizmoSectionLoader
{
    public override string Name => "GizBuildit Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "GizBuildit")) return;
        base.Load(bytes, ref index);
        _value = null;
    }
}
