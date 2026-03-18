using System.Collections.Generic;
using UnityEngine;

public class PushBlocksLoader : PropertyLoader
{
    public override string Name => "PushBlocks";

    public override void Load(byte[] bytes, ref int index)
    {
        PushBlocks blocks = TTObjectManager.Create<PushBlocks>(Name);
        List<TTProperty> props = new();

        _value = blocks;
    }
}
