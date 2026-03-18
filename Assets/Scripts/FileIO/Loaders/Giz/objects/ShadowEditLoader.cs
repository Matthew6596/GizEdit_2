using System.Collections.Generic;
using UnityEngine;

public class ShadowEditLoader : PropertyLoader
{
    public override string Name => "ShadowEdit";

    public override void Load(byte[] bytes, ref int index)
    {
        ShadowEdit shadowEdit = TTObjectManager.Create<ShadowEdit>(Name);
        List<TTProperty> props = new();

        _value = shadowEdit;
    }
}
