using System.Collections.Generic;
using UnityEngine;

public class MiniCutLoader : PropertyLoader
{
    public override string Name => "MiniCut";

    public override void Load(byte[] bytes, ref int index)
    {
        MiniCut minicut = TTObjectManager.Create<MiniCut>(Name);
        List<TTProperty> props = new();

        _value = minicut;
    }
}
