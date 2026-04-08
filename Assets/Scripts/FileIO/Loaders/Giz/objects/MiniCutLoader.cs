using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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
        minicut.AddProperty(new FloatProperty("Unknown 1", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "..."));

        // ### Unknown 2 ###
        minicut.AddProperty(new FloatProperty("Unknown 2", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "..."));

        // ### Unknown 3 ###
        minicut.AddProperty(new FloatProperty("Unknown 3", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "..."));

        // ### Unknown 4 ###
        minicut.AddProperty(new FloatProperty("Unknown 4", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "..."));

        // ### Unknown 5 ###
        minicut.AddProperty(new FloatProperty("Unknown 5", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "..."));

        // ### MiniCut Parts ###
        MiniCutPartsLoader objsLoader = new();
        objsLoader.Load(bytes, ref index);
        var parts = objsLoader.GetValue<MiniCutParts>();
        minicut.AddProperty(new ChildProperty("MiniCut Parts", parts, "", (e) => { }, objsLoader.LoadDefault()) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });

        // # MiniCut Parts Editor Nav Buttons #
        parts.PrependProperty(new NavBtnProperty($"<-- Back to MiniCut", () => { minicut.GeneratePropertyPanel(); }));
        minicut.AddProperty(new NavBtnProperty("MiniCut Parts -->", () => { parts.GeneratePropertyPanel(); }));

        _value = minicut;
    }
}
