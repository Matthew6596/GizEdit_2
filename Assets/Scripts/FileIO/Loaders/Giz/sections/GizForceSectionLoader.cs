using UnityEngine;

public class GizForceSectionLoader : GizmoSectionLoader
{
    public override string Name => "GizForce Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "GizForce")) return;

        GizForceSection section = TTObjectManager.Create<GizForceSection>(Name);

        // ### Version ###
        byte version = bytes[index];
        index++;

        section.AddProperty(new IntegerProperty("Version", GetTargetVersion(version), IntegerProperty.IntType.Byte) { generateOptions = TTProperty.FieldGenerateOptions.ReadonlyWName });

        // ### Force Count ###
        short count = LoadBytes<short,ShortLoader>(bytes, ref index);
        IntegerProperty countProp = new("Force Count", count, IntegerProperty.IntType.Short) { generateOptions = TTProperty.FieldGenerateOptions.Hidden };
        section.AddProperty(countProp);

        // ### Forces ###
        byte[] defaultBytes = new byte[124];
        int specVersInd = 42;
        if (version == 1) specVersInd += 27;
        if (version >= 8) specVersInd += 4;
        if (version >= 11) specVersInd += 1;
        defaultBytes[specVersInd] = 3;
        var childrenProp = ChildrenProperty.Create<GizForce>("Forces", "", "Force", new GizForceLoader(version), defaultBytes, bytes, ref index, count, (e) =>
        {
            countProp.Value = (e.value as ChildProperty[]).Length;
        }, TTProperty.FieldGenerateOptions.HiddenWName);
        section.AddProperty(childrenProp);

        // # Force Menu Options #
        section.AddProperty(new EditOptionProperty("Gizmos/Create/New Force", () =>
        {
            (childrenProp.AddNewChild().Value as TTObject).GeneratePropertyPanel();
        }));

        _value = section;
    }
}
