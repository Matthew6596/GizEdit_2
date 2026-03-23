using UnityEngine;

public class SpinnerSectionLoader : GizmoSectionLoader
{
    public override string Name => "Spinner Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "Spinner")) return;

        SpinnerSection section = TTObjectManager.Create<SpinnerSection>(Name, 1);
        RawProperty.Add(section, bytes, ref index, _value);

        _value = section;
    }
}
