using System.Collections.Generic;
using UnityEngine;

public class LeverSectionLoader : GizmoSectionLoader
{
    public override string Name => "Lever Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "Lever")) return;

        var section = TTObjectManager.Create<LeverSection>(Name);

        // ### Version ###
        int version = LoadBytes<int, IntLoader>(bytes, ref index);
        section.AddProperty(new IntegerProperty("Version", GetTargetVersion(version), IntegerProperty.IntType.Int) { generateOptions = TTProperty.FieldGenerateOptions.ReadonlyWName });

        // ### Lever Count ###
        int count = LoadBytes<int, IntLoader>(bytes, ref index);
        IntegerProperty countProp = new("Lever Count", count, IntegerProperty.IntType.Int) { generateOptions = TTProperty.FieldGenerateOptions.Hidden };
        section.AddProperty(countProp);

        // ### Levers ###
        var childrenProp = ChildrenProperty.Create<Lever>("Levers", "", "Lever", new LeverLoader(version), new byte[54], bytes, ref index, count, (e) =>
        {
            countProp.Value = (e.value as ChildProperty[]).Length;
        }, TTProperty.FieldGenerateOptions.HiddenWName);
        section.AddProperty(childrenProp);

        // # Lever Menu Options #
        section.AddProperty(new EditOptionProperty("Gizmos/Create/New Lever", () =>
        {
            (childrenProp.AddNewChild().Value as TTObject).GeneratePropertyPanel();
        }));

        _value = section;
    }
}
