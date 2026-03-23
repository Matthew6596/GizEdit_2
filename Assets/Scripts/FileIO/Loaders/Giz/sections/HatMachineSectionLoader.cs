using UnityEngine;

public class HatMachineSectionLoader : GizmoSectionLoader
{
    public override string Name => "HatMachine Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "HatMachine")) return;

        HatMachineSection section = TTObjectManager.Create<HatMachineSection>(Name, 1);
        RawProperty.Add(section, bytes, ref index, _value);

        _value = section;
    }
}
