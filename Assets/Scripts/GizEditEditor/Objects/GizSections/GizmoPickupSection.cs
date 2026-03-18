using System.Collections.Generic;
using UnityEngine;

public class GizmoPickupSection : GizmoSection
{
    public static float pickupScale = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GizmoPickup[] GetPickups()
    {
        var pups = FindProperty<ChildrenProperty>("Pickups");
        if(pups == null) return new GizmoPickup[0];
        return pups.GetChildrenValues<GizmoPickup>();
    }
}
