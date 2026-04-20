using UnityEngine;

public class SpinnerSectionLoader : GizmoSectionLoader
{
    public override string Name => "Spinner Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "Spinner")) return;

        SpinnerSection section = TTObjectManager.Create<SpinnerSection>(Name);

        // ### Version ###
        int version = LoadBytes<int, IntLoader>(bytes, ref index);
        section.AddProperty(new IntegerProperty("Version", GetTargetVersion(version), IntegerProperty.IntType.Int, "") { generateOptions = TTProperty.FieldGenerateOptions.ReadonlyWName });

        // ### Spinner Count ###
        int count = LoadBytes<int, IntLoader>(bytes, ref index);
        IntegerProperty countProp = new("Spinner Count", count, IntegerProperty.IntType.Int, "") { generateOptions = TTProperty.FieldGenerateOptions.Hidden };
        section.AddProperty(countProp);

        // ### Spinners ###
        ChildrenProperty childrenProp = ChildrenProperty.Create<Spinner>("Spinners", "", "Spinner", new SpinnerLoader(version), new byte[41], bytes, ref index, count, (e) =>
        {
            countProp.Value = (e.value as ChildProperty[]).Length;
        }, TTProperty.FieldGenerateOptions.HiddenWName);
        section.AddProperty(childrenProp);

        // # Spinner Menu Options #
        section.AddProperty(new EditOptionProperty("Gizmos/Create/New Spinner", () =>
        {
            (childrenProp.AddNewChild().Value as TTObject).GeneratePropertyPanel();
        }));

        _value = section;
    }
}
