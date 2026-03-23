using System.Collections.Generic;
using UnityEngine;

public class LeverSectionLoader : GizmoSectionLoader
{
    public override string Name => "Lever Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "Lever")) return;

        var section = TTObjectManager.Create<LeverSection>(Name, 1);

        // ### Version ###
        int version = LoadBytes<int, IntLoader>(bytes, ref index);
        section.AddProperty(new IntegerProperty("Version", version, IntegerProperty.IntType.Int) { generateOptions = TTProperty.FieldGenerateOptions.ReadonlyWName });

        // ### Lever Count ###
        int count = LoadBytes<int, IntLoader>(bytes, ref index);
        section.AddProperty(new IntegerProperty("Lever Count", count, IntegerProperty.IntType.Int) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });

        // ### Levers ###
        var levers = ChildrenProperty.LoadChildArray<Lever>(new LeverLoader(version), bytes, ref index, count, "Lever");
        section.AddProperty(new ChildrenProperty("Levers", levers) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });

        _value = section;
    }
}
