using UnityEngine;

public class PushBlocksSectionLoader : GizmoSectionLoader
{
    public override string Name => "PushBlocks Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "PushBlocks")) return;

        PushBlocksSection section = TTObjectManager.Create<PushBlocksSection>(Name);

        // ### Version ###
        int version = LoadBytes<int, IntLoader>(bytes, ref index);
        section.AddProperty(new IntegerProperty("Version", GetTargetVersion(version), IntegerProperty.IntType.Int, "") { generateOptions = TTProperty.FieldGenerateOptions.ReadonlyWName });

        // ### PushBlocks Count ###
        int count = LoadBytes<int, IntLoader>(bytes, ref index);
        IntegerProperty countProp = new("PushBlocks Count", count, IntegerProperty.IntType.Int, "") { generateOptions = TTProperty.FieldGenerateOptions.Hidden };
        section.AddProperty(countProp);

        // ### Push Blocks ###
        ChildrenProperty childrenProp = ChildrenProperty.Create<PushBlocks>("Push Blocks", "", "Push Block", new PushBlocksLoader(version), new byte[14], bytes, ref index, count, (e) =>
        {
            countProp.Value = (e.value as ChildProperty[]).Length;
        }, TTProperty.FieldGenerateOptions.HiddenWName);
        section.AddProperty(childrenProp);

        // # PushBlocks Menu Options #
        section.AddProperty(new EditOptionProperty("Gizmos/Create/New PushBlocks", () =>
        {
            (childrenProp.AddNewChild().Value as TTObject).GeneratePropertyPanel();
        }));

        _value = section;
    }
}
