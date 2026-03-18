using UnityEngine;

public class blowupSectionLoader : GizmoSectionLoader
{
    public override string Name => "Blowup Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "blowup")) return;
        base.Load(bytes, ref index);
        _value = null;
    }
}
