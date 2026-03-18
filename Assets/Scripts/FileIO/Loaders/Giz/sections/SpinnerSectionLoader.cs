using UnityEngine;

public class SpinnerSectionLoader : GizmoSectionLoader
{
    public override string Name => "Spinner Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "Spinner")) return;
        base.Load(bytes, ref index);
        _value = null;
    }
}
