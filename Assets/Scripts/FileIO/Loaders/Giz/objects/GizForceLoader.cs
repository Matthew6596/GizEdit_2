using System.Collections.Generic;
using UnityEngine;

public class GizForceLoader : PropertyLoader
{
    public override string Name => "GizForce";

    public override void Load(byte[] bytes, ref int index)
    {
        GizForce force = TTObjectManager.Create<GizForce>(Name);
        List<TTProperty> props = new();

        _value = force;
    }
}
