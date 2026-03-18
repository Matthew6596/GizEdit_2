using System.Collections.Generic;
using UnityEngine;

public class PanelLoader : PropertyLoader
{
    public override string Name => "Panel";

    public override void Load(byte[] bytes, ref int index)
    {
        Panel panel = TTObjectManager.Create<Panel>(Name);
        List<TTProperty> props = new();

        _value = panel;
    }
}
