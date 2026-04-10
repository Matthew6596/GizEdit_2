using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class GizmoPickup : TTObject
{
    private GameObject renderObj;
    private Vector3 scaleFactor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static readonly Dictionary<string, byte> PickupTypes = new()
    {
        { "Silver Stud", (byte)'s' },
        { "Gold Stud", (byte)'g' },
        { "Blue Stud", (byte)'b' },
        { "Purple Stud", (byte)'p' },
        { "Minikit", (byte)'m' },
        { "Powerup", (byte)'u' },
        { "Heart", (byte)'h' },
        { "Red Brick", (byte)'r' },
        { "Challenge Minikit", (byte)'c' },
        { "Torpedo", (byte)'t' }
    };

    public static readonly Dictionary<string, byte> SpawnTypes = new()
    {
        { "None", 0 },
        { "Triggered", 2 },
        { "Auto-Collect", 6 },
    };

    public void UpdateRender(byte typeByte)
    {
        char type = (char)typeByte;
        string mat = (type) switch
        {
            's' => "silver_stud", 'g'=>"gold_stud",'b'=>"blue_stud", 'p'=>"purple_stud", 'h'=>"heart", 'r'=>"red_brick",
            _ => "silver_stud"
        };

        GameObject newObj = null;
        if (mat.EndsWith("_stud") || mat == "heart") {
            scaleFactor = new(0.01f, 0.01f, 0.01f);
            newObj = TTResourceManager.CreateObject($"{name}_render", PrimitiveType.Plane,transform.position,Vector3.zero,Vector3.one,mat,true,FindPropertyValue<string>("Name"));
            newObj.transform.parent = transform;
        }
        else if(mat == "red_brick")
        {
            scaleFactor = new(0.3f, .2f, .3f);
            newObj = TTResourceManager.CreateObject($"{name}_render", PrimitiveType.Cube, transform.position, Vector3.zero, Vector3.one, mat, false, FindPropertyValue<string>("Name"));
            newObj.transform.parent = transform;
        }

        if (renderObj != null) Destroy(renderObj);
        renderObj = newObj;
        RefreshRenderScale();
    }

    public void RefreshRenderScale()
    {
        if (renderObj == null) return;
        renderObj.transform.localScale = GizmoPickupSection.pickupScale * scaleFactor;
    }
}
