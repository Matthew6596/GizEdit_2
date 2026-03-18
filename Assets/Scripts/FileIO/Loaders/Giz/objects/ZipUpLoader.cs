using System.Collections.Generic;
using UnityEngine;

public class ZipUpLoader : PropertyLoader
{
    public override string Name => "ZipUp";

    public override void Load(byte[] bytes, ref int index)
    {
        ZipUp zip = TTObjectManager.Create<ZipUp>(Name);
        List<TTProperty> props = new();

        _value = zip;
    }
}
