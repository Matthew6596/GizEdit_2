using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GizmoPickupSectionLoader : GizmoSectionLoader
{
    public override string Name => "GizmoPickup Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "GizmoPickup")) return;

        var pickupSection = TTObjectManager.Create<GizmoPickupSection>(Name, 1);

        // ### Version ###
        int version = LoadBytes<int, IntLoader>(bytes, ref index);
        pickupSection.AddProperty(new IntegerProperty("Version", version, IntegerProperty.IntType.Int, "The version/format of the section.") { generateOptions = TTProperty.FieldGenerateOptions.Readonly|TTProperty.FieldGenerateOptions.ShowName});

        // ### Pickup Count ###
        int pickupCount = LoadBytes<int, IntLoader>(bytes, ref index);
        pickupSection.AddProperty(new IntegerProperty("Pickup Count", pickupCount, IntegerProperty.IntType.Int, "") { generateOptions = TTProperty.FieldGenerateOptions.Hidden });

        if (version >= 3)
        {
            // ### Unknown 1 ###
            int unknown1 = LoadBytes<int, IntLoader>(bytes, ref index);

            pickupSection.AddProperty(new IntegerProperty("Unknown 1", unknown1, IntegerProperty.IntType.Int, "Unknown, but typically '0' in cutscene levels, '1' in episode 1-3 levels, and '3' in episode 4-6 levels. Maybe pickup version?"));
        }

        if (version >= 5)
        {
            // ### Draw Distance ###
            pickupSection.AddProperty(new FloatProperty("Draw Distance", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "The distance for the camera to be within in order for a given pickup to be loaded."));

            // ### Scale ###
            TTObjectManager.LowerPropertyInitializationPriority(); //this needs to load after the pickups
            pickupSection.AddProperty(new FloatProperty("Scale", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "The scale of all pickups. NOTE: This applies to all areas in the chapter, so make sure this value is the same for all areas for desired results.", (e) =>
            {
                GizmoPickupSection.pickupScale = e.value.Convert<float>();
                foreach (var pup in pickupSection.GetPickups()) pup.RefreshRenderScale();
            }));
            TTObjectManager.IncreasePropertyInitializationPriority();
        }

        // ### Pickups ###
        var children = ChildrenProperty.LoadChildArray<GizmoPickup>(new GizmoPickupLoader(version), bytes, ref index, pickupCount, "Pickup");
        pickupSection.AddProperty(new ChildrenProperty("Pickups", children, "All of the pickups in this area.") { generateOptions = TTProperty.FieldGenerateOptions.Hidden });

        _value = pickupSection;
    }
}
