using System.Collections.Generic;
using UnityEngine;

public class SpinnerLoader : PropertyLoader
{
    public override string Name => "Spinner";

    public override void Load(byte[] bytes, ref int index)
    {
        Spinner spinner = TTObjectManager.Create<Spinner>(Name);
        List<TTProperty> props = new();

        _value = spinner;
    }
}
