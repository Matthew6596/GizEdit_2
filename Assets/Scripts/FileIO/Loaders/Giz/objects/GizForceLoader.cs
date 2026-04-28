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
        force.AddProperty(new StringFixLenProperty("Name", name, 16, ""));
        index += 16;

        // ### Position ###
        PositionProperty posProp = new("Position", LoadBytes<Vector3, Vector3Loader>(bytes, ref index), force.transform, "");
        force.AddProperty(posProp);

        // ### Unknown 1 ###
        Vector3 unk1 = Vector3.zero;
        if (version == 1) unk1 = LoadBytes<Vector3, Vector3Loader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v == 1))
        {
            // ## Unknown 1 ##
            force.AddProperty(new Vector3Property("Unknown 1", unk1));
        }

        // ### Return Time ###
        force.AddProperty(new FloatProperty("Return Time", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "The amount of time for the GizForce to return to its original position."));

        // ### Shake Time ###
        float shakeTime = 0;
        if (version >= 8) shakeTime = LoadBytes<float, FloatLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 8))
        {
            // ## Shake Time ##
            force.AddProperty(new FloatProperty("Shake Time", shakeTime, FloatProperty.FloatType.Float, "How long the GizForce will shake for before activating."));
        }

        // ### Range ###
        force.AddProperty(new FloatProperty("Range", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, ""));

        // ### Unknown 2 ###
        // ### Unknown 3 ###
        Vector3 unk2 = Vector3.zero;
        short unk3 = 0;
        if (version == 1)
        {
            unk2 = LoadBytes<Vector3, Vector3Loader>(bytes, ref index);
            unk3 = LoadBytes<short, ShortLoader>(bytes, ref index);
        }
        if (ShouldAddProperty(version, v => v == 1))
        {
            // ## Unknown 2 ##
            force.AddProperty(new Vector3Property("Unknown 2", unk2));

            // ## Unknown 3 ##
            force.AddProperty(new IntegerProperty("Unknown 3", unk3, IntegerProperty.IntType.Short));
        }

        // ### Force Behaviors ###
        string[] interactOptions = new string[32];
        for (int i = 0; i < 32; i++) interactOptions[i] = "unkbit";
        interactOptions[1] = "Returns|When checked, the force will not stop when complete.";
        interactOptions[2] = "Can Return Later";
        interactOptions[4] = "Dark Side";
        interactOptions[5] = "idk";
        interactOptions[6] = "Turn on light?";
        interactOptions[10] = "Cannot undo?";
        interactOptions[11] = "two player force?|Stack box? but also used for other forces.";
        force.AddProperty(new IntBitFlagsProperty("Force Behaviors", LoadBytes<int, IntLoader>(bytes, ref index), interactOptions, "Behaviors such as Dark Side or whether reset occurs."));

        // ### Togglable ###
        force.AddProperty(new BoolProperty("Togglable", bytes[index] != 0xff, "...") { trueValue=0,falseValue=0xff});
        index++;

        // ### Unknown 4 ###
        byte unk4 = 0;
        if (version >= 11)
        {
            unk4 = bytes[index];
            index++;
        }
        if (ShouldAddProperty(version, v => v >= 11))
        {
            // ## Unknown 4 ##
            force.AddProperty(new IntegerProperty("Unknown 4", unk4, IntegerProperty.IntType.Byte, "..."));
        }

        // ### Unknown 5 ###
        force.AddProperty(new IntegerProperty("Unknown 5", bytes[index], IntegerProperty.IntType.Byte, "..."));
        index++;

        // ### Unknown 6 ###
        byte unk6 = 0;
        if (version == 1)
        {
            unk6 = bytes[index];
            index++;
        }
        if (ShouldAddProperty(version, v => v == 1))
        {
            // ## Unknown 6 ##
            force.AddProperty(new IntegerProperty("Unknown 6", unk6, IntegerProperty.IntType.Byte, "..."));
        }

        if (TTLoader.LogEnabled) Debug.Log($"Loading GizForce special objects at {index}");
        // ### Special Objects ###
        GizSpecialObjectsLoader objsLoader = new((o,ind) =>
        {
            // ### Unknown 7 ###
            short unk7 = 0;
            if (version >= 9)
            {
                unk7 = BitConverter.ToInt16(bytes, ind);
                ind += 2;
            }
            if (ShouldAddProperty(version, v => v >= 9))
            {
                // ## Unknown 7 ##
                o.AddProperty(new IntegerProperty("Unknown 7", unk7, IntegerProperty.IntType.Short));
            }
            return ind;
        });
        objsLoader.Load(bytes, ref index);
        var specialObjects = objsLoader.GetValue<GizSpecialObjects>();
        force.AddProperty(new ChildProperty("Special Objects", specialObjects, "") { generateOptions = TTProperty.FieldGenerateOptions.Hidden });

        // # Special Objects Editor Nav Buttons #
        specialObjects.PrependProperty(new NavBtnProperty($"<-- Back to GizForce", () => { force.GeneratePropertyPanel(); }));
        force.AddProperty(new NavBtnProperty("Special Objects -->", () => { specialObjects.GeneratePropertyPanel(); }));

        if (TTLoader.LogEnabled) Debug.Log($"Finished loading GizForce special objects at {index}");

        // ### Force Speed ###
        force.AddProperty(new FloatProperty("Force Speed", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, ""));

        // ### Return Speed ###
        force.AddProperty(new FloatProperty("Return Speed", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, ""));

        // ### Auto Force? ###
        float autoForce = 0;
        if (version >= 6) autoForce = LoadBytes<float, FloatLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 6))
        {
            // ## Auto Force? ##
            force.AddProperty(new FloatProperty("Auto Force (?)", autoForce, FloatProperty.FloatType.Float, "..."));
        }

        // ### Effect Scale ###
        float effectScale = 0;
        if (version >= 7) effectScale = LoadBytes<float, FloatLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 7))
        {
            // ## Effect Scale ##
            force.AddProperty(new FloatProperty("Effect Scale", effectScale, FloatProperty.FloatType.Float, "The scale of the force effect/aura."));
        }

        // ### Unknown 8 ###
        float unk8 = 0;
        if (version >= 3) unk8 = LoadBytes<float, FloatLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 3))
        {
            // ## Unknown 8 ##
            force.AddProperty(new FloatProperty("Unknown 8", unk8, FloatProperty.FloatType.Float, "Related to animation?"));
        }

        // ### Unknown 9 ###
        short unk9 = 0;
        if (version == 4) unk9 = LoadBytes<short, ShortLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v == 4))
        {
            // ## Unknown 9 ##
            force.AddProperty(new IntegerProperty("Unknown 9", unk9, IntegerProperty.IntType.Short, "..."));
        }

        // ### Linked blowup ###
        string blowup = "";
        if (version >= 5) blowup = LoadBytes<string, String8Loader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 5))
        {
            // ## Linked blowup ##
            force.AddProperty(new StringProperty("Blowup", blowup, StringProperty.MaxSize.Byte, "The blowup linked to this GizForce."));
        }

        // ### Minimum Studs Value ###
        // ### Maximum Studs Value ###
        // ### Studs Angle ###
        // ### Studs Position ###
        ushort minStuds = 0, maxStuds = 0, studAng = 0;
        Vector3 studPos = Vector3.zero;
        if (version >= 4)
        {
            minStuds = (ushort)LoadBytes<short, ShortLoader>(bytes, ref index);
            maxStuds = (ushort)LoadBytes<short, ShortLoader>(bytes, ref index);
            studAng = (ushort)LoadBytes<short, ShortLoader>(bytes, ref index);
            studPos = LoadBytes<Vector3, Vector3Loader>(bytes, ref index);
        }
        if (ShouldAddProperty(version, v => v >= 4))
        {
            // ## Minimum Studs Value ##
            force.AddProperty(new IntegerProperty("Minimum Studs Value", minStuds, IntegerProperty.IntType.UShort, "..."));

            // ## Maximum Studs Value ##
            force.AddProperty(new IntegerProperty("Maximum Studs Value", maxStuds, IntegerProperty.IntType.UShort, "..."));

            // ## Studs Angle ##
            GameObject studsSpawnObjTEMP = new("studs_spawn_obj_TEMP");
            studsSpawnObjTEMP.transform.SetParent(force.transform);
            studsSpawnObjTEMP.transform.localPosition = Vector3.zero;
            force.AddProperty(new AngleProperty("Studs Angle", studAng, studsSpawnObjTEMP.transform, "The angle at which studs emit."));

            // ## Studs Position ##
            force.AddProperty(new PositionProperty("Studs Position", studPos, studsSpawnObjTEMP.transform, "The relative position at which studs emit.") { isSecondaryPosGiz = true, primaryPosProperty = posProp });
        }

        // ### Studs Speed ###
        float studSpd = 1.5f;
        if (version >= 10) studSpd = LoadBytes<float, FloatLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 10))
        {
            // ## Studs Speed ##
            force.AddProperty(new FloatProperty("Studs Speed", studSpd, FloatProperty.FloatType.Float, "The speed of the studs as they emit.", (e) => { }, 1.5f));
        }

        // ### Process Sound ###
        // ### Complete Sound ###
        // ### Return Sound ###
        string procSfx = "", doneSfx = "", resetSfx = "";
        if (version >= 15)
        {
            procSfx = LoadBytes<string, String8Loader>(bytes, ref index);
            doneSfx = LoadBytes<string, String8Loader>(bytes, ref index);
            resetSfx = LoadBytes<string, String8Loader>(bytes, ref index);
        }
        if (ShouldAddProperty(version, v => v >= 15))
        {
            // ## Process Sound ##
            force.AddProperty(new StringProperty("Process Sound", procSfx, StringProperty.MaxSize.Byte, "The sound played as the force is being processed (forced/used)."));

            // ## Complete Sound ##
            force.AddProperty(new StringProperty("Complete Sound", doneSfx, StringProperty.MaxSize.Byte, "The sound played when the force is completed."));

            // ## Return Sound ##
            force.AddProperty(new StringProperty("Return Sound", resetSfx, StringProperty.MaxSize.Byte, "The sound played as the force is returning."));
        }

        _value = force;
    }
}
