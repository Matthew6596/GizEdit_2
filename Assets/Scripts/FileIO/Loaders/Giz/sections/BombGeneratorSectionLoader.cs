using UnityEngine;

public class BombGeneratorSectionLoader : GizmoSectionLoader
{
    public override string Name => "BombGenerator Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "BombGenerator")) return;

        BombGeneratorSection section = TTObjectManager.Create<BombGeneratorSection>(Name);

        // ### Version ###
        int version = bytes[index];
        index++;
        section.AddProperty(new IntegerProperty("Version", GetTargetVersion(version), IntegerProperty.IntType.Byte, "This is always 1 in Vanilla TCS.") { generateOptions = TTProperty.FieldGenerateOptions.ReadonlyWName });

        // ### Bomb Generator Count ###
        int count = LoadBytes<short, ShortLoader>(bytes, ref index);
        IntegerProperty countProp = new("Bomb Generator Count", count, IntegerProperty.IntType.Short) { generateOptions = TTProperty.FieldGenerateOptions.Hidden };
        section.AddProperty(countProp);

        // ### Bomb Generators ###
        ChildrenProperty childrenProp = ChildrenProperty.Create<BombGenerator>("Bomb Generators", "", "Bomb Generator", new BombGeneratorLoader(version), new byte[22], bytes, ref index, count, (e) =>
        {
            countProp.Value = (e.value as ChildProperty[]).Length;
        }, TTProperty.FieldGenerateOptions.HiddenWName);
        section.AddProperty(childrenProp);

        // # Bomb Generator Menu Options #
        section.AddProperty(new EditOptionProperty("Gizmos/Create/New Bomb Generator", () =>
        {
            (childrenProp.AddNewChild().Value as TTObject).GeneratePropertyPanel();
        }));

        _value = section;
    }
}
