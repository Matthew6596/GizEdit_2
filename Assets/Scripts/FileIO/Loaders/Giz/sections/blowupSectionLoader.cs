using UnityEngine;

public class blowupSectionLoader : GizmoSectionLoader
{
    public override string Name => "Blowup Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "blowup")) return;

        blowupSection section = TTObjectManager.Create<blowupSection>(Name, 1);
        RawProperty.Add(section, bytes, ref index, _value);

        _value = section;
    }
}
