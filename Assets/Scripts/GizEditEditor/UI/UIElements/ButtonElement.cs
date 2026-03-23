using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonElement : EditorUIElement, IScrollHandler
{
    public Button btn;
    public EditorColorType colorType;

    private void Awake()
    {
        //On Hover
        AddEventTrigger((e) =>
        {
            CursorSet.SetCursor(CursorType.Click);
        }, EventTriggerType.PointerEnter);

        //On Exit
        AddEventTrigger((e) =>
        {
            CursorSet.SetCursor(CursorType.Normal);
        }, EventTriggerType.PointerExit);
    }

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
        Image img = btn.GetComponent<Image>();
        img.color = EditorTheme.GetColor<ButtonElement>(colorType, img.color);
        var btnTxt = btn.transform.GetChild(0).GetComponent<TMP_Text>();
        btnTxt.color = EditorTheme.GetColor<LabelElement>(EditorTheme.ConvertWindowToTextColor(colorType), btnTxt.color);
        btnTxt.fontSize = EditorTheme.GetFontSize(EditorFontType.Label);
    }

    public void OnScroll(PointerEventData eventData)
    {
        Transform parent = transform;
        while(parent != null)
        {
            if(parent.TryGetComponent<ScrollRect>(out var scroll))
            {
                scroll.OnScroll(eventData);
                return;
            }
            parent = parent.parent;
        }
        
    }
}
