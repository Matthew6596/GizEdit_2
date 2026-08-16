using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GizObstacleLoader : PropertyLoader
{
    public override string Name => "GizObstacle";

    private int version;

    public GizObstacleLoader(int version)
    {
        this.version = version;
    }

    public override void Load(byte[] bytes, ref int index)
    {
        GizObstacle obstacle = TTObjectManager.Create<GizObstacle>(Name);

        // ### Name ###
        string name = Encoding.UTF8.GetString(bytes, index, 16);
        obstacle.AddProperty(new StringFixLenProperty("Name", name, 16, ""));
        index += 16;

        // ### Position ###
        PositionProperty posProp = new("Position", LoadBytes<Vector3, Vector3Loader>(bytes, ref index), obstacle.transform, "The position of the obstacle, or one corner of the bounds.");
        obstacle.AddProperty(posProp);

        // ### Bounds Corner ###
        Vector3 boundPoint = Vector3.zero;
        if (version >= 2) boundPoint = LoadBytes<Vector3, Vector3Loader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 2))
        {
            // ## Bounds Corner ##
            obstacle.AddProperty(new PositionProperty("Trigger Position", boundPoint, obstacle.BoundsCorner.transform,"The position of the trigger sphere.") { isSecondaryPosGiz = true, primaryPosProperty = posProp });
        }

        // ### Unknown 2 ###
        obstacle.AddProperty(new FloatProperty("Unknown 2", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, ""));

        // ### Unknown 3 ###
        obstacle.AddProperty(new FloatProperty("Trigger Radius", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, ""));

        // ### Unknown 4 ###
        // ### Unknown 5 ###
        Vector3 unk4 = Vector3.zero;
        short unk5 = 0;
        if (version >= 3)
        {
            unk4 = LoadBytes<Vector3, Vector3Loader>(bytes, ref index);
            unk5 = LoadBytes<short, ShortLoader>(bytes, ref index);
        }
        if (ShouldAddProperty(version, v => v >= 3))
        {
            // ## Unknown 4 ##
            obstacle.AddProperty(new Vector3Property("Unknown 4", unk4, ""));

            // ## Unknown 5 ##
            obstacle.AddProperty(new IntegerProperty("Unknown 5", unk5, IntegerProperty.IntType.Short, ""));
        }

        // ### Unknown 6 ###
        obstacle.AddProperty(new IntegerProperty("Unknown 6", LoadBytes<int, IntLoader>(bytes, ref index), IntegerProperty.IntType.Int, ""));

        // ### Unknown 7 ###
        int unk7 = 0;
        if (version >= 12) unk7 = LoadBytes<int, IntLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 12))
        {
            // ## Unknown 7 ##
            obstacle.AddProperty(new IntegerProperty("Unknown 7", unk7, IntegerProperty.IntType.Int, ""));
        }

        // ### Unknown 8 ###
        // ### Unknown 9 ###
        short unk8 = 0;
        byte unk9 = 0;
        if (version == 6)
        {
            unk8 = LoadBytes<short, ShortLoader>(bytes, ref index);
            unk9 = bytes[index];
            index++;
        }
        if (ShouldAddProperty(version, v => v == 6)) //Padding
        {
            // ## Unknown 8 ##
            obstacle.AddProperty(new IntegerProperty("Unknown 8", unk8, IntegerProperty.IntType.Short, "") { generateOptions = TTProperty.FieldGenerateOptions.Hidden });

            // ## Unknown 9 ##
            obstacle.AddProperty(new IntegerProperty("Unknown 9", unk9, IntegerProperty.IntType.Byte, "") { generateOptions = TTProperty.FieldGenerateOptions.Hidden });
        }

        // ### Unknown 10 ###
        obstacle.AddProperty(new EnumProperty("Anim Behaviour", bytes[index],
            new() { { "One Shot", 0 }, { "Auto Reverse", 1 }, { "Indefinite Loop", 2 }, { "Held Loop", 3 }, { "Instant Reverse", 4 }, },
            ""));
        //obstacle.AddProperty(new IntegerProperty("Unknown 10", bytes[index], IntegerProperty.IntType.Byte, ""));
        index++;

        // ### Unknown 11 ###
        obstacle.AddProperty(new EnumProperty("Trigger Type", bytes[index],
            new() { { "AutoStart", 0 }, { "Proximity1", 1 }, { "Proximity2", 2 }, { "NoTrigger", 3 }, { "TechnoOnly", 4 },
            { "Proximity3", 5 }, { "Proximity4", 6 }, { "PushOnly", 7 } },
            ""));
        //obstacle.AddProperty(new IntegerProperty("Unknown 11", bytes[index], IntegerProperty.IntType.Byte, ""));
        index++;

        // ### Unknown 12 ###
        int unk12 = 0;
        if (version >= 7)
        {
            unk12 = bytes[index];
            index++;
        }
        if (ShouldAddProperty(version, v => v >= 7))
        {
            // ## Unknown 12 ##
            obstacle.AddProperty(new IntegerProperty("Unknown 12", unk12, IntegerProperty.IntType.Byte, "") { generateOptions=TTProperty.FieldGenerateOptions.ReadonlyWName});
        }

        // ### Special Objects ###
        GizSpecialObjectsLoader objsLoader = new((o, ind) =>
        {
            // ### Unknown 13 ###
            short unk13 = 0;
            if (version >= 8)
            {
                unk13 = BitConverter.ToInt16(bytes, ind);
                ind += 2;
            }
            if (ShouldAddProperty(version, v => v >= 8))
            {
                // ## Unknown 13 ##
                o.AddProperty(new IntegerProperty("Unknown 13", unk13, IntegerProperty.IntType.Short));
            }
            return ind;
        });
        objsLoader.Load(bytes, ref index);
        var specialObjs = objsLoader.GetValue<GizSpecialObjects>();
        obstacle.AddProperty(new ChildProperty("Special Objects", specialObjs, "") { generateOptions = TTProperty.FieldGenerateOptions.Hidden });

        // # Special Objects Editor Nav Buttons #
        specialObjs.PrependProperty(new NavBtnProperty($"<-- Back to GizObstacle", () => { obstacle.GeneratePropertyPanel(); }));
        obstacle.AddProperty(new NavBtnProperty("Special Objects -->", () => { specialObjs.GeneratePropertyPanel(); }));

        // ### Unknown 14 ###
        float unk14 = 0;
        if (version >= 4) unk14 = LoadBytes<float, FloatLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 4)) 
        {
            // ## Unknown 14 ##
            obstacle.AddProperty(new FloatProperty("Anim Speed", unk14, FloatProperty.FloatType.Float, ""));
        }

        // ### Unknown 15 ###
        float unk15 = 0;
        if (version >= 5) unk15 = LoadBytes<float, FloatLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 5))
        {
            // ## Unknown 15 ##
            obstacle.AddProperty(new FloatProperty("Reverse Speed", unk15, FloatProperty.FloatType.Float, ""));
        }

        // ### Unknown 16 ###
        float unk16 = 0;
        if (version >= 8) unk16 = LoadBytes<float, FloatLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 8))
        {
            // ## Unknown 16 ##
            obstacle.AddProperty(new FloatProperty("Unknown 16", unk16, FloatProperty.FloatType.Float, ""));
        }

        // ### Unknown 17 ###
        short unk17 = 0;
        if (version == 9) unk17 = LoadBytes<short, ShortLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v == 9))
        {
            // ## Unknown 17 ##
            obstacle.AddProperty(new IntegerProperty("Blowup ID", unk17, IntegerProperty.IntType.Short, ""));
        }

        // ### Unknown 18 ###
        string unk18 = "";
        if (version >= 10) unk18 = LoadBytes<string, String8Loader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 10))
        {
            // ## Unknown 18 ##
            obstacle.AddProperty(new StringProperty("Blowup", unk18, StringProperty.MaxSize.Byte, ""));
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
            obstacle.AddProperty(new IntegerProperty("Studs Value", minStuds, IntegerProperty.IntType.UShort, "..."));

            // ## Maximum Studs Value ##
            obstacle.AddProperty(new IntegerProperty("Studs X Angle", maxStuds, IntegerProperty.IntType.UShort, "..."));

            // ## Studs Angle ##
            GameObject studsSpawnObjTEMP = new("studs_spawn_obj_TEMP");
            studsSpawnObjTEMP.transform.SetParent(obstacle.transform);
            studsSpawnObjTEMP.transform.localPosition = Vector3.zero;
            obstacle.AddProperty(new AngleProperty("Studs Y Angle", studAng, studsSpawnObjTEMP.transform, "The angle at which studs emit."));

            // ## Studs Position ##
            obstacle.AddProperty(new PositionProperty("Studs Position", studPos, studsSpawnObjTEMP.transform, "The relative position at which studs emit.") { isSecondaryPosGiz = true, primaryPosProperty = posProp });
        }

        // ### Studs Speed ###
        float studSpd = 1.75f;
        if (version >= 11) studSpd = LoadBytes<float, FloatLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 11))
        {
            // ## Studs Speed ##
            obstacle.AddProperty(new FloatProperty("Studs Speed", studSpd, FloatProperty.FloatType.Float, "The speed of the studs as they emit.", (e) => { }, 1.75f));
        }

        // ### Unknown 19 ###
        string unk19 = "";
        if (version >= 13) unk19 = LoadBytes<string, String8Loader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 13))
        {
            // ## Unknown 19 ##
            obstacle.AddProperty(new StringProperty("Start SFX", unk19, StringProperty.MaxSize.Byte, ""));
        }

        // ### Unknown 20 ###
        string unk20 = "";
        if (version >= 14) unk20 = LoadBytes<string, String8Loader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 14))
        {
            // ## Unknown 20 ##
            obstacle.AddProperty(new StringProperty("Reverse SFX", unk20, StringProperty.MaxSize.Byte, ""));
        }

        _value = obstacle;
    }
}
