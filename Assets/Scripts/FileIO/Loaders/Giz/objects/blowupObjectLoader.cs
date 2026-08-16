using System.Collections.Generic;
using UnityEngine;

public class blowupObjectLoader : PropertyLoader
{
    public override string Name => "blowupObject";

    private int version;

    public blowupObjectLoader(int version)
    {
        this.version = version;
    }

    public override void Load(byte[] bytes, ref int index)
    {
        blowupObject blowupObj = TTObjectManager.Create<blowupObject>(Name);

        // ### Special Object? ###
        StringProperty specObjProp = new("Special Object?", LoadStr8(bytes, ref index), StringProperty.MaxSize.Byte, "...") { generateOptions = TTProperty.FieldGenerateOptions.Hidden };
        blowupObj.AddProperty(specObjProp);

        // ### Name ###
        blowupObj.AddProperty(new StringProperty("Name", LoadStr8(bytes, ref index), StringProperty.MaxSize.Byte, "...", (e) =>
        {
            // !! After searching every .giz file, I found that there is no file used in game where "Special Object?" and "Name" are not equal !!
            specObjProp.Value = e.value.ToString();
        }));

        // ### .par Reference? ###
        // ### .par Reference? ###
        string parRef1 = "", parRef2 = "";
        if (version >= 17)
        {
            parRef1 = LoadStr8(bytes, ref index);
            parRef2 = LoadStr8(bytes, ref index);
        }
        if (ShouldAddProperty(version, v => v >= 17))
        {
            blowupObj.AddProperty(CreateStr8Prop("Part Type (.par ref)", parRef1));
            blowupObj.AddProperty(CreateStr8Prop("Part Type (.par ref)", parRef2));
        }

        // ### .ptl Reference? ###
        // ### .ptl Reference? ###
        // ### .ptl Reference? ###
        string ptlRef1 = "", ptlRef2 = "", ptlRef3 = "";
        if (version >= 4)
        {
            ptlRef1 = LoadStr8(bytes, ref index);
            ptlRef2 = LoadStr8(bytes, ref index);
            ptlRef3 = LoadStr8(bytes, ref index);
        }
        if (ShouldAddProperty(version, v => v >= 4))
        {
            blowupObj.AddProperty(CreateStr8Prop("Debris Effect (.ptl ref)", ptlRef1));
            blowupObj.AddProperty(CreateStr8Prop("Debris Effect (.ptl ref)", ptlRef2));
            blowupObj.AddProperty(CreateStr8Prop("Debris Effect (.ptl ref)", ptlRef3));
        }

        // ### _ Reference? ###
        // ### _ Reference? ###
        string unk1Ref1 = "", unk1Ref2 = "";
        if (version >= 26)
        {
            unk1Ref1 = LoadStr8(bytes, ref index);
            unk1Ref2 = LoadStr8(bytes, ref index);
        }
        if (ShouldAddProperty(version, v => v >= 26))
        {
            blowupObj.AddProperty(CreateStr8Prop("Debris Effect", unk1Ref1));
            blowupObj.AddProperty(CreateStr8Prop("Debris Effect", unk1Ref2));
        }

        // ### _ Reference? ###
        // ### _ Reference? ###
        string unk2Ref1 = "", unk2Ref2 = "";
        if (version >= 27)
        {
            unk2Ref1 = LoadStr8(bytes, ref index);
            unk2Ref2 = LoadStr8(bytes, ref index);
        }
        if (ShouldAddProperty(version, v => v >= 27))
        {
            blowupObj.AddProperty(CreateStr8Prop("Debris Effect", unk2Ref1));
            blowupObj.AddProperty(CreateStr8Prop("Debris Effect", unk2Ref2));
        }

        // ### Unknown 10 ###
        blowupObj.AddProperty(new IntegerProperty("Unknown 10", LoadBytes<int, IntLoader>(bytes, ref index), IntegerProperty.IntType.Int, "..."));

        // ### Unknown 11 ###
        // ### Unknown 12 ###
        int unk11 = 0;
        byte unk12 = 0;
        if (version >= 7)
        {
            unk11 = LoadBytes<int, IntLoader>(bytes, ref index);
            unk12 = bytes[index];
            index++;
        }
        if (ShouldAddProperty(version, v => v >= 7))
        {
            // ## Unknown 11 ##
            blowupObj.AddProperty(new IntegerProperty("Unknown 11", unk11, IntegerProperty.IntType.Int, "..."));

            // ## Unknown 12 ##
            blowupObj.AddProperty(new IntegerProperty("Unknown 12", unk12, IntegerProperty.IntType.Byte, "..."));
        }

        // ### Unknown 13 ###
        float unk13 = 0;
        if (version >= 8) unk13 = LoadBytes<float, FloatLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 8))
        {
            // ## Unknown 13 ##
            blowupObj.AddProperty(new FloatProperty("Unknown 13", unk13, FloatProperty.FloatType.Float, "..."));
        }

        // ### Blowup Decal ###
        string decal = "";
        if (version >= 9) decal = LoadStr8(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 9)) blowupObj.AddProperty(CreateStr8Prop("Blowup Decal", decal));

        // ### Unknown 14 ###
        // ### Unknown 15 ###
        float unk14 = 0, unk15 = 0;
        if (version >= 14)
        {
            unk14 = LoadBytes<float, FloatLoader>(bytes, ref index);
            unk15 = LoadBytes<float, FloatLoader>(bytes, ref index);
        }
        if (ShouldAddProperty(version, v => v >= 14))
        {
            // ## Unknown 14 ##
            blowupObj.AddProperty(new FloatProperty("Unknown 14", unk14, FloatProperty.FloatType.Float, "..."));

            // ## Unknown 15 ##
            blowupObj.AddProperty(new FloatProperty("Unknown 15", unk15, FloatProperty.FloatType.Float, "..."));
        }

        // ### Unknown 16 ###
        // ### Unknown 17 ###
        byte unk16 = 0, unk17 = 0;
        if (version >= 15)
        {
            unk16 = bytes[index];
            index++;
            unk17 = bytes[index];
            index++;
        }
        if (ShouldAddProperty(version, v => v >= 15))
        {
            // ## Unknown 16 ##
            blowupObj.AddProperty(new IntegerProperty("Unknown 16", unk16, IntegerProperty.IntType.Byte, "..."));

            // ## Unknown 17 ##
            blowupObj.AddProperty(new IntegerProperty("Unknown 17", unk17, IntegerProperty.IntType.Byte, "..."));
        }

        // ### Next Data ###
        // ### Unknown 18 ###
        // ### Unknown 19 ###
        // ### Unknown 20 ###
        // ### Unknown 21 ###
        // ### Unknown 22 ###
        // ### Unknown 23 ###
        // ### Unknown 24 ###
        // ### Unknown 25 ###
        // ### Unknown 26 ###
        bool nextData = false;
        List<TTProperty> nextDataProps = new();
        if (version >= 16)
        {
            nextData = bytes[index] != 0;
            index++;

            if (nextData)
            {
                // ## Unknown 18 ##
                nextDataProps.Add(new Vector3Property("Unknown 18", LoadBytes<Vector3, Vector3Loader>(bytes, ref index), "..."));

                // ## Unknown 19 ##
                nextDataProps.Add(new FloatProperty("Unknown 19", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "..."));

                // ## Unknown 20 ##
                nextDataProps.Add(new FloatProperty("Unknown 20", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "..."));

                // ## Unknown 21 ##
                nextDataProps.Add(new FloatProperty("Unknown 21", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "..."));

                // ## Unknown 22 ##
                nextDataProps.Add(new FloatProperty("Unknown 22", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "..."));

                // ## Unknown 23 ##
                nextDataProps.Add(new FloatProperty("Unknown 23", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "..."));

                // ## Unknown 24 ##
                nextDataProps.Add(new IntegerProperty("Unknown 24", LoadBytes<short, ShortLoader>(bytes, ref index), IntegerProperty.IntType.Short, "..."));

                // ## Unknown 25 ##
                nextDataProps.Add(new IntegerProperty("Unknown 25", bytes[index], IntegerProperty.IntType.Byte, "..."));
                index++;

                // ## Unknown 26 ##
                nextDataProps.Add(new IntegerProperty("Unknown 26", bytes[index], IntegerProperty.IntType.Byte, "..."));
                index++;
            }
        }
        if (ShouldAddProperty(version, v => v >= 16))
        {
            // ## Next Data ##
            blowupObj.AddProperty(new BoolProperty("Next Data", nextData, "...", (e) =>
            {
                int propsInd = blowupObj.FindPropertyIndex("Next Data") + 1;
                if ((bool)e.value) blowupObj.InsertProperties(nextDataProps.ToArray(), propsInd);
                else blowupObj.RemoveProperties(propsInd, 9);
                //blowupObj.GeneratePropertyPanel();
            }));
        }

        // ### Blowup Emit Object ###
        string emit1 = "";
        if (version >= 18) emit1 = LoadStr8(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 18)) blowupObj.AddProperty(CreateStr8Prop("Emit Object", emit1));

        // ### Blowup Emit Object ###
        // ### Blowup Emit Object ###
        // ### Blowup Emit Object ###
        string emit2 = "", emit3 = "", emit4 = "";
        if (version >= 22)
        {
            emit2 = LoadStr8(bytes, ref index);
            emit3 = LoadStr8(bytes, ref index);
            emit4 = LoadStr8(bytes, ref index);
        }
        if (ShouldAddProperty(version, v => v >= 22))
        {
            blowupObj.AddProperty(CreateStr8Prop("Emit Object", emit2));
            blowupObj.AddProperty(CreateStr8Prop("Emit Object", emit3));
            blowupObj.AddProperty(CreateStr8Prop("Emit Object", emit4));
        }

        // ### Unknown 27 ###
        // ### Unknown 28 ###
        // ### Unknown 29 ###
        byte unk27 = 0;
        float unk28 = 0, unk29 = 0;
        if (version >= 18)
        {
            unk27 = bytes[index];
            index++;
            unk28 = LoadBytes<float, FloatLoader>(bytes, ref index);
            unk29 = LoadBytes<float, FloatLoader>(bytes, ref index);
        }
        if (ShouldAddProperty(version, v => v >= 18))
        {
            blowupObj.AddProperty(new IntegerProperty("Unknown 27", unk27, IntegerProperty.IntType.Byte, "Related to emit objects?"));
            blowupObj.AddProperty(new FloatProperty("Unknown 28", unk28, FloatProperty.FloatType.Float, "Related to emit objects?"));
            blowupObj.AddProperty(new FloatProperty("Unknown 29", unk29, FloatProperty.FloatType.Float, "Related to emit objects?"));
        }

        // ### Blowup Shadow ###
        string shadow = "";
        if (version >= 19) shadow = LoadStr8(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 19)) blowupObj.AddProperty(CreateStr8Prop("Shadow", shadow));

        // ### Blowup Swap ###
        string swap = "";
        if (version >= 20) swap = LoadStr8(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 20)) blowupObj.AddProperty(CreateStr8Prop("Swap", swap));

        // ### Unknown 30 ###
        float unk30 = 0;
        if (version >= 23) unk30 = LoadBytes<float, FloatLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 23))
        {
            // ## Unknown 30 ##
            blowupObj.AddProperty(new FloatProperty("Unknown 30", unk30, FloatProperty.FloatType.Float, "..."));
        }

        // ### Unknown 31 ###
        float unk31 = 0;
        if (version >= 24) unk31 = LoadBytes<float, FloatLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 24))
        {
            // ## Unknown 31 ##
            blowupObj.AddProperty(new FloatProperty("Unknown 31", unk31, FloatProperty.FloatType.Float, "..."));
        }

        _value = blowupObj;
    }

    private string LoadStr8(byte[] bytes, ref int index) => LoadBytes<string, String8Loader>(bytes, ref index);
    private StringProperty CreateStr8Prop(string name, string val, string info = "...") => new(name, val, StringProperty.MaxSize.Byte, info);
}
