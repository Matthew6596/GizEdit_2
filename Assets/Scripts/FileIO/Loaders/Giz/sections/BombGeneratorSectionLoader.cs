using UnityEngine;

public class BombGeneratorSectionLoader : GizmoSectionLoader
{
    public override string Name => "BombGenerator Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "BombGenerator")) return;

        BombGeneratorSection section = TTObjectManager.Create<BombGeneratorSection>(Name, 1);
        RawProperty.Add(section, bytes, ref index, _value);

        _value = section;
    }
}
