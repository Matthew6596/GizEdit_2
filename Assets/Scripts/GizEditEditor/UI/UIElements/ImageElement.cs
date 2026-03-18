using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImageElement : EditorUIElement
{
    public Image image;
    public EditorColorType colorType;
    public float alpha = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ApplyCurrentTheme()
    {
        Color32 col = EditorTheme.GetColor<ImageElement>(colorType, image.color);
        col.a = (byte)(col.a * alpha);
        image.color = col;
    }
}
