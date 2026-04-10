using System;
using System.Collections.Generic;
using UnityEngine;

public class PanelLoader : PropertyLoader
{
    public override string Name => "Panel";

    private int version;

    public PanelLoader(int version) 
    {  
        this.version = version; 
    }

    public override void Load(byte[] bytes, ref int index)
    {
        Panel panel = TTObjectManager.Create<Panel>(Name);

        // ### Name ###
        string name = LoadBytes<string, String32Loader>(bytes, ref index);
        panel.AddProperty(new StringProperty("Name", name, StringProperty.MaxSize.Int, ""));

        // ### Position ###
        PositionProperty posProp = new("Position", LoadBytes<Vector3, Vector3Loader>(bytes, ref index), panel.transform);
        panel.AddProperty(posProp);

        // ### Angle ###
        ushort ang = BitConverter.ToUInt16(bytes, index);
        index += 2;
        panel.AddProperty(new AngleProperty("Angle", ang, panel.transform, ""));

        // ### Type ###
        panel.AddProperty(new EnumProperty("Type", bytes[index], Panel.PanelTypes, "", (e) =>
        {
            panel.UpdatePanelType((byte)e.value);
        }));
        index++;

        // ### Invisible ###
        bool invis = false;
        if (version >= 3)
        {
            invis = bytes[index] != 0;
            index++;
        }
        if (ShouldAddProperty(version, v => v >= 3))
        {
            // ## Invisible ##
            panel.AddProperty(new BoolProperty("Invisible", invis, "Whether the base of the panel is invisible."));
        }

        // ### Target Position ###
        // ### Target Size ###
        Vector3 targPos = Vector3.zero;
        float targSize = 1f;
        if (version >= 4)
        {
            targPos = LoadBytes<Vector3, Vector3Loader>(bytes, ref index);
            targSize = LoadBytes<float, FloatLoader>(bytes, ref index);
        }
        if (ShouldAddProperty(version, v => v >= 4))
        {
            // ## Target Position ##
            panel.AddProperty(new PositionProperty("Target Position", targPos, panel.ActivationTarget.transform, "") { isSecondaryPosGiz = true, primaryPosProperty = posProp });

            // ## Target Size ##
            panel.AddProperty(new FloatProperty("Target Size", targSize, FloatProperty.FloatType.Float, ""));
        }

        // ### Target Invisible ###
        bool targInvis = false;
        if (version >= 5)
        {
            targInvis = bytes[index] != 0;
            index++;
        }
        if (ShouldAddProperty(version, v => v >= 5))
        {
            // ## Target Invisible ##
            panel.AddProperty(new BoolProperty("Target Invisible", targInvis, ""));
        }

        // ### Alternative Face ###
        // ### Alternative Body ###
        bool altFace = false, altBody = false;
        if (version >= 6)
        {
            altFace = bytes[index] != 0;
            index++;
            altBody = bytes[index] != 0;
            index++;
        }
        if (ShouldAddProperty(version, v => v >= 6))
        {
            // ## Alternative Face ##
            panel.AddProperty(new BoolProperty("Alternative Face", altFace, "Only applies to Astromech and Protocol Droid types.", (e) =>
            {
                panel.ToggleAlternativeFace((bool)e.value);
            }));

            // ## Alternative Body ##
            panel.AddProperty(new BoolProperty("Alternative Body", altBody, "Only applies to Astromech and Protocol Droid types. The Protocol Droid type uses the same alternative body as the Astromech type, and thus looks incorrect.", (e) =>
            {
                panel.ToggleAlternativeBody((bool)e.value);
            }));
        }

        // ### Unknown 1 ###
        bool unk1 = false;
        if (version >= 7)
        {
            unk1 = bytes[index] != 0;
            index++;
        }
        if (ShouldAddProperty(version, v => v >= 7))
        {
            // ## Unknown 1 ##
            panel.AddProperty(new BoolProperty("Unknown 1", unk1, ""));
        }

        // ### Unknown 2 ###
        bool unk2 = false;
        if (version >= 8)
        {
            unk2 = bytes[index] != 0;
            index++;
        }
        if (ShouldAddProperty(version, v => v >= 8))
        {
            // ## Unknown 2 ##
            panel.AddProperty(new BoolProperty("Unknown 2", unk2, "Almost always false, except for panel3 in deathstarrescue_a."));
        }

        _value = panel;
    }
}
