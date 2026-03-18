using UnityEngine;

public class ShadowEditorSectionLoader : GizmoSectionLoader
{
    public override string Name => "ShadowEditor Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "ShadowEditor")) return;
        base.Load(bytes, ref index);
        _value = null;
    }
}
