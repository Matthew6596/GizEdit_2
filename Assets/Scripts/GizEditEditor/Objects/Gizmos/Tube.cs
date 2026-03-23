using System.Drawing;
using UnityEngine;

public class Tube : TTObject
{
    public GameObject renderObj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHeight(float amt)
    {
        if (renderObj == null) return;
        Vector3 scale = renderObj.transform.localScale;
        scale.y = amt / 2;
        renderObj.transform.localScale = scale;
    }

    public void UpdateRadius(float amt)
    {
        if (renderObj == null) return;
        Vector3 scale = renderObj.transform.localScale;
        scale.x = amt * 2;
        scale.z = amt * 2;
        renderObj.transform.localScale = scale;
    }
}
