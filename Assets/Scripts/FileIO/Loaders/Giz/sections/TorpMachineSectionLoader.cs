using UnityEngine;

public class TorpMachineSectionLoader : GizmoSectionLoader
{
    public override string Name => "Torp Machine Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "Torp Machine")) return;
        base.Load(bytes, ref index);
        _value = null;
    }
}
