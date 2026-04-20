using UnityEngine;

public class GizObstacleSectionLoader : GizmoSectionLoader
{
    public override string Name => "GizObstacle Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "GizObstacle")) return;

        GizObstacleSection section = TTObjectManager.Create<GizObstacleSection>(Name);

        // ### Version ###
        int version = bytes[index];
        index++;
        section.AddProperty(new IntegerProperty("Version", GetTargetVersion(version), IntegerProperty.IntType.Byte, "This is always at least 10 in Vanilla TCS.") { generateOptions = TTProperty.FieldGenerateOptions.ReadonlyWName });

        // ### Obstacle Count ###
        int count = LoadBytes<short, ShortLoader>(bytes, ref index);
        IntegerProperty countProp = new("Obstacle Count", count, IntegerProperty.IntType.Short, "") { generateOptions = TTProperty.FieldGenerateOptions.Hidden };
        section.AddProperty(countProp);

        // ### Obstacles ###
        ChildrenProperty childrenProp = ChildrenProperty.Create<GizObstacle>("Obstacles", "", "Obstacle", new GizObstacleLoader(version), new byte[105], bytes, ref index, count, (e) =>
        {
            countProp.Value = (e.value as ChildProperty[]).Length;
        }, TTProperty.FieldGenerateOptions.HiddenWName);
        section.AddProperty(childrenProp);

        // # GizObstacle Menu Options #
        section.AddProperty(new EditOptionProperty("Gizmos/Create/New GizObstacle", () =>
        {
            (childrenProp.AddNewChild().Value as TTObject).GeneratePropertyPanel();
        }));

        _value = section;
    }
}
