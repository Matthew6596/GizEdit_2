using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class LeverLoader : PropertyLoader
{
    private int version;

    public override string Name => "Lever";

    public LeverLoader(int version)
    {
        this.version = version;
    }

    public override void Load(byte[] bytes, ref int index)
    {
        Lever lever = TTObjectManager.Create<Lever>(Name);

        // ### Name ###
        string name = Encoding.UTF8.GetString(bytes, index, 16);
        lever.AddProperty(new StringFixLenProperty("Name", name, 16, ""));
        index += 16;

        // ### Position ###
        PositionProperty posProp = new("Position", LoadBytes<Vector3, Vector3Loader>(bytes, ref index), lever.transform, "");
        lever.AddProperty(posProp);

        // ### Angle ###
        var ang = BitConverter.ToUInt16(bytes, index);
        index += 2;
        lever.AddProperty(new AngleProperty("Angle", ang, lever.transform, ""));

        // ### (Handle Studs) Color ###
        lever.AddProperty(new EnumProperty("Handle Color", bytes[index], Lever.StudColors, "The color of the lever handles.", (e) =>
        {
            lever.UpdateHandleColor((byte)e.value);
        }, (byte)'y'));
        index++;

        // ### Multiple Pulls ###
        bool multiPulls = false;
        if (version >= 2)
        {
            multiPulls = bytes[index] != 0;
            index++;
        }
        if (ShouldAddProperty(version, v => v >= 2))
        {
            // ## Multiple Pulls ##
            lever.AddProperty(new BoolProperty("Multiple Pulls", multiPulls, "Whether the lever can be pulled multiple times or stays down when pulled once."));
        }

        // ### Pull Time ###
        float pullTime = 1.5f;
        if (version >= 3) pullTime = LoadBytes<float, FloatLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 3))
        {
            // ## Pull Time ##
            lever.AddProperty(new FloatProperty("Pull Time", pullTime, FloatProperty.FloatType.Float, "..."));
        }

        // ### Invisible ###
        bool invis = false;
        if (version >= 4)
        {
            invis = bytes[index] != 0;
            index++;
        }
        if (ShouldAddProperty(version, v => v >= 4))
        {
            // ## Invisible ##
            lever.AddProperty(new BoolProperty("Invisible", invis, "Whether the base of the lever is invisible."));
        }

        // ### Target Position ###
        // ### Target Size ###
        Vector3 targPos = Vector3.zero;
        float targSize = 1f;
        if (version >= 5)
        {
            targPos = LoadBytes<Vector3, Vector3Loader>(bytes, ref index);
            targSize = LoadBytes<float, FloatLoader>(bytes, ref index);
        }
        if (ShouldAddProperty(version, v => v >= 5))
        {
            // ## Target Position ##
            lever.AddProperty(new PositionProperty("Target Position", targPos, lever.ActivationTarget.transform, "Position of the red activation target relative to the lever.") { isSecondaryPosGiz = true, primaryPosProperty = posProp });

            // ## Target Size ##
            lever.AddProperty(new FloatProperty("Target Size", targSize, FloatProperty.FloatType.Float, "The scale of the red activation target."));
        }

        // ### Target Invisible ###
        bool targInvis = false;
        if (version >= 6)
        {
            targInvis = bytes[index] != 0;
            index++;
        }
        if (ShouldAddProperty(version, v => v >= 6))
        {
            // ## Target Invisible ##
            lever.AddProperty(new BoolProperty("Target Invisible", targInvis, "Whether the red activation target is invisible."));
        }

        _value = lever;
    }
}
