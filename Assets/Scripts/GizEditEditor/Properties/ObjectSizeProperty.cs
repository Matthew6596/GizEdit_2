using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectSizeProperty : TTProperty
{
    public ObjectSizeProperty(TTObject obj) : base("object_size", obj, obj, (e) => { }, "")
    {
    }

    public override void GenerateField(Transform parent) { }

    public override IEnumerable<byte> ToBytes()
    {
        int size = 0;
        bool counting = false;
        foreach(var prop in ((TTObject)Value).properties)
        {
            if (!counting)
            {
                if (prop == this) counting = true;
                continue;
            }

            size += prop.ToBytes().Count();
        }

        return BitConverter.GetBytes(size);
    }
}
