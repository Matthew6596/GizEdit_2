using UnityEngine;

public class PushBlocksSectionLoader : GizmoSectionLoader
{
    public override string Name => "PushBlocks Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "PushBlocks")) return;
        base.Load(bytes, ref index);
        _value = null;
    }
}
