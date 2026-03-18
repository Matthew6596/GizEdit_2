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

    public static readonly string[] PickupTypes = new string[]
    {
        "Silver Stud", "Gold Stud", "Blue Stud", "Purple Stud", "Minikit", "Powerup",
        "Heart", "Red Brick", "Challenge Minikit", "Torpedo"
    };

    public static int GetPickupType(char c) => (c) switch
    {
        's' => 0, 'g' => 1, 'b' => 2, 'p' => 3, 'm' => 4,
        'u' => 5, 'h' => 6, 'r' => 7, 'c' => 8, 't' => 9,
        _ => 0,
    };

    public static char GetPickupChar(int n) => (n) switch
    {
        0 => 's', 1 => 'g', 2 => 'b', 3 => 'p', 4 => 'm',
        5 => 'u', 6 => 'h', 7 => 'r', 8 => 'c', 9 => 't',
        _ => 's'
    };

    public void UpdateRender(int typeInd)
    {
        char type = GetPickupChar(typeInd);
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
