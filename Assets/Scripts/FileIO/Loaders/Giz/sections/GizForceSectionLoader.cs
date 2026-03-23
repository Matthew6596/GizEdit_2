using UnityEngine;

public class GizForceSectionLoader : GizmoSectionLoader
{
    public override string Name => "GizForce Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "GizForce")) return;

        GizForceSection section = TTObjectManager.Create<GizForceSection>(Name, 1);

        // ### Version ###
        byte version = bytes[index];
        index++;

        section.AddProperty(new IntegerProperty("Version", version, IntegerProperty.IntType.Byte) { generateOptions = TTProperty.FieldGenerateOptions.ReadonlyWName });

        // ### Force Count ###
        short count = LoadBytes<short,ShortLoader>(bytes, ref index);
        section.AddProperty(new IntegerProperty("Force Count", count, IntegerProperty.IntType.Short) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });

        // ### Forces ###
        var children = ChildrenProperty.LoadChildArray<GizForce>(new GizForceLoader(version), bytes, ref index, count, "GizForce");
        section.AddProperty(new ChildrenProperty("GizForces", children) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });

        _value = section;
    }
}
