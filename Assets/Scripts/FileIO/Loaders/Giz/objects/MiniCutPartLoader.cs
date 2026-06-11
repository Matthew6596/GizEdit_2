using System;
using System.Collections.Generic;
using UnityEngine;

public class MiniCutPartLoader : PropertyLoader
{
    public override string Name => "MiniCut Part";

    public override void Load(byte[] bytes, ref int index)
    {
        MiniCutPart part = TTObjectManager.Create<MiniCutPart>(Name);

        // ### Name ###
        string name = LoadBytes<string,String8Loader>(bytes, ref index);
        part.AddProperty(new StringProperty("Name", name, StringProperty.MaxSize.Byte, ""));

        // ### Target Position ###
        part.AddProperty(new PositionProperty("Target Position", LoadBytes<Vector3, Vector3Loader>(bytes, ref index), part.transform, "..."));

        // ### Unknown 4 ###
        part.AddProperty(new FloatProperty("Camera Distance", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "..."));

        // ### Unknown 5 ###
        part.AddProperty(new IntegerProperty("Camera Pitch", LoadBytes<short, ShortLoader>(bytes, ref index), IntegerProperty.IntType.Short, "..."));

        // ### Unknown 6 ###
        part.AddProperty(new IntegerProperty("Camera Yaw", LoadBytes<short, ShortLoader>(bytes, ref index), IntegerProperty.IntType.Short, "..."));

        // ### Unknown 7 ###
        part.AddProperty(new IntegerProperty("Camera Roll", LoadBytes<short, ShortLoader>(bytes, ref index), IntegerProperty.IntType.Short, "..."));

        // ### Unknown 8 ###
        part.AddProperty(new FloatProperty("Unknown 8", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "..."));

        // ### Unknown 9 ###
        part.AddProperty(new FloatProperty("Unknown 9", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "..."));

        _value = part;
    }
}
