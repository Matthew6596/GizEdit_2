using System;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class SpinnerLoader : PropertyLoader
{
    public override string Name => "Spinner";

    private int version;

    public SpinnerLoader(int version)
    {
        this.version = version;
    }

    public override void Load(byte[] bytes, ref int index)
    {
        Spinner spinner = TTObjectManager.Create<Spinner>(Name);

        // ### Name ###
        string name = LoadBytes<string, String8Loader>(bytes, ref index);
        spinner.AddProperty(new StringProperty("Name", name, StringProperty.MaxSize.Byte, ""));

        // ### Position ###
        PositionProperty posProp = new("Position", LoadBytes<Vector3, Vector3Loader>(bytes, ref index), spinner.transform);
        spinner.AddProperty(posProp);

        // ### Angle ###
        ushort ang = BitConverter.ToUInt16(bytes, index);
        index += 2;
        spinner.AddProperty(new AngleProperty("Angle", ang, spinner.transform));

        // ### Spinner Special Object ###
        spinner.AddProperty(new StringProperty("Spinner Special Object", LoadBytes<string, String8Loader>(bytes, ref index), StringProperty.MaxSize.Byte, ""));

        // ### Unknown Count ###
        byte unkCount = bytes[index];
        index++;
        IntegerProperty unkCountProp = new("Unknown Count", unkCount, IntegerProperty.IntType.Byte, "...");
        spinner.AddProperty(unkCountProp);

        // ### Flap Count ###
        byte flapCount = 0;
        if (version >= 2)
        {
            flapCount = bytes[index];
            index++;
        }
        if (ShouldAddProperty(version, v => v >= 2))
        {
            // ## Flap Count ##
            spinner.AddProperty(new IntegerProperty("Flap Count", version < 2 ? 4 : flapCount, IntegerProperty.IntType.Byte, "", (e) =>
            {
                spinner.UpdateFlaps(e.value.Convert<int>());
            }, 4));
        }

        bool flapsExist = flapCount != 0;

        // ### Unknown 1 ###
        // ### Unknown 2 ###
        int unk1 = 0;
        float unk2 = 0;
        if (flapsExist && version >= 3)
        {
            unk1 = LoadBytes<int, IntLoader>(bytes, ref index);
            unk2 = LoadBytes<float, FloatLoader>(bytes, ref index);
        }
        if (ShouldAddProperty(version, v => flapsExist && v >= 3))
        {
            // ## Unknown 1 ##
            spinner.AddProperty(new IntegerProperty("Unknown 1", unk1, IntegerProperty.IntType.Int, "..."));

            // ## Unknown 2 ##
            spinner.AddProperty(new FloatProperty("Unknown 2", unk2, FloatProperty.FloatType.Float, "..."));
        }

        // ### Unknown 3 ###
        float unk3 = 0;
        if (flapsExist && version >= 4) unk3 = LoadBytes<float, FloatLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => flapsExist && v >= 4))
        {
            // ## Unknown 3 ##
            spinner.AddProperty(new FloatProperty("Unknown 3", unk3, FloatProperty.FloatType.Float, "..."));
        }

        // ### Unknown 4 ###
        byte unk4 = 0;
        if (unk1 != 0 && version >= 6)
        {
            unk4 = bytes[index];
            index++;
        }
        if (ShouldAddProperty(version, v => unk1 != 0 && v >= 6))
        {
            // ## Unknown 4 ##
            spinner.AddProperty(new IntegerProperty("Unknown 4", unk4, IntegerProperty.IntType.Byte, "..."));
        }

        // !!! Format for version <= 4 excluded here !!!

        // ### Special Objects ###
        GizSpecialObjectsLoader objsLoader = new();
        objsLoader.Load(bytes, ref index);
        var specialObjects = objsLoader.GetValue<GizSpecialObjects>();
        spinner.AddProperty(new ChildProperty("Special Objects", specialObjects, "", (e) => { }, objsLoader.LoadDefault()) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });

        // # Special Objects Editor Nav Buttons #
        specialObjects.PrependProperty(new NavBtnProperty($"<-- Back to Spinner", () => { spinner.GeneratePropertyPanel(); }));
        spinner.AddProperty(new NavBtnProperty("Special Objects -->", () => { specialObjects.GeneratePropertyPanel(); }));

        // !!! Format for version <= 6 excluded here !!!

        // ### Unknown 5 ###
        ArrayProperty<FloatProperty> unk5Prop = null;
        if (version >= 7)
        {
            // ## Unknown 5 ##
            unk5Prop = ArrayProperty<FloatProperty>.Create("Unknown 5", "", "Unk5", new FloatLoader(), 0f, bytes, ref index, unkCountProp, (info) =>
            {
                return new FloatProperty($"{info.name}", (float)info.defaultValue, FloatProperty.FloatType.Float, "...");
            }, (e) => { }, TTProperty.FieldGenerateOptions.None);

            unkCountProp.generateOptions = TTProperty.FieldGenerateOptions.Hidden;
        }
        if (ShouldAddProperty(version, v => v >= 7)) spinner.AddProperty(unk5Prop);

        // ### Unknown 6 ###
        // ### Unknown 7 ###
        float unk6 = 0, unk7 = 0;
        if (version >= 8)
        {
            unk6 = LoadBytes<float, FloatLoader>(bytes, ref index);
            unk7 = LoadBytes<float, FloatLoader>(bytes, ref index);
        }
        if (ShouldAddProperty(version, v => v >= 8))
        {
            // ## Unknown 6 ##
            spinner.AddProperty(new FloatProperty("Unknown 6", unk6, FloatProperty.FloatType.Float, "..."));

            // ## Unknown 7 ##
            spinner.AddProperty(new FloatProperty("Unknown 7", unk7, FloatProperty.FloatType.Float, "..."));
        }

        _value = spinner;
    }
}
