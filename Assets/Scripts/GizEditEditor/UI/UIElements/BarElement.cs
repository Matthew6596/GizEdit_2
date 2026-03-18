using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BarElement : EditorUIElement
{
    protected Image bar;
    public EditorColorType colorType;

    private void Awake()
    {
        bar = GetComponent<Image>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void SetFillAmount(float percent)
    {
        if(bar == null) bar = GetComponent<Image>();
        bar.fillAmount = percent;
    }

    public virtual void SetColor(Color col)
    {
        bar.color = col;
    }

    public override void ApplyCurrentTheme()
    {
        if (bar != null) bar.color = EditorTheme.GetColor<BarElement>(colorType,bar.color);
    }
}
