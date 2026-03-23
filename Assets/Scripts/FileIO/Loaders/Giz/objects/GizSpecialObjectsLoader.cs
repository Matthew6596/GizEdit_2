using System;
using System.Collections.Generic;
using UnityEngine;

public class GizSpecialObjectsLoader : PropertyLoader
{
    public override string Name => "Special Objects (Gizmo)";

    private Func<TTObject, int, int> innerLoad;

    public GizSpecialObjectsLoader(Func<TTObject, int, int> innerLoad =null)
    {
        this.innerLoad = innerLoad ?? ((o,ind) => { return ind; });
    }

    public override void Load(byte[] bytes, ref int index)
    {
        GizSpecialObjects specialObjs = TTObjectManager.Create<GizSpecialObjects>(Name);

        // ### Version ###
        byte version = bytes[index];
        index++;

        specialObjs.AddProperty(new IntegerProperty("Version", version, IntegerProperty.IntType.Byte) { generateOptions = TTProperty.FieldGenerateOptions.ReadonlyWName });

        // ### Special Object Count ###
        byte count = bytes[index];
        index++;

        specialObjs.AddProperty(new IntegerProperty("Special Object Count", count, IntegerProperty.IntType.Byte) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });

        // ### Special Objects ###
        var children = ChildrenProperty.LoadChildArray<GizSpecialObject>(new GizSpecialObjectLoader(version, innerLoad), bytes, ref index, count, "Special Object");
        specialObjs.AddProperty(new ChildrenProperty("Special Objects", children));

        _value = specialObjs;
    }
}
