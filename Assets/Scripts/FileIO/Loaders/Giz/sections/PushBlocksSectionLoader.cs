using UnityEngine;

public class PushBlocksSectionLoader : GizmoSectionLoader
{
    public override string Name => "PushBlocks Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "PushBlocks")) return;

        PushBlocksSection section = TTObjectManager.Create<PushBlocksSection>(Name, 1);
        RawProperty.Add(section, bytes, ref index, _value);

        _value = section;
    }
}
