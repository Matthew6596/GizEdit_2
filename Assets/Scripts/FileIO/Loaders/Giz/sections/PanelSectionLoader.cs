using UnityEngine;

public class PanelSectionLoader : GizmoSectionLoader
{
    public override string Name => "Panel Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "Panel")) return;
        base.Load(bytes, ref index);
        _value = null;
    }
}
