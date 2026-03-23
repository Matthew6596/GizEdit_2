using UnityEngine;

public class ShadowEditorSectionLoader : GizmoSectionLoader
{
    public override string Name => "ShadowEditor Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "ShadowEditor")) return;

        ShadowEditorSection section = TTObjectManager.Create<ShadowEditorSection>(Name, 1);
        RawProperty.Add(section, bytes, ref index, _value);

        _value = section;
    }
}
