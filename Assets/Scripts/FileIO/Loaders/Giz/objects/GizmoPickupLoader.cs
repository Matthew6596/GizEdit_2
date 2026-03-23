using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class GizmoPickupLoader : PropertyLoader
{
    private int version;
    public override string Name => "GizmoPickup";

    public GizmoPickupLoader(int version)
    {
        this.version = version;
    }

    public override void Load(byte[] bytes, ref int index)
    {
        GizmoPickup pickup = TTObjectManager.Create<GizmoPickup>(Name);

        // ### Name ###
        string name = Encoding.UTF8.GetString(bytes, index, 8);

        var hierarchyBtn = EditorUIManager.Instance.AddObjectToHierarchy(EditorUIManager.GetStr(name,"unnamed_pickup"), 2, () => { pickup.GeneratePropertyPanel(); });

        pickup.AddProperty(new StringFixLenProperty("Name", name, 8, "This is the name of the pickup, which can be used to reference this pickup in the .git file.", (e) =>
        {
            //update labels
            string newName = e.value.ToString();
            hierarchyBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = EditorUIManager.GetStr(newName, "unnamed_pickup");
        }));
        index += 8;

        // ### Position ###
        pickup.AddProperty(new PositionProperty("Position", LoadBytes<Vector3,Vector3Loader>(bytes, ref index), pickup.transform, ""));

        // ### (Pickup) Type ###
        pickup.AddProperty(new EnumProperty("Type", GizmoPickup.GetPickupType((char)bytes[index]), GizmoPickup.PickupTypes, "This determines what type this pickup is. Note that 'Torpedo' may not function as expected.", (e) =>
        {
            //Update visually
            pickup.UpdateRender((int)e.value);
        }));
        index++;

        if(version >= 2)
        {
            // ### Spawn Type ###
            pickup.AddProperty(new EnumProperty("Spawn Type", (bytes[index]) switch { 2 => 1, 6 => 2, _ => 0 }, new string[] { "None", "Triggered", "Auto-Collect" }, "This is how the pickup loads into the level. If 'None' it acts as normal. If 'Triggered' it will only spawn after being triggered in the .git file. If 'Auto-Collect' it will be collected automatically when it spawns (more info needed)."));
            index++;
        }

        if(version >= 3)
        {
            // ### Spawn Group ###
            pickup.AddProperty(new IntegerProperty("Spawn Group", bytes[index], IntegerProperty.IntType.Byte, "This allows multiple pickups to spawn at the same time. If 0 it acts as normal. Otherwise, it will not appear and only spawn in once it or a pickup of the same spawn group is spawned by trigger."));
            index++;
        }

        _value = pickup;
    }
}
