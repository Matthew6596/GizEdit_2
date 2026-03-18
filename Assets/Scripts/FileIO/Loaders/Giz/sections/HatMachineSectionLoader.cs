using UnityEngine;

public class HatMachineSectionLoader : GizmoSectionLoader
{
    public override string Name => "HatMachine Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "HatMachine")) return;
        base.Load(bytes, ref index);
        _value = null;
    }
}
