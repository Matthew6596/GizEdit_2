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

        var hierarchyBtn = EditorUIManager.Instance.AddObjectToHierarchy(name, 2, () => { lever.GeneratePropertyPanel(); });

        lever.AddProperty(new StringFixLenProperty("Name", name, 16, "", (e) => 
        {
            //update labels?
            hierarchyBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = e.value.ToString();
        }));
        index += 16;

        // ### Position ###
        lever.AddProperty(new PositionProperty("Position", LoadBytes<Vector3, Vector3Loader>(bytes, ref index), lever.transform, ""));

        // ### Angle ###
        //Change to AngleProperty
        var ang = BitConverter.ToUInt16(bytes, index);
        index += 2;
        lever.AddProperty(new FloatProperty("Angle", ang / (float)ushort.MaxValue, FloatProperty.FloatType.Float, "", (e) =>
        {
            lever.transform.rotation = Quaternion.Euler(0, e.value.Convert<float>(), 0);
        }));

        // ### (Handle Studs) Color ###
        lever.AddProperty(new EnumProperty("Color", bytes[index], Lever.StudColors, "The color of the lever handles.", (e) =>
        {
            //update lever material?
        }));
        index++;

        if(version >= 2)
        {
            // ### Multiple Pulls ###
            lever.AddProperty(new BoolProperty("Multiple Pulls", bytes[index] == 1, "Whether the lever can be pulled multiple times or only once."));
            index++;
        }

        if (version >= 3)
        {
            // ### Pull Time ###
            lever.AddProperty(new FloatProperty("Pull Time", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "..."));
        }

        if (version >= 4)
        {
            // ### Invisible ###
            lever.AddProperty(new BoolProperty("Invisible", bytes[index] == 1, "Whether the lever model is invisible."));
            index++;
        }

        if (version >= 5)
        {
            // ### Target Position ###
            lever.AddProperty(new PositionProperty("Target Position", LoadBytes<Vector3, Vector3Loader>(bytes, ref index), lever.ActivationTarget.transform, "Position of the red activation target relative to the lever."));

            // ### Target Size ###
            lever.AddProperty(new FloatProperty("Target Size", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "The scale of the red activation target."));
        }

        if (version >= 6)
        {
            // ### Target Invisible ###
            lever.AddProperty(new BoolProperty("Target Invisible", bytes[index] == 1, "Whether the red activation target is invisible."));
            index++;
        }

        _value = lever;
    }
}
