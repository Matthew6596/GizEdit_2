using UnityEngine;

public class GizBuilditSectionLoader : GizmoSectionLoader
{
    public override string Name => "GizBuildit Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "GizBuildit")) return;

        GizBuilditSection section = TTObjectManager.Create<GizBuilditSection>(Name, 1);
        RawProperty.Add(section, bytes, ref index, _value);

        _value = section;
    }
}
