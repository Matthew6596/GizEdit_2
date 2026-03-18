using System.Collections.Generic;
using UnityEngine;

public class BombGeneratorLoader : PropertyLoader
{
    public override string Name => "BombGenerator";

    public override void Load(byte[] bytes, ref int index)
    {
        BombGenerator bombGen = TTObjectManager.Create<BombGenerator>(Name);
        List<TTProperty> props = new();

        _value = bombGen;
    }
}
