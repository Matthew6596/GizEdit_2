using System;
using UnityEngine;

public class ShortLoader : PropertyLoader
{
    public override string Name => "short";

    public override void Load(byte[] bytes, ref int index)
    {
        _value = BitConverter.ToInt16(bytes, index);
        index += 2;
    }

    public override void Load(string text, ref int index)
    {
        //Invalid int character
        if (text[index] != '-' && (text[index] < 48 || text[index] >= 58)) return;

        //Determine string length of the int
        int len = 1;
        while (text[index + len] >= 48 && text[index + len] < 58) len++;

        //Parse int
        _value = short.Parse(text.Substring(index, len));
        index += len;
    }

    public override void Load(string[] lines, ref int index)
    {
        if (short.TryParse(lines[index], out short v))
        {
            _value = v;
            index++;
        }
    }
}
