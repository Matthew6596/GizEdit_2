using System.Collections.Generic;
using UnityEngine;

public class TorpMachineLoader : PropertyLoader
{
    public override string Name => "Torp Machine";

    public override void Load(byte[] bytes, ref int index)
    {
        TorpMachine torpMachine = TTObjectManager.Create<TorpMachine>(Name);
        List<TTProperty> props = new();

        _value = torpMachine;
    }
}
