using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class GizBuilditLoader : PropertyLoader
{
    public override string Name => "GizBuildit";

    private int version;

    public GizBuilditLoader(int version)
    {
        this.version = version;
    }

    public override void Load(byte[] bytes, ref int index)
    {
        GizBuildit buildit = TTObjectManager.Create<GizBuildit>(Name);

        // ### Name ###
        string name = Encoding.UTF8.GetString(bytes, index, 16);
        buildit.AddProperty(new StringFixLenProperty("Name", name, 16, ""));
        index += 16;

        // ### Position ###
        PositionProperty posProp = new("Position", LoadBytes<Vector3, Vector3Loader>(bytes, ref index), buildit.transform, "");
        buildit.AddProperty(posProp);

        // ### Special Objects ###
        GizSpecialObjectsLoader objsLoader = new();
        objsLoader.Load(bytes, ref index);
        var specialObjects = objsLoader.GetValue<GizSpecialObjects>();
        buildit.AddProperty(new ChildProperty("Special Objects", specialObjects, "", (e) => { }, objsLoader.LoadDefault()) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });

        // # Special Objects Editor Nav Buttons #
        specialObjects.PrependProperty(new NavBtnProperty($"<-- Back to GizBuildit", () => { buildit.GeneratePropertyPanel(); }));
        buildit.AddProperty(new NavBtnProperty("Special Objects -->", () => { specialObjects.GeneratePropertyPanel(); }));

        // ### Jump Intensity ###
        buildit.AddProperty(new FloatProperty("Jump Intensity", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "..."));

        // ### Unknown 1 ###
        float unk1 = 0;
        if (version <= 6) unk1 = LoadBytes<float, FloatLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v <= 6))
        {
            // ## Unknown 1 ##
            buildit.AddProperty(new FloatProperty("Unknown 1", unk1, FloatProperty.FloatType.Float, "..."));
        }

        // ### Minimum Studs Value ###
        buildit.AddProperty(new IntegerProperty("Minimum Studs Value", (ushort)LoadBytes<short, ShortLoader>(bytes, ref index), IntegerProperty.IntType.UShort, "..."));

        // ### Maximum Studs Value (or random variance?) ###
        buildit.AddProperty(new IntegerProperty("Maximum Studs Value", (ushort)LoadBytes<short, ShortLoader>(bytes, ref index), IntegerProperty.IntType.UShort, "..."));

        // ### Unknown 2 ###
        buildit.AddProperty(new IntegerProperty("Unknown 2", bytes[index], IntegerProperty.IntType.Byte, "..."));
        index++;

        // ### Unknown 3 ###
        buildit.AddProperty(new IntegerProperty("Unknown 3", bytes[index], IntegerProperty.IntType.Byte, "..."));
        index++;

        // ### Unknown 4 ###
        float unk4 = 0;
        if (version >= 6) unk4 = LoadBytes<float, FloatLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 6))
        {
            // ## Unknown 4 ##
            buildit.AddProperty(new FloatProperty("Unknown 4", unk4, FloatProperty.FloatType.Float, "..."));
        }

        // ### Unknown 5 ###
        short unk5 = 0;
        if (version == 7) unk5 = LoadBytes<short, ShortLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v == 7))
        {
            // ## Unknown 5 ##
            buildit.AddProperty(new IntegerProperty("Unknown 5", unk5, IntegerProperty.IntType.Short, "..."));
        }

        // ### Unknown 6 ###
        string unk6 = "";
        if (version >= 8) unk6 = LoadBytes<string, String8Loader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 8))
        {
            // ## Unknown 6 ##
            buildit.AddProperty(new StringProperty("Unknown 6", unk6, StringProperty.MaxSize.Byte, "..."));
        }

        // ### Studs Pitch? ###
        // ### Studs Yaw? ###
        // ### Studs Position ###
        short studPitch = 0, studYaw = 0;
        Vector3 studPos = Vector3.zero;
        if (version >= 7)
        {
            studPitch = LoadBytes<short, ShortLoader>(bytes, ref index);
            studYaw = LoadBytes<short, ShortLoader>(bytes, ref index);
            studPos = LoadBytes<Vector3, Vector3Loader>(bytes, ref index);
        }
        if (ShouldAddProperty(version, v => v >= 7))
        {
            // ## Studs Pitch? ##
            buildit.AddProperty(new IntegerProperty("Studs Pitch", studPitch, IntegerProperty.IntType.Short, "The pitch angle at which studs emit."));

            // ## Studs Yaw? ##
            buildit.AddProperty(new IntegerProperty("Studs Yaw", studYaw, IntegerProperty.IntType.Short, "The yaw angle at which studs emit."));

            // ## Studs Position ##
            GameObject studsSpawnObjTEMP = new("studs_spawn_obj_TEMP");
            studsSpawnObjTEMP.transform.SetParent(buildit.transform);
            studsSpawnObjTEMP.transform.localPosition = Vector3.zero;
            buildit.AddProperty(new PositionProperty("Studs Position", studPos, studsSpawnObjTEMP.transform, "The relative position at which studs emit.") { isSecondaryPosGiz = true, primaryPosProperty = posProp });
        }

        // ### Studs Speed ###
        float studSpd = 1.75f;
        if (version >= 9) studSpd = LoadBytes<float, FloatLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 9))
        {
            // ## Studs Speed ##
            buildit.AddProperty(new FloatProperty("Studs Speed", studSpd, FloatProperty.FloatType.Float, "The speed of the studs as they emit.", (e) => { }, 1.75f));
        }

        // ### Unknown 7 ###
        short unk7 = 0;
        if (version >= 4) unk7 = LoadBytes<short, ShortLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 4))
        {
            // ## Unknown 7 ##
            buildit.AddProperty(new IntegerProperty("Unknown 7", unk7, IntegerProperty.IntType.Short, "..."));
        }

        // ### Unknown 8 ###
        // ### Unknown 9 ###
        short unk8 = 0;
        string unk9 = "";
        if (version >= 5)
        {
            unk8 = LoadBytes<short, ShortLoader>(bytes, ref index);
            unk9 = LoadBytes<string, String8Loader>(bytes, ref index);
        }
        if (ShouldAddProperty(version, v => v >= 5))
        {
            // ## Unknown 8 ##
            buildit.AddProperty(new IntegerProperty("Unknown 8", unk8, IntegerProperty.IntType.Short, "..."));

            // ## Unknown 9 ##
            buildit.AddProperty(new StringProperty("Unknown 9", unk9, StringProperty.MaxSize.Byte, "..."));
        }

        _value = buildit;
    }
}
