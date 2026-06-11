using System.Collections.Generic;
using UnityEngine;

public class blowupLoader : PropertyLoader
{
    public override string Name => "blowup";

    private int version;

    public blowupLoader(int version)
    {
        this.version = version;
    }

    public override void Load(byte[] bytes, ref int index)
    {
        blowup blwup = TTObjectManager.Create<blowup>(Name);

        // ### blowup Object? ###
        blwup.AddProperty(new StringProperty("Blowup Type", LoadBytes<string, String8Loader>(bytes, ref index), StringProperty.MaxSize.Byte, "..."));

        // ### Name? ###
        string name = "";
        if (version >= 2) name = LoadBytes<string, String8Loader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 2)) blwup.AddProperty(new StringProperty("Name", name, StringProperty.MaxSize.Byte, "..."));

        // ### Position ###
        PositionProperty posProp = new("Position", LoadBytes<Vector3, Vector3Loader>(bytes, ref index), blwup.transform);
        blwup.AddProperty(posProp);

        // ### Unknown 1 ###
        blwup.AddProperty(new IntegerProperty("Unknown 1", LoadBytes<short, ShortLoader>(bytes, ref index), IntegerProperty.IntType.Short, "..."));

        // ### Unknown 2 ###
        blwup.AddProperty(new IntegerProperty("Unknown 2", LoadBytes<short, ShortLoader>(bytes, ref index), IntegerProperty.IntType.Short, "..."));

        // ### Unknown 3 ###
        blwup.AddProperty(new IntegerProperty("Unknown 3", LoadBytes<short, ShortLoader>(bytes, ref index), IntegerProperty.IntType.Short, "..."));

        // ### Unknown 4a ###
        int unk4a = 0;
        if (version >= 2 && version <= 19) unk4a = LoadBytes<short, ShortLoader>(bytes, ref index);
        else if (version >= 20) unk4a = LoadBytes<int, IntLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 2 && v <= 19)) blwup.AddProperty(new IntegerProperty("Unknown 4a", unk4a, IntegerProperty.IntType.Short, "..."));
        else if (ShouldAddProperty(version, v => v >= 20))
        {
            // ## Blowup Flags ##
            string[] dropOptions = new string[32];
            for (int i = 0; i < 32; i++) dropOptions[i] = "unkbit";
            dropOptions[0] = "idk?";
            dropOptions[1] = "idk|Usually on thermal";
            dropOptions[2] = "Collision";
            dropOptions[3] = "Proximity Trigger|Blow up when the player enters proximity.";
            dropOptions[7] = "Can Drop Health?";
            dropOptions[8] = "Drop Powerup";
            dropOptions[15] = "Ranged Attackable|Can be destroyed by ranged attacks. Note that some attacks like Jedi Slam count.";
            dropOptions[16] = "Melee Attackable|Can be destroyed by melee attacks.";
            dropOptions[21] = "Thermal Sticky?";
            dropOptions[24] = "Torpedo?";
            var flagsProp = new IntBitFlagsProperty("Blowup Behaviors", unk4a, dropOptions, "This value appears to have some effect on the pickups dropped from the blowup. 98437 seems to be studs, 98693 (9th bit) seems to add a powerup, 245897 and 98445 are also used. This appears to be bit flags.");
            blwup.AddProperty(flagsProp);

            // ## Blowup Flags Presets ##
            blwup.InsertPropertyBefore(new PresetsProperty("Flags Presets", new Dictionary<string, byte> {
                    { "None", 0 }, { "Default", 1 }, { "Thermal (Metal)", 2 }
            }, "", (e) =>
            {
                switch (e.value.Convert<int>())
                {
                    case 1: flagsProp.SetAllFlags(0,2,7,15,16); break;
                    case 2: flagsProp.SetAllFlags(1,2,21); break;
                    default: break;
                }
            }), flagsProp.name);

            
        }

        // ### Unknown 4b ###
        int unk4b = 0;
        if (version == 28) unk4b = LoadBytes<int, IntLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v == 28)) blwup.AddProperty(new IntegerProperty("Unknown 4b", unk4b, IntegerProperty.IntType.Int, "..."));

        // ### Unknown 5 ###
        int unk5 = 0;
        if (version >= 30) unk5 = LoadBytes<int, IntLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 30)) blwup.AddProperty(new IntegerProperty("Unknown 5", unk5, IntegerProperty.IntType.Int, "..."));

        // ### Unknown 6 ###
        // ### Unknown 7 ###
        // ### Unknown 8 ###
        int studAmt = 0;
        byte unk7 = 0, unk8 = 0;
        if (version >= 2)
        {
            studAmt = LoadBytes<int, IntLoader>(bytes, ref index);
            unk7 = bytes[index];
            index++;
            unk8 = bytes[index];
            index++;
        }
        if (ShouldAddProperty(version, v => v >= 2))
        {
            // ## Studs Value ##
            blwup.AddProperty(new IntegerProperty("Studs Value", studAmt, IntegerProperty.IntType.Int, "..."));

            // ## Unknown 7 ##
            blwup.AddProperty(new IntegerProperty("Unknown 7", unk7, IntegerProperty.IntType.Byte, "..."));

            // ## Unknown 8 ##
            blwup.AddProperty(new IntegerProperty("Unknown 8", unk8, IntegerProperty.IntType.Byte, "..."));
        }

        // ### Damage ###
        byte dmg = 0;
        if (version >= 4)
        {
            dmg = bytes[index];
            index++;
        }
        if (ShouldAddProperty(version, v => v >= 4)) blwup.AddProperty(new IntegerProperty("Damage", dmg, IntegerProperty.IntType.Byte, "..."));

        // ### Range ###
        float range = 0;
        if (version >= 6) range = LoadBytes<float, FloatLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 6)) blwup.AddProperty(new FloatProperty("Range", range, FloatProperty.FloatType.Float, "..."));

        // ### Unknown 11 ###
        // ### Unknown 12 ###
        float unk11 = 0, unk12 = 0;
        if (version >= 8)
        {
            unk11 = LoadBytes<float, FloatLoader>(bytes, ref index);
            unk12 = LoadBytes<float, FloatLoader>(bytes, ref index);
        }
        if (ShouldAddProperty(version, v => v >= 8))
        {
            // ## Unknown 11 ##
            blwup.AddProperty(new FloatProperty("Unknown 11", unk11, FloatProperty.FloatType.Float, "..."));

            // ## Unknown 12 ##
            blwup.AddProperty(new FloatProperty("Unknown 12", unk12, FloatProperty.FloatType.Float, "..."));
        }

        // ### Unknown 13 ###
        // ### Unknown 14 ###
        // ### Unknown 15 ###
        // ### Unknown 16 ###
        // ### Unknown 17 ###
        // ### Unknown 18 ###
        short unk13 = 0, unk14 = 0, unk15 = 0;
        float unk16 = 0, unk17 = 0, unk18 = 0;
        if (version >= 9)
        {
            unk13 = LoadBytes<short, ShortLoader>(bytes, ref index);
            unk14 = LoadBytes<short, ShortLoader>(bytes, ref index);
            unk15 = LoadBytes<short, ShortLoader>(bytes, ref index);
            unk16 = LoadBytes<float, FloatLoader>(bytes, ref index);
            unk17 = LoadBytes<float, FloatLoader>(bytes, ref index);
            unk18 = LoadBytes<float, FloatLoader>(bytes, ref index);
        }
        if (ShouldAddProperty(version, v => v >= 9))
        {
            // ## Unknown 13 ##
            blwup.AddProperty(new IntegerProperty("Unknown 13", unk13, IntegerProperty.IntType.Short, "..."));

            // ## Unknown 14 ##
            blwup.AddProperty(new IntegerProperty("Unknown 14", unk14, IntegerProperty.IntType.Short, "..."));

            // ## Unknown 15 ##
            blwup.AddProperty(new IntegerProperty("Unknown 15", unk15, IntegerProperty.IntType.Short, "..."));

            // ## Unknown 16 ##
            blwup.AddProperty(new FloatProperty("Unknown 16", unk16, FloatProperty.FloatType.Float, "..."));

            // ## Unknown 17 ##
            blwup.AddProperty(new FloatProperty("Unknown 17", unk17, FloatProperty.FloatType.Float, "..."));

            // ## Unknown 18 ##
            blwup.AddProperty(new FloatProperty("Unknown 18", unk18, FloatProperty.FloatType.Float, "..."));
        }

        // ### Unknown 19 ###
        float unk19 = 0;
        if (version >= 10) unk19 = LoadBytes<float, FloatLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 10)) blwup.AddProperty(new FloatProperty("Unknown 19", unk19, FloatProperty.FloatType.Float, "..."));

        // ### Unknown 20 ###
        // ### Unknown 21 ###
        // ### Unknown 22 ###
        float unk20 = 0, unk21 = 0, unk22 = 0;
        if (version >= 11)
        {
            unk20 = LoadBytes<float, FloatLoader>(bytes, ref index);
            unk21 = LoadBytes<float, FloatLoader>(bytes, ref index);
            unk22 = LoadBytes<float, FloatLoader>(bytes, ref index);
        }
        if (ShouldAddProperty(version, v => v >= 11))
        {
            // ## Unknown 20 ##
            blwup.AddProperty(new FloatProperty("Unknown 20", unk20, FloatProperty.FloatType.Float, "..."));

            // ## Unknown 21 ##
            blwup.AddProperty(new FloatProperty("Unknown 21", unk21, FloatProperty.FloatType.Float, "..."));

            // ## Unknown 22 ##
            blwup.AddProperty(new FloatProperty("Unknown 22", unk22, FloatProperty.FloatType.Float, "..."));
        }

        // ### Unknown 23 ###
        byte unk23 = 0;
        if (version >= 12)
        {
            unk23 = bytes[index];
            index++;
        }
        if (ShouldAddProperty(version, v => v >= 12)) blwup.AddProperty(new IntegerProperty("Unknown 23", unk23, IntegerProperty.IntType.Byte, "..."));

        // ### Unknown 24 ###
        // ### Unknown 25 ###
        short unk24 = 0, unk25 = 0;
        if (version >= 13)
        {
            unk24 = LoadBytes<short, ShortLoader>(bytes, ref index);
            unk25 = LoadBytes<short, ShortLoader>(bytes, ref index);
        }
        if (ShouldAddProperty(version, v => v >= 13))
        {
            // ## Unknown 24 ##
            blwup.AddProperty(new IntegerProperty("Unknown 24", unk24, IntegerProperty.IntType.Short, "..."));

            // ## Unknown 25 ##
            blwup.AddProperty(new IntegerProperty("Unknown 25", unk25, IntegerProperty.IntType.Short, "..."));
        }

        // ### Unknown 26 ###
        // ### Unknown 27 ###
        // ### Unknown 28 ###
        // ### Unknown 29 ###
        // ### Unknown 30 ###
        // ### Unknown 31 ###
        // ### Unknown 32 ###
        short unk26 = 0, unk27 = 0, unk28 = 0;
        float unk29 = 0, unk30 = 0, unk31 = 0, unk32 = 0;
        if (version >= 19)
        {
            unk26 = LoadBytes<short, ShortLoader>(bytes, ref index);
            unk27 = LoadBytes<short, ShortLoader>(bytes, ref index);
            unk28 = LoadBytes<short, ShortLoader>(bytes, ref index);
            unk29 = LoadBytes<float, FloatLoader>(bytes, ref index);
            unk30 = LoadBytes<float, FloatLoader>(bytes, ref index);
            unk31 = LoadBytes<float, FloatLoader>(bytes, ref index);
            unk32 = LoadBytes<float, FloatLoader>(bytes, ref index);
        }
        if (ShouldAddProperty(version, v => v >= 19))
        {
            // ## Unknown 26 ##
            blwup.AddProperty(new IntegerProperty("Unknown 26", unk26, IntegerProperty.IntType.Short, "..."));

            // ## Unknown 27 ##
            blwup.AddProperty(new IntegerProperty("Unknown 27", unk27, IntegerProperty.IntType.Short, "..."));

            // ## Unknown 28 ##
            blwup.AddProperty(new IntegerProperty("Unknown 28", unk28, IntegerProperty.IntType.Short, "..."));

            // ## Unknown 29 ##
            blwup.AddProperty(new FloatProperty("Unknown 29", unk29, FloatProperty.FloatType.Float, "..."));

            // ## Unknown 30 ##
            blwup.AddProperty(new FloatProperty("Unknown 30", unk30, FloatProperty.FloatType.Float, "..."));

            // ## Unknown 31 ##
            blwup.AddProperty(new FloatProperty("Unknown 31", unk31, FloatProperty.FloatType.Float, "..."));

            // ## Unknown 32 ##
            blwup.AddProperty(new FloatProperty("Unknown 32", unk32, FloatProperty.FloatType.Float, "..."));
        }

        // ### Unknown 33 ###
        float unk33 = 0;
        if (version >= 21) unk33 = LoadBytes<float, FloatLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 21)) blwup.AddProperty(new FloatProperty("Unknown 33", unk33, FloatProperty.FloatType.Float, "..."));

        // ### Unknown 34 ###
        float unk34 = 0;
        if (version >= 23) unk34 = LoadBytes<float, FloatLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 23)) blwup.AddProperty(new FloatProperty("Unknown 34", unk34, FloatProperty.FloatType.Float, "..."));

        // ### Unknown 19 ###
        float unk35 = 0;
        if (version >= 31) unk35 = LoadBytes<float, FloatLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 31)) blwup.AddProperty(new FloatProperty("Unknown 35", unk35, FloatProperty.FloatType.Float, "..."));

        _value = blwup;
    }
}
