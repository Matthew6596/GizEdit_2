
public class GizmoPickupSectionLoader : GizmoSectionLoader
{
    public override string Name => "GizmoPickup Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "GizmoPickup")) return;

        var pickupSection = TTObjectManager.Create<GizmoPickupSection>(Name);

        // ### Version ###
        int version = LoadBytes<int, IntLoader>(bytes, ref index);
        pickupSection.AddProperty(new IntegerProperty("Version", GetTargetVersion(version), IntegerProperty.IntType.Int, "The version/format of the section.") { generateOptions = TTProperty.FieldGenerateOptions.ReadonlyWName });

        // ### Pickup Count ###
        int pickupCount = LoadBytes<int, IntLoader>(bytes, ref index);
        IntegerProperty countProp = new("Pickup Count", pickupCount, IntegerProperty.IntType.Int, "") { generateOptions = TTProperty.FieldGenerateOptions.Hidden };
        pickupSection.AddProperty(countProp);

        // ### Unknown 1 ###
        int unknown1 = 3;
        if (version >= 3) unknown1 = LoadBytes<int, IntLoader>(bytes, ref index);    
        if (ShouldAddProperty(version, v => v >= 3))
        {
            // ## Unknown 1 ##
            pickupSection.AddProperty(new IntegerProperty("Unknown 1", unknown1, IntegerProperty.IntType.Int, "Unknown, but typically '0' in cutscene levels, '1' in episode 1-3 levels, and '3' in episode 4-6 levels. Maybe pickup version?"));
        }

        // ### Draw Distance ###
        // ### Scale ###
        float drawDist = 16, scale = 1;
        if (version >= 5)
        {
            drawDist = LoadBytes<float, FloatLoader>(bytes, ref index);
            scale = LoadBytes<float, FloatLoader>(bytes, ref index);
        }
        if(ShouldAddProperty(version, v => v >= 5))
        {
            // ## Draw Distance ##
            pickupSection.AddProperty(new FloatProperty("Draw Distance", drawDist, FloatProperty.FloatType.Float, "The distance for the camera to be within in order for a given pickup to be loaded."));

            // ## Scale ##
            TTObjectManager.LowerPropertyInitializationPriority(); //this needs to load after the pickups
            pickupSection.AddProperty(new FloatProperty("Scale", scale, FloatProperty.FloatType.Float, "The scale of all pickups. NOTE: This applies to all areas in the chapter, so make sure this value is the same for all areas for desired results.", (e) =>
            {
                GizmoPickupSection.pickupScale = e.value.Convert<float>();
                foreach (var pup in pickupSection.GetPickups()) pup.RefreshRenderScale();
            }));
            TTObjectManager.IncreasePropertyInitializationPriority();
        }

        // ### Pickups ###
        var childrenProp = ChildrenProperty.Create<GizmoPickup>("Pickups", "", "Pickup", new GizmoPickupLoader(version), new byte[23], bytes, ref index, pickupCount, (e) =>
        {
            countProp.Value = (e.value as ChildProperty[]).Length;
        }, TTProperty.FieldGenerateOptions.HiddenWName);
        pickupSection.AddProperty(childrenProp);

        // # Pickup Menu Options #
        EditorUIManager.Instance.AddMenuOption("Gizmos/Create/New Pickup", () =>
        {
            (childrenProp.AddNewChild().Value as TTObject).GeneratePropertyPanel();
        });

        _value = pickupSection;
    }
}
