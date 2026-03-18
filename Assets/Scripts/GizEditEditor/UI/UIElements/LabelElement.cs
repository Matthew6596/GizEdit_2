using TMPro;
using UnityEngine;

public class LabelElement : EditorUIElement
{
    public TMP_Text label;
    public EditorColorType colorType;
    public EditorFontType fontType;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(label == null) label = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public override void ApplyCurrentTheme()
    {
        label.color = EditorTheme.GetColor<LabelElement>(colorType, label.color);
        label.fontSize = EditorTheme.GetFontSize(fontType);
    }

    public void SetText(string txt) => label.text = txt;
    public void SetFontSize(int pts) => label.fontSize = pts;

    public static LabelElement CreateLabel(Transform parent, string text) => Create(parent, text, EditorColorType.TextSecondary, EditorFontType.Label, FontStyles.Normal);

    public static LabelElement CreateTip(Transform parent, string text, bool isSpecial=false) => Create(parent, text, isSpecial ? EditorColorType.TextSpecial : EditorColorType.TextSecondary, EditorFontType.Tip, FontStyles.Italic);

    public static LabelElement CreateText(Transform parent, string text) => Create(parent, text, EditorColorType.TextPrimary, EditorFontType.Primary, FontStyles.Normal);

    public static LabelElement CreateHeader(Transform parent, string text) => Create(parent, text, EditorColorType.Title, EditorFontType.Header, FontStyles.Bold);

    public static LabelElement CreateInputPlacehold(Transform parent, string text) => Create(parent, text, EditorColorType.TextSecondary, EditorFontType.Input, FontStyles.Italic);

    public static LabelElement CreateInput(Transform parent, string text) => Create(parent, text, EditorColorType.TextPrimary, EditorFontType.Input, FontStyles.Normal);

    public static LabelElement Create(Transform parent, string text, EditorColorType colorType, EditorFontType fontType, FontStyles fontStyles)
    {
        GameObject obj = new("ui_label");

        var tmp = obj.AddComponent<TextMeshProUGUI>();
        var lbl = obj.AddComponent<LabelElement>();
        lbl.label = tmp;

        tmp.text = text;
        tmp.fontStyle = fontStyles;
        lbl.colorType = colorType;
        lbl.fontType = fontType;

        obj.transform.SetParent(parent);
        obj.transform.localScale = Vector3.one;

        lbl.ApplyCurrentTheme();
        return lbl;
    }
}
