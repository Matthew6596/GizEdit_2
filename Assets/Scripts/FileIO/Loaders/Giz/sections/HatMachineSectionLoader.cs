using UnityEngine;

public class HatMachineSectionLoader : GizmoSectionLoader
{
    public override string Name => "HatMachine Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "HatMachine")) return;

        HatMachineSection section = TTObjectManager.Create<HatMachineSection>(Name);

        // ### Version ###
        int version = LoadBytes<int, IntLoader>(bytes, ref index);
        section.AddProperty(new IntegerProperty("Version", GetTargetVersion(version), IntegerProperty.IntType.Int, "This is always 5 in Vanilla TCS.") { generateOptions = TTProperty.FieldGenerateOptions.ReadonlyWName });

        // ### Hat Machine Count ###
        int count = LoadBytes<int, IntLoader>(bytes, ref index);
        IntegerProperty countProp = new("Hat Machine Count", count, IntegerProperty.IntType.Int) { generateOptions = TTProperty.FieldGenerateOptions.Hidden };
        section.AddProperty(countProp);

        // ### Hat Machines ###
        var childrenProp = ChildrenProperty.Create<HatMachine>("Hat Machines", "", "Hat Machine", new HatMachineLoader(version), new byte[37], bytes, ref index, count, (e) =>
        {
            countProp.Value = (e.value as ChildProperty[]).Length;
        }, TTProperty.FieldGenerateOptions.HiddenWName);
        section.AddProperty(childrenProp);

        // # Hat Machine Menu Options #
        section.AddProperty(new EditOptionProperty("Gizmos/Create/New Hat Machine", () =>
        {
            (childrenProp.AddNewChild().Value as TTObject).GeneratePropertyPanel();
        }));

        _value = section;
    }
}
