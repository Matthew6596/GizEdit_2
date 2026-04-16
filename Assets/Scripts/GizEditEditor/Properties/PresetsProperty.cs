using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PresetsProperty : EnumProperty
{
    public PresetsProperty(string name, Dictionary<string, byte> options, string info = "", UnityAction<ChangeEventData> onValueChange = null, byte defaultValue = 0) : base(name, defaultValue, options, info, onValueChange, defaultValue)
    {
    }

    public override IEnumerable<byte> ToBytes() => new byte[0];
}
