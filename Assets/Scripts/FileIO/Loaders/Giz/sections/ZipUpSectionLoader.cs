using System.Linq;
using UnityEngine;

public class ZipUpSectionLoader : GizmoSectionLoader
{
    public override string Name => "ZipUp Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "ZipUp")) return;

        ZipUpSection section = TTObjectManager.Create<ZipUpSection>(Name, 1);
        RawProperty.Add(section, bytes, ref index, _value);

        _value = section;
    }
}
