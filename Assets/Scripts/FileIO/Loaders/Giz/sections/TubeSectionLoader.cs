using UnityEngine;
using System.Collections.Generic;

public class TubeSectionLoader : GizmoSectionLoader
{
    public override string Name => "Tube Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "Tube")) return;

        TubeSection section = TTObjectManager.Create<TubeSection>(Name, 1);

        // ### Version ###
        int version = LoadBytes<int, IntLoader>(bytes, ref index);
        section.AddProperty(new IntegerProperty("Version", version, IntegerProperty.IntType.Int) { generateOptions = TTProperty.FieldGenerateOptions.ReadonlyWName });

        // ### Tube Count ###
        int count = LoadBytes<int, IntLoader>(bytes, ref index);
        section.AddProperty(new IntegerProperty("Tube Count", count, IntegerProperty.IntType.Int) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });

        // ### Tubes ###
        var children = ChildrenProperty.LoadChildArray<Tube>(new TubeLoader(version), bytes, ref index, count, "Tube");
        section.AddProperty(new ChildrenProperty("Tubes", children) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });

        _value = section;
    }
}
