using System.Collections.Generic;
using UnityEngine;

public class HatMachineLoader : PropertyLoader
{
    public override string Name => "HatMachine";

    public override void Load(byte[] bytes, ref int index)
    {
        HatMachine hatMachine = TTObjectManager.Create<HatMachine>(Name);
        List<TTProperty> props = new();

        _value = hatMachine;
    }
}
