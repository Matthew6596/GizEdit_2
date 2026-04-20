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
        void Hover()
        {
            CursorSet.SetCursor(CursorType.Click);
        }

        void UnHover()
        {
            CursorSet.SetCursor(CursorType.Normal);
        }

        void Click()
        {

        }

        //On Hover
        AddEventTrigger((e) => { Hover(); }, EventTriggerType.PointerEnter);

        //On Exit
        AddEventTrigger((e) => { UnHover(); }, EventTriggerType.PointerExit);

        AddEventTrigger((e) => { Click(); UnHover(); }, EventTriggerType.PointerClick);
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

    public void SetText(string text) => btn.transform.GetChild(0).GetComponent<TMP_Text>().text = text;

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
