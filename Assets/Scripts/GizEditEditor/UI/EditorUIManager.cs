using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum LayoutMode { None, Horizontal, Vertical }

public class EditorUIManager : MonoBehaviour
{
    public static EditorUIManager Instance { get; private set; }

    public EditorPanel propertyPanel, progressBar, optionsBar, hierarchyPanel, toolsPanel, settingsPanel;
    public Stack<EditorPanel> popupStack;
    public Color32 defaultProgressBarColor = new(0, 255, 0, 255);
    public Color32 errorColor = new(255, 0, 0, 255);
    public CursorSet defaultCursors;
    public int uispacing = 2;
    public int indentSize = 8;

    [Header("UI Element Assets")]
    public Sprite inputFieldSprite;
    public Sprite checkmarkSprite, dropArrowSprite, moveGizIcon, whiteSprite, whiteSpriteSliced, trashIcon, plusIcon;
    public GameObject dropdownPrefab, popupWindowPrefab;

    private Canvas canvas;

    public Rect ViewportRect { 
        get
        {
            Rect canvasRect = canvas.GetComponent<RectTransform>().rect;
            float w = canvasRect.width;
            float h = canvasRect.height;
            float t = optionsBar.Rect.rect.height;
            float l = hierarchyPanel.Rect.rect.width;
            float b = toolsPanel.Rect.rect.height;
            float r = propertyPanel.Rect.rect.width;
            Vector2 pos = new(l, t);
            Vector2 size = new(w - l - r, h - t - b);
            return new(pos, size);
        } 
    }

    private Camera cam;
    private readonly List<HierarchyRoot> hierarchyRoots = new();

    private void Awake()
    {
        Instance = this;
        canvas = FindFirstObjectByType<Canvas>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;

        if(Settings.GetOrSetDefault("cursors", "default") == "default") CursorSet.Current = defaultCursors;
        else { } //load custom cursors
        CursorSet.SetCursor(CursorType.Normal);

        EditorTheme.SetTheme(Settings.GetOrSetDefault("theme", "default"));
        RefreshTheme();

        RefreshViewportRect();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenPropertyPanel(TTObject obj)
    {
        //load panel with object properties
        Debug.Log("Opening panel for " + obj.name);
        propertyPanel.Open();
    }
    public void ClosePropertyPanel() => propertyPanel.Hide();
    public void ClearPropertyPanel() => propertyPanel.Clear();

    public void ShowProgressBar(string title="Progress Bar", string desc="")
    {
        progressBar.Title = title;
        var bar = progressBar.FindElement<BarElement>();
        bar.SetFillAmount(0);
        bar.SetColor(defaultProgressBarColor);
        progressBar.FindElement<LabelElement>().label.text = desc;
        progressBar.Open();
    }
    public void UpdateProgressBar(float amt, string desc="", Color32? color=null)
    {
        var bar = progressBar.FindElement<BarElement>();
        bar.SetFillAmount(amt);
        bar.SetColor(color ?? defaultProgressBarColor);
        progressBar.FindElement<LabelElement>().label.text = desc;
    }
    public void CloseProgressBar() => progressBar.Hide();

    public void Err(string msg, Exception e=null, string title = "Error", params (string, Action)[] btns)
    {
        if (e != null) msg += ": " + e;
        Debug.LogError(msg);
        //error popup
        var popup = CreatePopup(title, msg, btns);
        var titleEl = popup.transform.GetChild(0).GetChild(0).GetComponent<LabelElement>();
        titleEl.colorType = EditorColorType.ErrRed;
        titleEl.ApplyCurrentTheme();
    }

    public void Warn(string msg, Exception e=null, string title = "Warning", params (string, Action)[] btns)
    {
        if (e != null) msg += ": " + e;
        Debug.LogWarning(msg);
        //warning popup
        var popup = CreatePopup(title, msg, btns);
        var titleEl = popup.transform.GetChild(0).GetChild(0).GetComponent<LabelElement>();
        titleEl.colorType = EditorColorType.WarnYellow;
        titleEl.ApplyCurrentTheme();
    }

    public void Inform(string msg, string title="Info", params (string, Action)[] btns)
    {
        Debug.Log(msg);
        //info popup
        var popup = CreatePopup(title, msg, btns);
    }

    public EditorPanel CreatePopup(string title, string msg, params (string,Action)[] btns)
    {
        if (btns == null || btns.Length == 0) btns = new (string, Action)[] { ("Close", null) };

        EditorPanel popup = Instantiate(popupWindowPrefab, canvas.transform).GetComponent<EditorPanel>();
        popup.transform.GetChild(0).GetChild(0).GetComponent<LabelElement>().SetText(title);

        Transform content = popup.transform.GetChild(1);
        content.GetChild(0).GetComponent<LabelElement>().SetText(msg);

        Transform btnsArea = content.GetChild(1);
        foreach(var btn in btns)
        {
            Button button = CreateButton(btnsArea, TTProperty.FieldGenerateOptions.Default, 60);
            button.GetComponent<LayoutElement>().minWidth = 60;
            Action btnAction = btn.Item2;
            button.onClick.AddListener(() => { popup.Close(); btnAction?.Invoke(); });
            var lbl = button.transform.GetChild(0).GetComponent<LabelElement>();
            lbl.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Center;
            lbl.SetText(btn.Item1);
            button.transform.SetSiblingIndex(btnsArea.childCount - 2);

            popup.AddChildren(new EditorUIElement[] { lbl, button.GetComponent<ButtonElement>() });
        }

        popup.ApplyCurrentTheme();
        return popup;
    }

    public void PopupTest() => CreatePopup("Test", "This is a test for the popup window.", ("Cancel", () => { }), ("Ok", () => { }));

    public T Create<T>(Transform parent=null) where T : EditorUIElement
    {
        GameObject obj = new();
        if(parent != null) obj.transform.parent = parent;
        return obj.AddComponent<T>();
    }

    public Button AddObjectToHierarchy(string name, int indent=0, Action onSelect=null, bool collapsible=false, HierarchySection section=null)
    {
        Transform content = CreateContentArea(hierarchyPanel.contentArea, LayoutMode.Horizontal);

        for(int i=0; i<indent; i++)
        {
            var obj = CreateGameObject(content, "indent");
            var layout = obj.AddComponent<LayoutElement>();
            layout.minWidth = indentSize;
        }

        Button btn = CreateButton(content, TTProperty.FieldGenerateOptions.Default, 1000);
        btn.GetComponent<LayoutElement>().flexibleWidth = float.MaxValue;

        var txt = btn.transform.GetChild(0).GetComponent<TMP_Text>();
        var lblEl = txt.GetComponent<LabelElement>();
        lblEl.fontType = EditorFontType.Input;
        txt.text = name;
        txt.alignment = TextAlignmentOptions.Left;
        txt.margin = new(4, 0, 0, 0);

        btn.onClick.AddListener(() => { onSelect?.Invoke(); });

        if (collapsible && section != null)
        {
            //create dropdown button
            Button dropbtn = CreateIconButton(content, dropArrowSprite, TTProperty.FieldGenerateOptions.Default, indentSize);
            dropbtn.GetComponent<LayoutElement>().minWidth = indentSize;

            var droptxt = dropbtn.transform.GetChild(0).GetComponent<TMP_Text>();

            dropbtn.onClick.AddListener(() => 
            {
                section.ToggleCollapse();
            });

            dropbtn.GetComponent<ButtonElement>().ApplyCurrentTheme();
        }

        btn.GetComponent<ButtonElement>().ApplyCurrentTheme();
        lblEl.ApplyCurrentTheme();

        return btn;
    }

    /*public Button FindHierarchyButton(string path)
    {
        string[] btns = path.Split('/');
        int ind = 0;

        static Transform GetButton(Transform obj, out int indent)
        {
            int count = 0;
            while (count < obj.childCount && obj.GetChild(count).gameObject.name == "indent") count++;
            indent = count;
            return obj.GetChild(count);
        }

        for(int i = hierarchyPanel.contentArea.childCount-1; i>=0; i--)
        {
            Transform row = hierarchyPanel.contentArea.GetChild(0);
            Transform btn = GetButton(row, out int indent);
            if (indent < ind) return null;
            if (indent == ind && btn.GetChild(0).GetComponent<TMP_Text>().text == btns[ind])
            {
                ind++;
                if (ind == btns.Length) return btn.GetComponent<Button>();
            }
        }

        return null;
    }

    public void OpenHierarchyPath(string path)
    {
        
    }

    public void CloseHierarchyPath(string path)
    {

    }

    private void EnsureHierarchyPathExists(string path)
    {
        string[] btns = path.Split('/');
        string p = btns[0];
        var root = hierarchyRoots.Where(r => r.rootObj.name == btns[0]).FirstOrDefault().rootObj;
        for(int i=1; i<btns.Length; i++)
        {
            Button btn = FindHierarchyButton(p);
            TTObject obj = root;
            if (btn == null) btn = AddObjectToHierarchy(btns[i - 1], i - 1, () => { obj.GeneratePropertyPanel(); });

            p += btns[i];
        }
    }*/

    public void GenerateHierarchyFromRoot(TTObject fileObj, string[] paths)
    {
        HierarchyRoot root = new(fileObj, paths);
        hierarchyRoots.Add(root);
        GenerateHierarchyFromRoot(root);

    }

    private string[] GenerateObjectHierarchy(TTObject obj, int indent, string[] dirs, string[] prevPath, HierarchySection section, int index=0)
    {
        List<string> pathTracker = new();
        for (int i = index; i < dirs.Length; i++)
        {
            string dir = dirs[i];

            //Use previous button/name (already generated)
            if (dir == "..")
            {
                if (i < prevPath.Length)
                {
                    pathTracker.Add(prevPath[i]);
                    indent++;
                    obj = obj.FindProperty(prevPath[i]).Value as TTObject;
                    continue;
                }
                else break;
            }

            //Get property and generate button
            var prop = obj.FindProperty(dir);
            if (prop == null) break;

            void GenerateChildProp(ChildProperty child, bool collapsible, HierarchySection childSection)
            {
                obj = child.Value as TTObject;
                var childObj = obj;
                var btn = AddObjectToHierarchy(child.name, indent, () => { childObj.GeneratePropertyPanel(); }, collapsible, collapsible?childSection.subSections[^1]:null);
                var nameProp = childObj.FindProperty("Name");
                nameProp?.onValueChanged.AddListener((e) =>
                {
                    string newName = e.value.ToString();
                    btn.transform.GetChild(0).GetComponent<TMP_Text>().text = GetStr(newName, $"unnamed_{childObj.name}");
                });
                childSection.AddField(btn.transform.parent.gameObject);
            }

            if (prop is ChildProperty child)
            {
                bool collapsible = false;
                HierarchySection subSection = new(true);
                if(i+1 < dirs.Length)
                {
                    var nextProp = (child.Value as TTObject).FindProperty(dirs[i + 1]);
                    collapsible = (nextProp != null && nextProp is ChildrenProperty);
                    if (collapsible) section.AddSubSection(subSection);
                }
                GenerateChildProp(child,collapsible,section);

                indent++;
                pathTracker.Add(prop.name);
                continue;
            }
            else if (prop is ChildrenProperty children)
            {
                foreach (var c in children.Value as ChildProperty[])
                {
                    GenerateChildProp(c, false, section.subSections[^1]);
                    GenerateObjectHierarchy(c.Value as TTObject, indent + 1, dirs, prevPath.Append(prop.name).ToArray(), section.subSections[^1], i + 1);
                }
                pathTracker.Add(prop.name);
                break;
            }
            else break;
        }

        return pathTracker.ToArray();
    }

    private void GenerateHierarchyFromRoot(HierarchyRoot root)
    {
        //GIZ File [V]
        //   +--Gizmo Section 1 [V]
        //   |     +--Object 1 [V]
        //   |     |     +--Special Object 1
        //   |     +--Object 2
        //   +--Gizmo Section 2 [V]
        //         +--Object 1
        //         +--Object 2 [V]
        //               +--Special Object 1

        //Gizmos, GizBuildit Section, GizBuildits, Special Objects, Special Objects
        //.. --> Increase Indent (starts same as previous route)
        //TTObject --> Create BTN, generates prop panel
        //ChildProperty --> Create BTN, generates prop panel of TTObject value
        //ChildrenProperty --> Loop each ChildProperty, follow above steps

        AddObjectToHierarchy(root.rootObj.name, 0, () => { root.rootObj.GeneratePropertyPanel(); }, true, root.section);

        string[] prevPath = new string[0];
        TTObject currObj = root.rootObj;
        foreach(var path in root.paths)
        {
            int indent = 1;
            string[] dirs = path.Split("/");
            prevPath = GenerateObjectHierarchy(currObj, indent, dirs, prevPath, root.section);
        }
    }

    public void RefreshHierarchy()
    {
        hierarchyPanel.Clear();
        foreach (var root in hierarchyRoots) GenerateHierarchyFromRoot(root);
    }

    public Button AddMenuOption(string path, Action callback)
    {
        Debug.LogWarning("Add Menu Option is not implemented yet...");
        //throw new NotImplementedException("Add Menu Option not implemented");
        return null;
    }

    public GameObject CreateGameObject(Transform parent, string name="gameobject")
    {
        GameObject obj = new(name);
        obj.transform.SetParent(parent);
        obj.transform.localScale = Vector3.one;
        return obj;
    }

    public Transform CreateContentArea(Transform parent, LayoutMode layout)
    {
        GameObject areaObj = CreateGameObject(parent);

        LayoutGroup layoutGroup = null;
        switch (layout)
        {
            case LayoutMode.None: break;
            case LayoutMode.Horizontal:
                var hoz = areaObj.AddComponent<HorizontalLayoutGroup>();
                hoz.spacing = uispacing;
                hoz.childControlWidth = true;
                hoz.childControlHeight = true;
                hoz.childForceExpandWidth = true;
                hoz.childForceExpandHeight = false;
                break;
            case LayoutMode.Vertical:
                var vert = areaObj.AddComponent<VerticalLayoutGroup>();
                vert.spacing = uispacing;
                vert.childControlWidth = true;
                vert.childControlHeight = true;
                vert.childForceExpandWidth = true;
                vert.childForceExpandHeight = false;
                break;
        }

        if(layoutGroup != null) layoutGroup.padding = new(uispacing, uispacing, uispacing, uispacing);

        return areaObj.transform;
    }

    public Transform CreateContentAreaBG(Transform parent, LayoutMode layout, EditorColorType colorType)
    {
        Transform content = CreateContentArea(parent, layout);
        RectTransform imgRect = CreateImg(content, whiteSprite, Vector2.zero, Vector2.zero, Vector2.one, "contentarea_bgimg", colorType).GetComponent<RectTransform>();
        StretchRectTransform(imgRect);
        return content;
    }

    private T CreateSelectableInput<T>(Transform parent, TTProperty.FieldGenerateOptions options, string inputTypeName, int? preferredInputWidth=null) where T : Selectable
    {
        GameObject inputObj = CreateGameObject(parent,$"input_{inputTypeName}");
        var inp = inputObj.AddComponent<T>();

        if(preferredInputWidth.HasValue) inp.gameObject.AddComponent<LayoutElement>().preferredWidth = preferredInputWidth.Value;
        if (options.HasFlag(TTProperty.FieldGenerateOptions.Readonly)) inp.interactable = false;

        return inp;
    }

    private void StretchRectTransform(RectTransform rect, Vector2? topleft=null, Vector2? bottomright=null)
    {
        Vector2 tl = topleft ?? Vector2.zero;
        Vector2 br = -bottomright ?? Vector2.zero;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = tl;
        rect.offsetMax = br;
    }

    private Image CreateImg(Transform parent, Sprite sprite, Vector2 anchormin, Vector2 anchormax, Vector2 size, string name, EditorColorType colorType, float alpha=1)
    {
        var img = CreateGameObject(parent, name);
        var imgRect = img.AddComponent<RectTransform>();
        imgRect.anchorMin = anchormin;
        imgRect.anchorMax = anchormax;
        imgRect.sizeDelta = size;
        var image = img.AddComponent<Image>();
        var imageEl = img.AddComponent<ImageElement>();
        imageEl.image = image;
        imageEl.colorType = colorType;
        imageEl.alpha = alpha;
        image.sprite = sprite;
        imageEl.ApplyCurrentTheme();
        return image;
    }

    public TMP_InputField CreateInputField(Transform parent, TTProperty.FieldGenerateOptions options, int? preferredInputWidth=null)
    {
        var inp = CreateSelectableInput<TMP_InputField>(parent, options, "InputField", preferredInputWidth ?? 80);
        var inpObj = inp.gameObject;
        if (options.HasFlag(TTProperty.FieldGenerateOptions.Readonly))
        {
            inp.interactable = true;
            inp.readOnly = true;
            Color newSelectionColor = inp.selectionColor;
            newSelectionColor.a = 0.08f;
            inp.selectionColor = newSelectionColor;
        }

        //give preferred hight 16
        if (!inp.TryGetComponent(out LayoutElement layout)) layout = inpObj.AddComponent<LayoutElement>();
        layout.preferredHeight = 16;

        //programmatically create objects for input
        var img = inpObj.AddComponent<Image>();
        img.sprite = inputFieldSprite;
        inp.targetGraphic = img;
        img.type = Image.Type.Sliced;
        var imgEl = inpObj.AddComponent<ImageElement>();
        imgEl.image = img;
        imgEl.colorType = EditorColorType.WindowTertiary;
        imgEl.ApplyCurrentTheme();

        //mask
        var mask = CreateGameObject(inp.transform, "TextArea");
        RectTransform maskRect = mask.gameObject.AddComponent<RectTransform>();
        inp.textViewport = maskRect;
        maskRect.anchorMin = Vector2.zero;
        maskRect.anchorMax = Vector2.one;
        maskRect.offsetMin = new(4,2);
        maskRect.offsetMax = new(-4,-2);

        mask.AddComponent<RectMask2D>();

        //placehold text
        var placeholdElement = LabelElement.CreateInputPlacehold(mask.transform, "");
        inp.placeholder = placeholdElement.label;
        placeholdElement.label.alignment = TextAlignmentOptions.Left;
        StretchRectTransform(placeholdElement.GetComponent<RectTransform>());

        //text
        var textElement = LabelElement.CreateInput(mask.transform, "");
        inp.textComponent = textElement.label;
        textElement.label.alignment = TextAlignmentOptions.Left;
        StretchRectTransform(textElement.GetComponent<RectTransform>());

        inpObj.SetActive(false); //shenanigans (selection/caret visibility)
        inpObj.SetActive(true);

        return inp;
    }

    public Toggle CreateToggle(Transform parent, TTProperty.FieldGenerateOptions options)
    {
        var inp = CreateSelectableInput<Toggle>(parent, options, "Toggle");
        var inpObj = inp.gameObject;

        inpObj.AddComponent<GridLayoutGroup>().cellSize = new(14,14);
        inpObj.AddComponent<LayoutElement>().preferredWidth = 80;

        //programmatically create objects for input
        //set bg w/h 14, checkmark w/h 16
        var bgImg = CreateImg(inpObj.transform, inputFieldSprite, new(0, 0.5f), new(0, 0.5f), new(14, 14), "toggle_bg", EditorColorType.WindowTertiary);
        var bgLayout = bgImg.gameObject.AddComponent<HorizontalLayoutGroup>();
        bgLayout.childForceExpandWidth = false;
        bgLayout.childForceExpandHeight = false;
        bgLayout.childControlWidth = false;
        bgLayout.childControlHeight = false;
        bgLayout.padding = new(-1, 0, -1, 0);

        var checkImg = CreateImg(bgImg.transform, checkmarkSprite, new(.5f, .5f), new(.5f, .5f), new(14, 14), "toggle_check", EditorColorType.TextPrimary);

        inp.targetGraphic = bgImg;
        inp.graphic = checkImg;

        return inp;
    }

    public TMP_Dropdown CreateDropdown(Transform parent, TTProperty.FieldGenerateOptions options, int? preferredInputWidth = null)
    {
        var inpObj = Instantiate(dropdownPrefab,parent);
        inpObj.name = "input_Dropdown";
        var inp = inpObj.GetComponent<TMP_Dropdown>();

        LayoutElement layoutEl = inp.gameObject.AddComponent<LayoutElement>();
        layoutEl.preferredWidth = preferredInputWidth ?? 80;
        layoutEl.preferredHeight = 16;
        if (options.HasFlag(TTProperty.FieldGenerateOptions.Readonly)) inp.interactable = false;

        //set input field's properties/sprites
        inp.GetComponent<Image>().sprite = inputFieldSprite;
        inpObj.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = dropArrowSprite;

        Transform template = inpObj.transform.GetChild(2);
        template.gameObject.GetComponent<Image>().sprite = inputFieldSprite;

        Transform scrollBar = template.GetChild(1);
        scrollBar.gameObject.GetComponent<Image>().sprite = inputFieldSprite;
        scrollBar.GetChild(0).GetChild(0).gameObject.GetComponent<Image>().sprite = inputFieldSprite;

        template.GetChild(0).GetChild(0).GetChild(0).GetChild(1).gameObject.GetComponent<Image>().sprite = checkmarkSprite;

        foreach(var el in inpObj.GetComponentsInChildren<EditorUIElement>(true)) el.ApplyCurrentTheme();
        return inp;
    }

    public Slider CreateSlider(Transform parent, TTProperty.FieldGenerateOptions options, int? preferredInputWidth = null)
    {
        var inp = CreateSelectableInput<Slider>(parent, options, "Slider", preferredInputWidth);
        var inpObj = inp.gameObject;

        //give preferred hight 14
        if (!inp.TryGetComponent(out LayoutElement layout)) layout = inpObj.AddComponent<LayoutElement>();
        layout.preferredHeight = 14;

        //programmatically create objects for input
        throw new NotImplementedException("Slider input is not implemented yet.");

        return inp;
    }

    public Button CreateButton(Transform parent, TTProperty.FieldGenerateOptions options, int? preferredInputWidth = null, Sprite sprite=null)
    {
        var inp = CreateSelectableInput<Button>(parent, options, "Button", preferredInputWidth);
        var inpObj = inp.gameObject;

        //give preferred hight 14
        if (!inp.TryGetComponent(out LayoutElement layout)) layout = inpObj.AddComponent<LayoutElement>();
        layout.preferredHeight = 14;

        //programmatically create objects for input
        var img = inpObj.AddComponent<Image>();
        var btnEl = inpObj.AddComponent<ButtonElement>();
        img.sprite = sprite ?? inputFieldSprite;
        img.type = Image.Type.Sliced;
        inp.targetGraphic = img;
        btnEl.btn = inp;
        btnEl.colorType = EditorColorType.WindowSecondary;

        //button text
        var txt = LabelElement.CreateInput(inpObj.transform, "");
        StretchRectTransform(txt.GetComponent<RectTransform>());

        btnEl.ApplyCurrentTheme();

        return inp;
    }

    public Button CreateIconButton(Transform parent, Sprite icon, TTProperty.FieldGenerateOptions options, int? preferredInputWidth = null)
    {
        Button btn = CreateButton(parent, options, preferredInputWidth, whiteSpriteSliced);
        var layoutEl = btn.GetComponent<LayoutElement>();
        layoutEl.minWidth = 14;
        layoutEl.minHeight = 14;
        var img = CreateGameObject(btn.transform, "btn_icon");
        StretchRectTransform(img.AddComponent<RectTransform>());
        img.AddComponent<Image>().sprite = icon;
        return btn;
    }

    public Transform CreateLabeledField(Transform parent, string lbl, TTProperty.FieldGenerateOptions options, int? lblPreferredWidth=null)
    {
        Transform contentArea = CreateContentArea(parent, LayoutMode.Horizontal);
        if (options.HasFlag(TTProperty.FieldGenerateOptions.ShowName))
        {
            var lblEl = LabelElement.CreateLabel(contentArea, lbl);
            lblEl.label.alignment = TextAlignmentOptions.Left;
            if(lblPreferredWidth.HasValue) lblEl.gameObject.AddComponent<LayoutElement>().preferredWidth = lblPreferredWidth.Value;
        }
        return contentArea;
    }

    public TMP_InputField CreateLabeledInputField(Transform parent, string lbl, TTProperty.FieldGenerateOptions options, int preferredInputWidth=80)
    {
        Transform contentArea = CreateLabeledField(parent, lbl, options);
        var inp = CreateInputField(contentArea,options,preferredInputWidth);
        return inp;
    }

    public void RefreshTheme()
    {
        foreach(var el in FindObjectsByType<EditorUIElement>(FindObjectsInactive.Include, FindObjectsSortMode.None)) el.ApplyCurrentTheme();
    }

    public void RefreshViewportRect()
    {
        FindFirstObjectByType<CameraController>().RefreshViewportRect();
    }

    public static string GetStr(string str, string strIfNull) => str.Length == 0 || str[0] == 0 || string.IsNullOrWhiteSpace(str) ? strIfNull : str;

    public class HierarchyRoot
    {
        public TTObject rootObj;
        public string[] paths;
        public HierarchySection section;

        public HierarchyRoot(TTObject rootObj, string[] paths)
        {
            this.rootObj = rootObj;
            this.paths = paths;
            section = new(true);
        }

        public void AddField(GameObject field) => section.AddField(field);
    }

    public class HierarchySection
    {
        public bool collapsed;
        public List<GameObject> fields;
        public List<HierarchySection> subSections;

        public HierarchySection(bool collapsed)
        {
            this.collapsed = collapsed;
            fields = new();
            subSections = new();
        }

        public void AddField(GameObject field)
        {
            fields.Add(field);
            field.SetActive(!collapsed);
        }

        public void AddSubSection(HierarchySection section) => subSections.Add(section);

        public void ToggleCollapse()
        {
            foreach (GameObject field in fields) field.SetActive(collapsed);
            collapsed = !collapsed;
            if(collapsed) foreach (var section in subSections) section.Collapse(collapsed);
        }

        public void Collapse(bool collapsed)
        {
            this.collapsed = !collapsed;
            ToggleCollapse();
        }
    }
}

public enum CursorType { Normal, Click, Help, Unavailable, Drag_EW, Drag_NS, Drag_NESW, Drag_NWSE, Move, Pen, Person, Pin }
[Serializable]
public struct CursorSet
{
    public static CursorSet Current;

    [SerializeField]
    private Texture2D normal,click,help,unavailable,drag_ew,drag_ns,drag_nesw,drag_nwse,move,pen,person,pin;
    [SerializeField]
    private Vector2 normalHotSpot, clickHotSpot, helpHotSpot, unavailableHotSpot, drag_ewHotSpot, drag_nsHotSpot,
        drag_neswHotSpot, drag_nwseHotSpot, moveHotSpot, penHotSpot, personHotSpot, pinHotSpot;
    private CursorType lastSetType;

    public readonly Texture2D GetCursor(CursorType type) => (type) switch
    {
        CursorType.Normal => normal,
        CursorType.Click => click,
        CursorType.Help => help,
        CursorType.Unavailable => unavailable,
        CursorType.Drag_EW => drag_ew,
        CursorType.Drag_NS => drag_ns,
        CursorType.Drag_NESW => drag_nesw,
        CursorType.Drag_NWSE => drag_nwse,
        CursorType.Move => move,
        CursorType.Pen => pen,
        CursorType.Person => person,
        CursorType.Pin => pin,
        _ => normal
    };

    public readonly Vector2 GetHotSpot(CursorType type) => (type) switch
    {
        CursorType.Normal => normalHotSpot,
        CursorType.Click => clickHotSpot,
        CursorType.Help => helpHotSpot,
        CursorType.Unavailable => unavailableHotSpot,
        CursorType.Drag_EW => drag_ewHotSpot,
        CursorType.Drag_NS => drag_nsHotSpot,
        CursorType.Drag_NESW => drag_neswHotSpot,
        CursorType.Drag_NWSE => drag_nwseHotSpot,
        CursorType.Move => moveHotSpot,
        CursorType.Pen => penHotSpot,
        CursorType.Person => personHotSpot,
        CursorType.Pin => pinHotSpot,
        _ => normalHotSpot
    };

    public static bool IsCursor(CursorType type) => Current.lastSetType == type;

    public static void SetCursor(CursorType type)
    {
        Cursor.SetCursor(Current.GetCursor(type), Current.GetHotSpot(type), CursorMode.Auto);
        Current.lastSetType = type;
    }
}

public enum EditorColorType { WindowPrimary, WindowSecondary, WindowTertiary, Title, TextPrimary, TextSecondary, TextSpecial, GoodGreen, HelpBlue, ErrRed, WarnYellow }
public enum EditorFontType { Header, Primary, Tip, Label, Special, Input }
[Serializable]
public struct EditorTheme
{
    private readonly struct Theme
    {
        public readonly Dictionary<ColType, Color32> colors;
        public readonly Dictionary<EditorFontType, int> fontSizes;

        public Theme((EditorFontType,int)[] fontSizes, params (ColType, Color32)[] colors)
        {
            this.colors = new();
            foreach (var pair in colors) this.colors.Add(pair.Item1, pair.Item2);

            this.fontSizes = new();
            foreach(var pair in fontSizes) this.fontSizes.Add(pair.Item1, pair.Item2);
        }

        public static (ColType,Color32) Col<T>(EditorColorType type, byte r, byte g, byte b) where T:EditorUIElement => new(new(typeof(T), type), new(r, g, b, 255));
    }

    private readonly struct ColType
    {
        public readonly Type elementType;
        public readonly EditorColorType colorType;
        public ColType(Type elementType, EditorColorType colorType)
        {
            this.elementType = elementType;
            this.colorType = colorType;
        }
    }

    private static Theme Current;

    private readonly static (EditorFontType, int)[] DefaultFontSizes = new (EditorFontType, int)[]
    {
        new(EditorFontType.Header,16), new(EditorFontType.Primary, 12), new(EditorFontType.Tip, 10),
        new(EditorFontType.Label, 12), new(EditorFontType.Input, 10), new(EditorFontType.Special, 12),
    };

    private readonly static Theme DefaultLight = new(DefaultFontSizes,
        Theme.Col<ImageElement>(EditorColorType.WindowPrimary, 242, 242, 242), 
        Theme.Col<ImageElement>(EditorColorType.WindowSecondary, 242, 242, 242), 
        Theme.Col<ImageElement>(EditorColorType.WindowTertiary, 242, 242, 242), 
        Theme.Col<ImageElement>(EditorColorType.Title, 242, 242, 242), 
        Theme.Col<LabelElement>(EditorColorType.Title, 0, 0, 0), 
        Theme.Col<LabelElement>(EditorColorType.TextPrimary, 10, 10, 10), 
        Theme.Col<LabelElement>(EditorColorType.TextSecondary, 60, 60, 60), 
        Theme.Col<LabelElement>(EditorColorType.TextSpecial, 0, 100, 200), 
        Theme.Col<LabelElement>(EditorColorType.WarnYellow, 128, 128, 0), 
        Theme.Col<ImageElement>(EditorColorType.WarnYellow, 200, 200, 0), 
        Theme.Col<LabelElement>(EditorColorType.ErrRed, 128, 0, 0), 
        Theme.Col<ImageElement>(EditorColorType.ErrRed, 200, 0, 0)
    );

    private readonly static Theme DefaultDark = new(DefaultFontSizes,
        Theme.Col<ImageElement>(EditorColorType.WindowPrimary, 48, 48, 48),
        Theme.Col<ImageElement>(EditorColorType.WindowSecondary, 56, 56, 56),
        Theme.Col<ImageElement>(EditorColorType.WindowTertiary, 64, 64, 64),
        Theme.Col<ImageElement>(EditorColorType.Title, 80, 80, 80),
        Theme.Col<ButtonElement>(EditorColorType.WindowPrimary, 128, 128, 128),
        Theme.Col<ButtonElement>(EditorColorType.WindowSecondary, 96, 96, 96),
        Theme.Col<ButtonElement>(EditorColorType.WindowTertiary, 72, 72, 72),
        Theme.Col<LabelElement>(EditorColorType.Title, 240, 240, 240),
        Theme.Col<LabelElement>(EditorColorType.TextPrimary, 232, 232, 232),
        Theme.Col<LabelElement>(EditorColorType.TextSecondary, 200, 200, 200),
        Theme.Col<LabelElement>(EditorColorType.TextSpecial, 0, 100, 200),
        Theme.Col<ImageElement>(EditorColorType.TextPrimary, 232, 232, 232),
        Theme.Col<LabelElement>(EditorColorType.WarnYellow, 220, 188, 64),
        Theme.Col<ImageElement>(EditorColorType.WarnYellow, 200, 200, 50),
        Theme.Col<ButtonElement>(EditorColorType.WarnYellow, 200, 200, 150),
        Theme.Col<LabelElement>(EditorColorType.ErrRed, 220, 64, 64),
        Theme.Col<ImageElement>(EditorColorType.ErrRed, 200, 50, 50),
        Theme.Col<ButtonElement>(EditorColorType.ErrRed, 200, 150, 150),
        Theme.Col<LabelElement>(EditorColorType.GoodGreen, 64, 220, 64),
        Theme.Col<ImageElement>(EditorColorType.GoodGreen, 50, 200, 50),
        Theme.Col<ButtonElement>(EditorColorType.GoodGreen, 150, 200, 150),
        Theme.Col<LabelElement>(EditorColorType.HelpBlue, 64, 64, 220),
        Theme.Col<ImageElement>(EditorColorType.HelpBlue, 50, 50, 200),
        Theme.Col<ButtonElement>(EditorColorType.HelpBlue, 150, 150, 200)
    );

    private readonly static Dictionary<string, Theme> Themes = new()
    {
        { "default_light", DefaultLight }, {"default_dark", DefaultDark}
    };

    public static void SetTheme(string theme) => Current = Themes.ContainsKey(theme)?Themes[theme]:DefaultDark;
    public static Color32 GetColor<T>(EditorColorType colorType, Color32 defaultColor)
    {
        if (Current.colors.TryGetValue(new(typeof(T), colorType), out Color32 col)) return col;
        return defaultColor;
    }
    public static int GetFontSize(EditorFontType fontType) => Current.fontSizes[fontType];
    public static EditorColorType ConvertWindowToTextColor(EditorColorType colorType)
    {
        if (colorType == EditorColorType.WindowPrimary) return EditorColorType.TextPrimary;
        else if (colorType == EditorColorType.WindowSecondary) return EditorColorType.TextSecondary;
        else if (colorType == EditorColorType.WindowTertiary) return EditorColorType.TextSpecial;
        else return EditorColorType.Title;
    }
}