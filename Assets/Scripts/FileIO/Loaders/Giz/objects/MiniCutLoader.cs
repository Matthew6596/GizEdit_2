using System;
using System.Collections.Generic;
using UnityEngine;

public class MiniCutLoader : PropertyLoader
{
    public override string Name => "MiniCut";

    public override void Load(byte[] bytes, ref int index)
    {
        MiniCut minicut = TTObjectManager.Create<MiniCut>(Name);

        // ### Name ###
        string name = LoadBytes<string, String8Loader>(bytes, ref index);
        minicut.AddProperty(new StringProperty("Name", name, StringProperty.MaxSize.Byte));

        // ### Unknown 1 ###
        minicut.AddProperty(new FloatProperty("Start Delay", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "..."));

        // ### Unknown 2 ###
        minicut.AddProperty(new FloatProperty("Duration", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "..."));

        // ### Unknown 3 ###
        minicut.AddProperty(new FloatProperty("Blend In Time", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "..."));

        // ### Unknown 4 ###
        minicut.AddProperty(new FloatProperty("Blend Out Time", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "..."));

        // ### Unknown 5 ###
        minicut.AddProperty(new FloatProperty("Max Total Duration?", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "..."));

        // ### MiniCut Parts ###
        MiniCutPartsLoader objsLoader = new();
        objsLoader.Load(bytes, ref index);
        var parts = objsLoader.GetValue<MiniCutParts>();
        minicut.AddProperty(new ChildProperty("MiniCut Parts", parts, "") { generateOptions = TTProperty.FieldGenerateOptions.Hidden });

        // # MiniCut Parts Editor Nav Buttons #
        parts.PrependProperty(new NavBtnProperty($"<-- Back to MiniCut", () => { minicut.GeneratePropertyPanel(); }));
        minicut.AddProperty(new NavBtnProperty("MiniCut Parts -->", () => { parts.GeneratePropertyPanel(); }));

        _value = minicut;
    }
}
