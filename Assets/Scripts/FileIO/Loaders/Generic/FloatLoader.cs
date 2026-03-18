using System;
using UnityEngine;

public class FloatLoader : PropertyLoader
{
    public override string Name => "float";

    public override void Load(byte[] bytes, ref int index)
    {
        _value = BitConverter.ToSingle(bytes, index);
        index += 4;
    }

    public override void Load(string text, ref int index)
    {
        throw new NotImplementedException();
    }

    public override void Load(string[] lines, ref int index)
    {
        if (float.TryParse(lines[index], out float v))
        {
            _value = v;
            index++;
        }
    }
}
