using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class GizForceLoader : PropertyLoader
{
    public override string Name => "GizForce";

    private int version;

    public GizForceLoader(int version)
    {
        this.version = version;
    }

    public override void Load(byte[] bytes, ref int index)
    {
        GizForce force = TTObjectManager.Create<GizForce>(Name);

        // ### Name ###
        string name = Encoding.UTF8.GetString(bytes, index, 16);

        var hierarchyBtn = EditorUIManager.Instance.AddObjectToHierarchy(EditorUIManager.GetStr(name, "unnamed_gizforce"), 2, () => { force.GeneratePropertyPanel(); });

        force.AddProperty(new StringFixLenProperty("Name", name, 16, "", (e) =>
        {
            //update labels
            string newName = e.value.ToString();
            hierarchyBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = EditorUIManager.GetStr(name, "unnamed_gizforce");
        }));
        index += 16;

        // ### Position ###
        PositionProperty posProp = new("Position", LoadBytes<Vector3, Vector3Loader>(bytes, ref index), force.transform, "");
        force.AddProperty(posProp);

        if (version == 1)
        {
            // ### Unknown 1 ###
            force.AddProperty(new Vector3Property("Unknown 1", LoadBytes<Vector3, Vector3Loader>(bytes, ref index)));
        }

        // ### Reset Time ###
        force.AddProperty(new FloatProperty("Reset Time", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "The amount of time for the GizForce to reset."));

        if (version >= 8)
        {
            // ### Shake Time ###
            force.AddProperty(new FloatProperty("Shake Time", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "How long the GizForce will shake for before activating."));
        }

        // ### Range ###
        force.AddProperty(new FloatProperty("Range", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, ""));

        if (version == 1)
        {
            // ### Unknown 2 ###
            force.AddProperty(new Vector3Property("Unknown 2", LoadBytes<Vector3, Vector3Loader>(bytes, ref index)));

            // ### Unknown 3 ###
            force.AddProperty(new IntegerProperty("Unknown 3", LoadBytes<short,ShortLoader>(bytes, ref index), IntegerProperty.IntType.Short));
        }

        // ### Interaction Options? ###
        force.AddProperty(new IntegerProperty("Interaction Options (?)", LoadBytes<int, IntLoader>(bytes, ref index), IntegerProperty.IntType.Int, "Options for interaction such as Dark Side and whether reset occurs. (Needs further research)."));

        // ### Togglable ###
        force.AddProperty(new BoolProperty("Togglable", bytes[index] != 0, "..."));
        index++;

        if (version >= 11)
        {
            // ### Unknown 4 ###
            force.AddProperty(new IntegerProperty("Unknown 4", bytes[index], IntegerProperty.IntType.Byte, "..."));
            index++;
        }

        // ### Unknown 5 ###
        force.AddProperty(new IntegerProperty("Unknown 5", bytes[index], IntegerProperty.IntType.Byte, "..."));
        index++;

        if (version == 1)
        {
            // ### Unknown 6 ###
            force.AddProperty(new IntegerProperty("Unknown 6", bytes[index], IntegerProperty.IntType.Byte, "..."));
            index++;
        }

        // ### Special Objects ###
        GizSpecialObjectsLoader objsLoader = new((o,ind) =>
        {
            if (version >= 9)
            {
                // ### Unknown 7 ###
                o.AddProperty(new IntegerProperty("Unknown 7", BitConverter.ToInt16(bytes, ind), IntegerProperty.IntType.Short));
                return ind + 2;
            }
            return ind;
        });
        objsLoader.Load(bytes, ref index);
        var specialObjects = objsLoader.GetValue<GizSpecialObjects>();
        force.AddProperty(new ChildProperty("Special Objects", specialObjects, "") { generateOptions = TTProperty.FieldGenerateOptions.Hidden });

        // # Special Objects Editor Nav Buttons #
        //EditorUIManager.Instance.AddObjectToHierarchy("Special Objects", 3, () => { specialObjects.GeneratePropertyPanel(); });
        force.AddProperty(new NavBtnProperty("Special Objects", () => { specialObjects.GeneratePropertyPanel(); }));

        // ### Force Speed ###
        force.AddProperty(new FloatProperty("Force Speed", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, ""));

        // ### Reset Speed ###
        force.AddProperty(new FloatProperty("Reset Speed", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, ""));

        if (version >= 6)
        {
            // ### Auto Force? ###
            force.AddProperty(new FloatProperty("Auto Force (?)", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "..."));
        }

        if (version >= 7)
        {
            // ### Effect Scale ###
            force.AddProperty(new FloatProperty("Effect Scale", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "The scale of the force effect/aura."));
        }

        if (version >= 3)
        {
            // ### Unknown 8 ###
            force.AddProperty(new FloatProperty("Unknown 8", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "Related to animation?"));
        }

        if (version == 4)
        {
            // ### Unknown 9 ###
            force.AddProperty(new IntegerProperty("Unknown 9", LoadBytes<short, ShortLoader>(bytes, ref index), IntegerProperty.IntType.Short, "..."));
        }

        if (version >= 5)
        {
            // ### Linked blowup ###
            force.AddProperty(new StringProperty("Blowup", LoadBytes<string,String8Loader>(bytes, ref index), StringProperty.MaxSize.Byte, "The blowup linked to this GizForce."));
        }

        if (version >= 4)
        {
            // ### Minimum Studs Value ###
            force.AddProperty(new IntegerProperty("Minimum Studs Value", (ushort)LoadBytes<short, ShortLoader>(bytes, ref index), IntegerProperty.IntType.UShort, "..."));

            // ### Maximum Studs Value ###
            force.AddProperty(new IntegerProperty("Maximum Studs Value", (ushort)LoadBytes<short, ShortLoader>(bytes, ref index), IntegerProperty.IntType.UShort, "..."));

            // ### Studs Angle ###
            GameObject studsSpawnObjTEMP = new("studs_spawn_obj_TEMP");
            studsSpawnObjTEMP.transform.SetParent(force.transform);
            studsSpawnObjTEMP.transform.localPosition = Vector3.zero;
            force.AddProperty(new AngleProperty("Studs Angle", (ushort)LoadBytes<short, ShortLoader>(bytes, ref index), studsSpawnObjTEMP.transform, "The angle at which the studs emit."));

            // ### Studs Position ###
            force.AddProperty(new PositionProperty("Studs Position", LoadBytes<Vector3, Vector3Loader>(bytes, ref index), studsSpawnObjTEMP.transform, "The relative position at which the studs emit.") { isLowPriorityPosGiz = true, primaryPosProperty = posProp });
        }

        if (version >= 10)
        {
            // ### Studs Speed ###
            force.AddProperty(new FloatProperty("Studs Speed", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "The speed of the studs as they emit."));
        }

        if(version >= 15)
        {
            // ### Process Sound ###
            force.AddProperty(new StringProperty("Process Sound", LoadBytes<string, String8Loader>(bytes, ref index), StringProperty.MaxSize.Byte, "The sound played as the force is being activated/forced."));

            // ### Complete Sound ###
            force.AddProperty(new StringProperty("Complete Sound", LoadBytes<string, String8Loader>(bytes, ref index), StringProperty.MaxSize.Byte, "The sound played when the force is completed/activated."));

            // ### Reset Sound ###
            force.AddProperty(new StringProperty("Reset Sound", LoadBytes<string, String8Loader>(bytes, ref index), StringProperty.MaxSize.Byte, "The sound played as the force is resetting."));
        }

        _value = force;
    }
}
