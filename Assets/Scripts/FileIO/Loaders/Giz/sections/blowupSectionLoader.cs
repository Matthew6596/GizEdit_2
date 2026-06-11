using UnityEngine;

public class blowupSectionLoader : GizmoSectionLoader
{
    public override string Name => "Blowup Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "blowup")) return;

        blowupSection section = TTObjectManager.Create<blowupSection>(Name);

        // ### Version ###
        int version = LoadBytes<int, IntLoader>(bytes, ref index);
        section.AddProperty(new IntegerProperty("Version", GetTargetVersion(version), IntegerProperty.IntType.Int, "This is always at least 21 in Vanilla TCS.") { generateOptions = TTProperty.FieldGenerateOptions.ReadonlyWName });

        // ### blowupObject Count ###
        int objCount = 0;
        IntegerProperty objCountProp = null;
        if (version >= 2)
        {
            objCount = LoadBytes<int, IntLoader>(bytes, ref index);

            // ## blowupObject Count ##
            objCountProp = new("blowupType Count", objCount, IntegerProperty.IntType.Int, "") { generateOptions = TTProperty.FieldGenerateOptions.Hidden };
        }
        if (ShouldAddProperty(version, v => v >= 2)) section.AddProperty(objCountProp);

        // ### blowup Count ###
        int count = LoadBytes<int, IntLoader>(bytes, ref index);
        IntegerProperty countProp = new("blowup Count", count, IntegerProperty.IntType.Int, "") { generateOptions = TTProperty.FieldGenerateOptions.Hidden };
        section.AddProperty(countProp);

        // ### blowupObjects ###
        ChildrenProperty objChildrenProp = null;
        if (version >= 2)
        {
            // ## blowupObjects ##
            objChildrenProp = ChildrenProperty.Create<blowupObject>("Blowup Types", "", "Blowup Type", new blowupObjectLoader(version), new byte[59], bytes, ref index, objCount, (e) =>
            {
                objCountProp.Value = (e.value as ChildProperty[]).Length;
            }, TTProperty.FieldGenerateOptions.HiddenWName);
        }
        if (ShouldAddProperty(version, v => v >= 2))
        {
            section.AddProperty(objChildrenProp);

            // # blowupObject Menu Options #
            section.AddProperty(new EditOptionProperty("Gizmos/Create/New Blowup Type", () =>
            {
                (objChildrenProp.AddNewChild().Value as TTObject).GeneratePropertyPanel();
            }));
        }

        // ### blowups ###
        ChildrenProperty childrenProp = ChildrenProperty.Create<blowup>("Blowups", "", "Blowup", new blowupLoader(version), new byte[126], bytes, ref index, count, (e) =>
        {
            countProp.Value = (e.value as ChildProperty[]).Length;
        }, TTProperty.FieldGenerateOptions.HiddenWName);
        section.AddProperty(childrenProp);

        // # blowup Menu Options #
        section.AddProperty(new EditOptionProperty("Gizmos/Create/New Blowup", () =>
        {
            (childrenProp.AddNewChild().Value as TTObject).GeneratePropertyPanel();
        }));

        _value = section;
    }
}
