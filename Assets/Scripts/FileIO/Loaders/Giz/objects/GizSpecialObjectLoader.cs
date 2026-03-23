using System;
using System.Collections.Generic;
using UnityEngine;

public class GizSpecialObjectLoader : PropertyLoader
{
    public override string Name => "Special Object (Gizmo)";

    private Func<TTObject,int,int> innerLoad;
    private int version;

    public GizSpecialObjectLoader(int version, Func<TTObject, int, int> innerLoad =null)
    {
        this.innerLoad = innerLoad ?? ((o,ind) => { return ind; });
        this.version = version;
    }

    public override void Load(byte[] bytes, ref int index)
    {
        GizSpecialObject specialObj = TTObjectManager.Create<GizSpecialObject>(Name);

        // ### Name ###
        string name = LoadBytes<string,String8Loader>(bytes, ref index);
        specialObj.AddProperty(new StringProperty("Name", name, StringProperty.MaxSize.Byte, "", (e) =>
        {
            //Update labels?
        }));

        // ### Unknown 1 ###
        specialObj.AddProperty(new FloatProperty("Unknown 1", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float));

        // ### Animation Time ###
        specialObj.AddProperty(new FloatProperty("Animation Time", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "The amount of time it takes for the special object's animation/movement to complete."));

        
        if (version >= 2)
        {
            // ### Unknown 2 ###
            specialObj.AddProperty(new IntegerProperty("Unknown 2", LoadBytes<int, IntLoader>(bytes, ref index), IntegerProperty.IntType.Int));
        }

        // ### Additional Loading ###
        int newInd = innerLoad.Invoke(specialObj, index);
        index = newInd;

        _value = specialObj;
    }
}
