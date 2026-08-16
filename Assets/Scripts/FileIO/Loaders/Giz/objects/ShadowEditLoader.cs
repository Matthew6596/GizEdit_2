using System.Collections.Generic;
using UnityEngine;

public class ShadowEditLoader : PropertyLoader
{
    public override string Name => "ShadowEdit";

    private int version;

    public ShadowEditLoader(int version)
    {
        this.version = version;
    }

    public override void Load(byte[] bytes, ref int index)
    {
        ShadowEdit shadowEdit = TTObjectManager.Create<ShadowEdit>(Name);

        // ### Position ###
        Vector3Property posProp = new("Shadow Direction", LoadBytes<Vector3, Vector3Loader>(bytes, ref index));
        shadowEdit.AddProperty(posProp);

        // ### Unknown 1 ###
        shadowEdit.AddProperty(new FloatProperty("Opacity", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "..."));

        // ### Unknown 2 ###
        // ### Unknown 3 ###
        float unk2 = 0, unk3 = 0;
        if (version >= 2)
        {
            unk2 = LoadBytes<float, FloatLoader>(bytes, ref index);
            unk3 = LoadBytes<float, FloatLoader>(bytes, ref index);
        }
        if (ShouldAddProperty(version, v => v >= 2))
        {
            // ## Unknown 2 ##
            shadowEdit.AddProperty(new FloatProperty("Unknown 2", unk2, FloatProperty.FloatType.Float, "..."));

            // ## Unknown 3 ##
            shadowEdit.AddProperty(new FloatProperty("Unknown 3", unk3, FloatProperty.FloatType.Float, "..."));
        }

        // ### Unknown 4 ###
        // ### Unknown 5 ###
        float unk4 = 0, unk5 = 0;
        if (version >= 3)
        {
            unk4 = LoadBytes<float, FloatLoader>(bytes, ref index);
            unk5 = LoadBytes<float, FloatLoader>(bytes, ref index);
        }
        if (ShouldAddProperty(version, v => v >= 3))
        {
            // ## Unknown 4 ##
            shadowEdit.AddProperty(new FloatProperty("Unknown 4", unk4, FloatProperty.FloatType.Float, "..."));

            // ## Unknown 5 ##
            shadowEdit.AddProperty(new FloatProperty("Unknown 5", unk5, FloatProperty.FloatType.Float, "..."));
        }

        // ### Unknown 6 ###
        float unk6 = 0;
        if (version >= 4) unk6 = LoadBytes<float, FloatLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 4)) shadowEdit.AddProperty(new FloatProperty("Render Distance", unk6, FloatProperty.FloatType.Float, "..."));

        // ### Unknown 7 ###
        // ### Unknown 8 ###
        float unk7 = 0, unk8 = 0;
        if (version >= 5)
        {
            unk7 = LoadBytes<float, FloatLoader>(bytes, ref index);
            unk8 = LoadBytes<float, FloatLoader>(bytes, ref index);
        }
        if (ShouldAddProperty(version, v => v >= 5)) //padding
        {
            // ## Unknown 7 ##
            shadowEdit.AddProperty(new FloatProperty("Unknown 7", unk7, FloatProperty.FloatType.Float, "This is always 0 in Vanilla TCS.") { generateOptions=TTProperty.FieldGenerateOptions.Hidden});

            // ## Unknown 8 ##
            shadowEdit.AddProperty(new FloatProperty("Unknown 8", unk8, FloatProperty.FloatType.Float, "This is always 0 in Vanilla TCS.") { generateOptions = TTProperty.FieldGenerateOptions.Hidden });
        }

        // ### Unknown 9 ###
        // ### Unknown 10 ###
        // ### Unknown 11 ###
        float unk9 = 0, unk10 = 0, unk11 = 0;
        if (version >= 6)
        {
            unk9 = LoadBytes<float, FloatLoader>(bytes, ref index);
            unk10 = LoadBytes<float, FloatLoader>(bytes, ref index);
            unk11 = LoadBytes<float, FloatLoader>(bytes, ref index);
        }
        if (ShouldAddProperty(version, v => v >= 6))
        {
            // ## Unknown 9 ##
            shadowEdit.AddProperty(new FloatProperty("Blur", unk9, FloatProperty.FloatType.Float, "..."));

            // ## Unknown 10 ##
            shadowEdit.AddProperty(new FloatProperty("Unknown 10", unk10, FloatProperty.FloatType.Float, "..."));

            // ## Unknown 11 ##
            shadowEdit.AddProperty(new FloatProperty("Unknown 11", unk11, FloatProperty.FloatType.Float, "..."));
        }

        // ### Unknown 12 ###
        float unk12 = 0;
        if (version >= 7) unk12 = LoadBytes<float, FloatLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 7)) shadowEdit.AddProperty(new FloatProperty("Quality (0=better)", unk12, FloatProperty.FloatType.Float, "..."));

        // ### Unknown 13 ###
        int unk13 = 0;
        if (version >= 8) unk13 = LoadBytes<int, IntLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 8))
        {
            shadowEdit.AddProperty(new IntegerProperty("Preset", unk13, IntegerProperty.IntType.Int, "This is always 0, 1, or 2 in Vanilla TCS."));
            shadowEdit.AddProperty(new NavLblProperty("Preset 0 = Custom"));
            shadowEdit.AddProperty(new NavLblProperty("Preset 1 = Normal"));
            shadowEdit.AddProperty(new NavLblProperty("Preset 2 = Vehicle"));
        }

        _value = shadowEdit;
    }
}
