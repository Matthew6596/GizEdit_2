using UnityEngine;

public class MiniCutSectionLoader : GizmoSectionLoader
{
    public override string Name => "MiniCut Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "MiniCut")) return;

        MiniCutSection section = TTObjectManager.Create<MiniCutSection>(Name, 1);
        RawProperty.Add(section, bytes, ref index, _value);

        _value = section;
    }
}
