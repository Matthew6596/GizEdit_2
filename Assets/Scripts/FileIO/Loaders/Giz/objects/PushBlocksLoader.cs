using System.Collections.Generic;
using UnityEngine;

public class PushBlocksLoader : PropertyLoader
{
    public override string Name => "PushBlocks";

    private int version;

    public PushBlocksLoader(int version)
    {
        this.version = version;
    }

    public override void Load(byte[] bytes, ref int index)
    {
        PushBlocks blocks = TTObjectManager.Create<PushBlocks>(Name);

        // ### Name ###
        string name = LoadBytes<string, String8Loader>(bytes, ref index);
        blocks.AddProperty(new StringProperty("Name", name, StringProperty.MaxSize.Byte, "Name of the PushBlock Special Object."));

        // ### Snap Range ###
        blocks.AddProperty(new FloatProperty("Snap Range", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, ""));

        // ### Push Location? ###
        blocks.AddProperty(new BoolProperty("Push Location?", bytes[index] != 0, "Whether this is where you are meant to push an object to/on."));
        index++;

        // ### Unknown 1 ###
        blocks.AddProperty(new BoolProperty("Unknown 1", bytes[index] != 0, "This is always 0 in Vanilla TCS."));
        index++;

        // ### Lock Z ###
        blocks.AddProperty(new BoolProperty("Lock Z", bytes[index] != 0, "Prevents the PushBlock from being pushed on the Z axis."));
        index++;

        // ### Lock X ###
        blocks.AddProperty(new BoolProperty("Lock X", bytes[index] != 0, "Prevents the PushBlock from being pushed on the X axis."));
        index++;

        // ### Unknown 2 ###
        // ### Unknown 3 ###
        bool unk2 = false, unk3 = false;
        if (version >= 4)
        {
            unk2 = bytes[index] != 0;
            index++;
            unk3 = bytes[index] != 0;
            index++;
        }
        if (ShouldAddProperty(version, v => v >= 4))
        {
            // ## Unknown 2 ##
            blocks.AddProperty(new BoolProperty("Unknown 2", unk2, ""));

            // ## Unknown 3 ##
            blocks.AddProperty(new BoolProperty("Unknown 3", unk3, ""));
        }

        // ### Unknown 4 ###
        // ### No Slipperiness ###
        bool unk4 = false, noslip = false;
        if (version >= 5)
        {
            unk4 = bytes[index] != 0;
            index++;
            noslip = bytes[index] != 0;
            index++;
        }
        if (ShouldAddProperty(version, v => v >= 5))
        {
            // ## Unknown 4 ##
            blocks.AddProperty(new BoolProperty("Unknown 4", unk4, "This is always 0 in Vanilla TCS."));

            // ## No Slipperiness ##
            blocks.AddProperty(new BoolProperty("No Slipperiness", noslip, "Prevents the PushBlock from slipping entirely."));
        }

        // ### PushBlock Link Object Count ###
        // ### PushBlock Link Objects ###
        IntegerProperty linkObjCountProp = null;
        ArrayProperty<StringProperty> linkObjectsProp = null;
        if (version >= 3)
        {
            // ## PushBlock Link Objects Count ##
            linkObjCountProp = new("Link Object Count", bytes[index], IntegerProperty.IntType.Byte) { generateOptions = TTProperty.FieldGenerateOptions.Hidden };
            index++;

            // ## PushBlock Link Objects ##
            linkObjectsProp = ArrayProperty<StringProperty>.Create("Link Objects", "", "Link Object", new String8Loader(), "", bytes, ref index, linkObjCountProp, (info) =>
            {
                return new StringProperty($"{info.name} Name", info.defaultValue.ToString(), StringProperty.MaxSize.Byte, "Name of a link object");
            }, (e) => { }, TTProperty.FieldGenerateOptions.None);
        }
        if (ShouldAddProperty(version, v => v >= 3))
        {
            blocks.AddProperty(linkObjCountProp);
            blocks.AddProperty(linkObjectsProp);
        }

        _value = blocks;
    }
}