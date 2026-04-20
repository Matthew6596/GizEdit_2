using UnityEngine;

public class GizTurretSectionLoader : GizmoSectionLoader
{
    public override string Name => "GizTurret Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "GizTurret")) return;

        GizTurretSection section = TTObjectManager.Create<GizTurretSection>(Name);

        // ### Version ###
        byte version = bytes[index];
        index++;
        section.AddProperty(new IntegerProperty("Version", GetTargetVersion(version), IntegerProperty.IntType.Byte) { generateOptions = TTProperty.FieldGenerateOptions.ReadonlyWName });

        // ### Turret Count ###
        short count = LoadBytes<short, ShortLoader>(bytes, ref index);
        IntegerProperty countProp = new("Turret Count", count, IntegerProperty.IntType.Short) { generateOptions = TTProperty.FieldGenerateOptions.Hidden };
        section.AddProperty(countProp);

        // ### Turrets ###
        byte[] defaultBytes = new byte[152];
        defaultBytes[16] = 3;
        var childrenProp = ChildrenProperty.Create<GizTurret>("Turrets", "", "Turret", new GizTurretLoader(version), defaultBytes, bytes, ref index, count, (e) =>
        {
            countProp.Value = (e.value as ChildProperty[]).Length;
        }, TTProperty.FieldGenerateOptions.HiddenWName);
        section.AddProperty(childrenProp);

        // # Force Menu Options #
        section.AddProperty(new EditOptionProperty("Gizmos/Create/New Turret", () =>
        {
            (childrenProp.AddNewChild().Value as TTObject).GeneratePropertyPanel();
        }));

        _value = section;
    }
}
