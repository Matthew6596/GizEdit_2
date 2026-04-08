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

        specialObjs.AddProperty(new IntegerProperty("Version", GetTargetVersion(version), IntegerProperty.IntType.Byte) { generateOptions = TTProperty.FieldGenerateOptions.ReadonlyWName });

        // ### Special Object Count ###
        byte count = bytes[index];
        index++;
        IntegerProperty countProp = new("Special Object Count", count, IntegerProperty.IntType.Byte) { generateOptions = TTProperty.FieldGenerateOptions.Hidden };
        specialObjs.AddProperty(countProp);

        if (TTLoader.LogEnabled) Debug.Log($"Loading {count} v{version} special objects at {index}");
        // ### Special Objects ###
        var childrenProp = ChildrenProperty.Create<GizSpecialObject>("Special Objects", "", "Special Object", new GizSpecialObjectLoader(version,innerLoad), new byte[17], bytes, ref index, count, (e) =>
        {
            countProp.Value = (e.value as ChildProperty[]).Length;
        });
        specialObjs.AddProperty(childrenProp);

        _value = specialObjs;
    }

    public GizSpecialObjects LoadDefault()
    {
        int tempInd = 0;
        Load(new byte[] { 3, 0, 0, 0, 0, 0 }, ref tempInd);
        return GetValue<GizSpecialObjects>();
    }
}
