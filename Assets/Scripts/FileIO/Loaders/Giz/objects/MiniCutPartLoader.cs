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

        // ### Unknown 1 ###
        part.AddProperty(new FloatProperty("Unknown 1", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "..."));

        // ### Unknown 2 ###
        part.AddProperty(new FloatProperty("Unknown 2", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "..."));

        // ### Unknown 3 ###
        part.AddProperty(new FloatProperty("Unknown 3", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "..."));

        // ### Unknown 4 ###
        part.AddProperty(new FloatProperty("Unknown 4", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "..."));

        // ### Unknown 5 ###
        part.AddProperty(new IntegerProperty("Unknown 5", LoadBytes<short, ShortLoader>(bytes, ref index), IntegerProperty.IntType.Short, "..."));

        // ### Unknown 6 ###
        part.AddProperty(new IntegerProperty("Unknown 6", LoadBytes<short, ShortLoader>(bytes, ref index), IntegerProperty.IntType.Short, "..."));

        // ### Unknown 7 ###
        part.AddProperty(new IntegerProperty("Unknown 7", LoadBytes<short, ShortLoader>(bytes, ref index), IntegerProperty.IntType.Short, "..."));

        // ### Unknown 8 ###
        part.AddProperty(new FloatProperty("Unknown 8", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "..."));

        // ### Unknown 9 ###
        part.AddProperty(new FloatProperty("Unknown 9", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "..."));

        _value = part;
    }
}
