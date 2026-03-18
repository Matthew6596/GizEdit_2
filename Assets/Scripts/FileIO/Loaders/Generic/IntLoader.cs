using System;
using UnityEngine;

public class IntLoader : PropertyLoader
{
    public override string Name => "int";

    public override void Load(byte[] bytes, ref int index)
    {
        _value = BitConverter.ToInt32(bytes, index);
        index += 4;
    }

    public override void Load(string text, ref int index)
    {
        //Invalid int character
        if (text[index] != '-' && (text[index] < 48 || text[index] >= 58)) return;

        //Determine string length of the int
        int len = 1;
        while (text[index + len] >= 48 && text[index + len] < 58) len++;

        //Parse int
        _value = int.Parse(text.Substring(index, len));
        index += len;
    }

    public override void Load(string[] lines, ref int index)
    {
        if (int.TryParse(lines[index], out int v))
        {
            _value = v;
            index++;
        }
    }
}
