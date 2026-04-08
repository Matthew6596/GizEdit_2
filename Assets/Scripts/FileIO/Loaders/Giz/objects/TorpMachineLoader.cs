using System;
using System.Collections.Generic;
using UnityEngine;

public class TorpMachineLoader : PropertyLoader
{
    public override string Name => "Torp Machine";

    private int version;

    public TorpMachineLoader(int version)
    {
        this.version = version;
    }

    public override void Load(byte[] bytes, ref int index)
    {
        TorpMachine torpMachine = TTObjectManager.Create<TorpMachine>(Name);

        // ### Name ###
        string name = LoadBytes<string, String32Loader>(bytes, ref index);
        torpMachine.AddProperty(new StringProperty("Name", name, StringProperty.MaxSize.Int, ""));

        // ### Position ###
        torpMachine.AddProperty(new PositionProperty("Position", LoadBytes<Vector3, Vector3Loader>(bytes, ref index), torpMachine.transform));

        // ### Angle ###
        ushort ang = BitConverter.ToUInt16(bytes, index);
        index += 2;
        torpMachine.AddProperty(new AngleProperty("Angle", ang, torpMachine.transform));

        // ### Red Outline ###
        bool red = false;
        if (version >= 2)
        {
            red = bytes[index] != 0;
            index++;
        }
        if (ShouldAddProperty(version, v => v >= 2))
        {
            // ## Red Outline ##
            torpMachine.AddProperty(new BoolProperty("Red Outline", red, "...", (e) =>
            {
                torpMachine.SetOutlineColor((bool)e.value);
            }));
        }

        _value = torpMachine;
    }
}
