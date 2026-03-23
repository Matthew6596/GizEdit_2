using UnityEngine;

public class TorpMachineSectionLoader : GizmoSectionLoader
{
    public override string Name => "Torp Machine Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "Torp Machine")) return;

        TorpMachineSection section = TTObjectManager.Create<TorpMachineSection>(Name, 1);
        RawProperty.Add(section, bytes, ref index, _value);

        _value = section;
    }
}
