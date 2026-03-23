using UnityEngine;

public class PanelSectionLoader : GizmoSectionLoader
{
    public override string Name => "Panel Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "Panel")) return;

        PanelSection section = TTObjectManager.Create<PanelSection>(Name, 1);
        RawProperty.Add(section, bytes, ref index, _value);

        _value = section;
    }
}
