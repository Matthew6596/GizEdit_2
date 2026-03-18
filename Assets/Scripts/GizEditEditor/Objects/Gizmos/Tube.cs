using UnityEngine;

public class Tube : TTObject
{
    private GameObject renderObj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateRender()
    {
        if(renderObj != null) Destroy(renderObj);
        //renderObj = newthing;
    }

    public void UpdateHeight(float amt)
    {
        if (renderObj == null) return;
        
    }

    public void UpdateRadius(float amt)
    {
        if (renderObj == null) return;

    }
}
