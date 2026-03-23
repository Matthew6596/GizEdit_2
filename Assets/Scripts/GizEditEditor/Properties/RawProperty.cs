using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RawProperty : TTProperty
{
    public RawProperty(byte[] value) : base("raw_data", new byte[0], value, (e) => { }, "")
    {
    }

    public override void GenerateField(Transform parent){}

    public override IEnumerable<byte> ToBytes() => (byte[])Value;

    public static void Add(TTObject obj, byte[] bytes, ref int index, object size)
    {
        int len = (int)size;
        RawProperty prop = new(bytes.Skip(index).Take(len).ToArray());
        index += len;
        obj.AddProperty(prop);
    }
}
