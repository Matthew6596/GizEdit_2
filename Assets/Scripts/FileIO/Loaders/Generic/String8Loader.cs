using System.Text;
using UnityEngine;

public class String8Loader : PropertyLoader
{
    public override string Name => "String8";

    public override void Load(byte[] bytes, ref int index)
    {
        int len = bytes[index];
        index++;
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
