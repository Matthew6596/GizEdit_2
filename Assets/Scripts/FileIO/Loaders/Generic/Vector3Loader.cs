using System;
using UnityEngine;

public class Vector3Loader : PropertyLoader
{
    public override string Name => "int";

    public override void Load(byte[] bytes, ref int index)
    {
        _value = new Vector3(BitConverter.ToSingle(bytes, index), BitConverter.ToSingle(bytes, index+4), BitConverter.ToSingle(bytes, index+8));
        index += 12;
    }

    public override void Load(string text, ref int index)
    {
        //To-do
    }

    public override void Load(string[] lines, ref int index)
    {
        //To-do
    }
}
