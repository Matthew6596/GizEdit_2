using UnityEngine;

public class MiniCutSectionLoader : GizmoSectionLoader
{
    public override string Name => "MiniCut Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "MiniCut")) return;
        base.Load(bytes, ref index);
        _value = null;
    }
}
