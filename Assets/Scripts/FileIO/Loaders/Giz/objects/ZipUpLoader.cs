using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class ZipUpLoader : PropertyLoader
{
    public override string Name => "ZipUp";

    private int version;

    public ZipUpLoader(int version)
    {
        this.version = version;
    }

    public override void Load(byte[] bytes, ref int index)
    {
        ZipUp zip = TTObjectManager.Create<ZipUp>(Name);

        // ### Name ###
        string name = Encoding.UTF8.GetString(bytes, index, 16);
        zip.AddProperty(new StringFixLenProperty("Name", name, 16, ""));
        index += 16;

        // ### Start ###
        PositionProperty startPosProp = new("Start", LoadBytes<Vector3, Vector3Loader>(bytes, ref index), zip.StartTransform, "...");
        zip.AddProperty(startPosProp);

        // ### Axis ###
        zip.AddProperty(new PositionProperty("Axis", LoadBytes<Vector3, Vector3Loader>(bytes, ref index), zip.AxisTransform, "...") { isSecondaryPosGiz = true, primaryPosProperty = startPosProp });

        // ### End ###
        zip.AddProperty(new PositionProperty("End", LoadBytes<Vector3, Vector3Loader>(bytes, ref index), zip.EndTransform, "...") { isSecondaryPosGiz = true, primaryPosProperty = startPosProp });

        // ### Unknown 1 ###
        zip.AddProperty(new IntegerProperty("Unknown 1", LoadBytes<short, ShortLoader>(bytes, ref index), IntegerProperty.IntType.Short, "..."));

        // ### Unknown 2 ###
        zip.AddProperty(new IntegerProperty("Unknown 2", LoadBytes<short, ShortLoader>(bytes, ref index), IntegerProperty.IntType.Short, "..."));

        // ### Swing ###
        zip.AddProperty(new BoolProperty("Swing", bytes[index] != 0, "..."));
        index++;

        // ### Unknown 3 ###
        zip.AddProperty(new BoolProperty("Unknown 3", bytes[index] != 0, "..."));
        index++;

        // ### Two Way ###
        zip.AddProperty(new BoolProperty("Two Way", bytes[index] != 0, "..."));
        index++;

        // ### Invisible ###
        bool invis = false;
        if (version >= 2)
        {
            invis = bytes[index] != 0;
            index++;
        }
        if (ShouldAddProperty(version, v => v >= 2))
        {
            // ## Invisible ##
            zip.AddProperty(new BoolProperty("Invisible", invis, "..."));
        }

        // ### Unknown 4 ###
        bool unk4 = false;
        if (version >= 3)
        {
            unk4 = bytes[index] != 0;
            index++;
        }
        if (ShouldAddProperty(version, v => v >= 3))
        {
            // ## Unknown 4 ##
            zip.AddProperty(new BoolProperty("Unknown 4", unk4, "..."));
        }

        // ### Target(s) Invisible? ###
        bool targInvis = false;
        if (version >= 4)
        {
            targInvis = bytes[index] != 0;
            index++;
        }
        if (ShouldAddProperty(version, v => v >= 4))
        {
            // ## Target(s) Invisible? ##
            zip.AddProperty(new BoolProperty("Target(s) Invisible?", targInvis, "..."));
        }

        _value = zip;
    }
}
