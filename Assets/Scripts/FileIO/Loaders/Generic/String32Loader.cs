using System;
using System.Text;
using UnityEngine;

public class String32Loader : PropertyLoader
{
    public override string Name => "String32";

    public override void Load(byte[] bytes, ref int index)
    {
        int len = BitConverter.ToInt32(bytes,index);
        index += 4;
        _value = Encoding.UTF8.GetString(bytes, index, len);
        index += len;
    }

    public override void Load(string text, ref int index)
    {
        int ind1 = text.IndexOf('"', index)+1;
        int ind2 = text.IndexOf('"', ind1) - 1;
        _value = text.Substring(ind1, ind2);
        index = ind2 + 2;
    }

    public override void Load(string[] lines, ref int index)
    {
        _value = lines[index];
        index++;
    }
}
