using UnityEngine;
using System.Collections.Generic;

public class TubeSectionLoader : GizmoSectionLoader
{
    public override string Name => "Tube Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "Tube")) return;

        TubeSection section = TTObjectManager.Create<TubeSection>(Name);

        // ### Version ###
        int version = LoadBytes<int, IntLoader>(bytes, ref index);
        section.AddProperty(new IntegerProperty("Version", GetTargetVersion(version), IntegerProperty.IntType.Int) { generateOptions = TTProperty.FieldGenerateOptions.ReadonlyWName });

        // ### Tube Count ###
        int count = LoadBytes<int, IntLoader>(bytes, ref index);
        IntegerProperty countProp = new("Tube Count", count, IntegerProperty.IntType.Int) { generateOptions = TTProperty.FieldGenerateOptions.Hidden };
        section.AddProperty(countProp);

        // ### Tubes ###
        var childrenProp = ChildrenProperty.Create<Tube>("Tubes", "", "Tube", new TubeLoader(version), new byte[38], bytes, ref index, count, (e) =>
        {
            countProp.Value = (e.value as ChildProperty[]).Length;
        }, TTProperty.FieldGenerateOptions.HiddenWName);
        section.AddProperty(childrenProp);

        // # Tube Menu Options #
        EditorUIManager.Instance.AddMenuOption("Gizmos/Create/New Tube", () => 
        {
            (childrenProp.AddNewChild().Value as TTObject).GeneratePropertyPanel();
        });

        _value = section;
    }
}
