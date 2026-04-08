using System;
using System.Collections.Generic;
using UnityEngine;

public class HatMachineLoader : PropertyLoader
{
    public override string Name => "HatMachine";

    private int version;

    public HatMachineLoader(int version)
    {
        this.version = version;
    }

    public override void Load(byte[] bytes, ref int index)
    {
        HatMachine hatMachine = TTObjectManager.Create<HatMachine>(Name);

        // ### Name ###
        string name = LoadBytes<string, String32Loader>(bytes, ref index);
        hatMachine.AddProperty(new StringProperty("Name", name, StringProperty.MaxSize.Int, ""));

        // ### Position ###
        PositionProperty posProp = new("Position", LoadBytes<Vector3, Vector3Loader>(bytes, ref index), hatMachine.transform, "");
        hatMachine.AddProperty(posProp);

        // ### Angle ###
        ushort ang = BitConverter.ToUInt16(bytes, index);
        index += 2;
        hatMachine.AddProperty(new AngleProperty("Angle", ang, hatMachine.transform));

        // ### Type ###
        hatMachine.AddProperty(new EnumProperty("Hat Type", bytes[index], HatMachine.HatTypes, "The type of hat given by the hat machine.", (e) =>
        {
            hatMachine.UpdateHatType((int)e.value);
        }, 5));
        index++;

        // ### Handle Color ###
        byte col = 2;
        if (version >= 3)
        {
            col = bytes[index];
            index++;
        }
        if (ShouldAddProperty(version, v => v >= 3))
        {
            // ## Handle Color ##
            hatMachine.AddProperty(new EnumProperty("Handle Color", col, Lever.StudColors, "The color of the lever handles.", (e) =>
            {
                hatMachine.UpdateHandleColor((int)e.value);
            }, 2));
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
            hatMachine.AddProperty(new PositionProperty("Target Position", targPos, hatMachine.ActivationTarget.transform) { isSecondaryPosGiz = true, primaryPosProperty = posProp });

            // ## Target Size ##
            hatMachine.AddProperty(new FloatProperty("Target Size", targSize, FloatProperty.FloatType.Float, "", (e) => { }, 1.5f));
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
            hatMachine.AddProperty(new BoolProperty("Target Invisible", targInvis, ""));
        }

        _value = hatMachine;
    }
}
