using System.Collections.Generic;
using UnityEngine;

public class GizBuilditLoader : PropertyLoader
{
    public override string Name => "GizBuildit";

    public override void Load(byte[] bytes, ref int index)
    {
        GizBuildit buildit = TTObjectManager.Create<GizBuildit>(Name);
        List<TTProperty> props = new();

        _value = buildit;
    }
}
