using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class TubeLoader : PropertyLoader
{
    public override string Name => "Tube";

    private int version;
    public TubeLoader(int version)
    {
        this.version = version;
    }

    public override void Load(byte[] bytes, ref int index)
    {
        Tube tube = TTObjectManager.Create<Tube>(Name);

        // ### Name ###
        string name = Encoding.UTF8.GetString(bytes, index, 16);

        var hierarchyBtn = EditorUIManager.Instance.AddObjectToHierarchy(name, 2, () => { tube.GeneratePropertyPanel(); });

        tube.AddProperty(new StringFixLenProperty("Name", name, 16, "", (e) =>
        {
            hierarchyBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = e.value.ToString();
        }));
        index += 16;

        // ### Position ###
        tube.AddProperty(new PositionProperty("Position", LoadBytes<Vector3,Vector3Loader>(bytes, ref index), tube.transform, "The position of the bottom-center of the tube."));

        // ### Height ###
        tube.AddProperty(new FloatProperty("Height", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "", (e) =>
        {
            tube.UpdateHeight(e.value.Convert<float>());
        }));

        // ### Radius ###
        tube.AddProperty(new FloatProperty("Radius", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "", (e) =>
        {
            tube.UpdateRadius(e.value.Convert<float>());
        }));

        if (version >= 2)
        {
            // ### Magnetic ###
            tube.AddProperty(new BoolProperty("Magnetic", bytes[index] == 1, ""));
            index++;
        }

        if(version >= 3)
        {
            // ### Special Object ###
            //may later change this to something like "ReferenceProperty"
            tube.AddProperty(new StringProperty("Special Object", LoadBytes<string, String8Loader>(bytes, ref index), StringProperty.MaxSize.Byte, "Unknown exactly how the special object involves the tube."));
        }

        _value = tube;
    }
}
