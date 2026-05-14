using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum LayoutMode { None, Horizontal, Vertical }

[DefaultExecutionOrder(-1)]
public class EditorUIManager : MonoBehaviour
{
    public static EditorUIManager Instance { get; private set; }
    public static bool IsPopupOpen => Instance.popupStack.Count > 0;

    public EditorPanel propertyPanel, progressBar, optionsBar, hierarchyPanel, toolsPanel, settingsPanel;
    public Stack<EditorPanel> popupStack = new();
    public Color32 defaultProgressBarColor = new(0, 255, 0, 255);
    public Color32 errorColor = new(255, 0, 0, 255);
    public CursorSet defaultCursors;
    public int uispacing = 2;
    public int indentSize = 8;

    [Header("UI Element Assets")]
    public Sprite inputFieldSprite;
    public Sprite checkmarkSprite, dropArrowSprite, moveGizIcon, whiteSprite, whiteSpriteSliced, trashIcon, plusIcon;
    public GameObject dropdownPrefab, popupWindowPrefab, optionSubMenuPrefab, subOptionButtonPrefab;

    [NonSerialized]
    public Canvas canvas;
    public static float CanvasScale => Instance.canvas.transform.localScale.x;

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
    private readonly List<TTObject> hierarchyRoots = new();

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

        //Adding Default Menu Options
        AddMenuOption("File/Load Level", () => { TTLoader.Instance.LoadALevel(); });
        //AddMenuOption("File/Export Level", () => { TTExporter.Instance.Export(); });
        AddMenuOption("File/Load GIZ", () => { TTLoader.Instance.LoadAFile(); });
        AddMenuOption("File/Export GIZ", () => { TTExporter.Instance.Export(); });
        AddMenuOption("File/Unload All", () => { TTObjectManager.UnloadAll(); });
        AddMenuOption("Settings", () => { Settings.Instance.LoadMenu(); }, 0);
        AddMenuOption("Camera/TP to Obj", () => { CameraController.Instance.TeleportToLastSelectedObject(); });
        AddMenuOption("Camera/TP to 0,0,0", () => { CameraController.Instance.transform.position = Vector3.zero; });
        AddMenuOption("App/Update", () => { UpdateManager.Instance.CheckLatestVers(); });
        AddMenuOption("App/Report Bug", () => { UpdateManager.Instance.ReportBug(); });
        SetOptionPriority("File", -1);
        SetOptionPriority("App", 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Button AddEditorTool(string lbl, Action callback, int priority = 1)
    {
        var btnEl = CreateButton(toolsPanel.contentArea, TTProperty.FieldGenerateOptions.Default, 60).GetComponent<ButtonElement>();
        btnEl.transform.GetChild(0).GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Center;
        btnEl.SetText(lbl);
        btnEl.btn.onClick.AddListener(() => { callback?.Invoke(); });
        btnEl.gameObject.name = lbl.ToLower()+"_tool";
        return btnEl.btn;
    }

    public ButtonElement FindEditorTool(string name)
    {
        name = name.ToLower()+"_tool";
        foreach (Transform child in toolsPanel.contentArea)
        {
            if (child.name == name) return child.GetComponent<ButtonElement>();
        }
        return null;
    }

    public void RemoveEditorTool(string name)
    {
        ButtonElement btn = FindEditorTool(name);
        if(btn != null) Destroy(btn.gameObject);
    }

    public Button AddMenuOption(string path, Action callback, int priority=1)
    {
        Transform root = optionsBar.transform;
        OptionBarButtonElement btn = null;

        string[] btns = path.Split('/');
        foreach (string p in btns)
        {
            if (string.IsNullOrEmpty(p)) continue;

            //Create button for given option
            btn = FindMenuOption(root, p);
            if (btn == null)
            {
                btn = CreateOptionBarButton(p, root);
                RefreshOptionsPriorityOrder(root);
            }

            //Get submenu for that option and continue
            root = GetOptionButtonSubMenu(btn);
            root.gameObject.SetActive(true);
        }

        //Add callback to the last option added
        btn.btn.onClick.AddListener(() => { callback?.Invoke(); });
        if (btn.TryGetComponent<LayoutElement>(out var layout)) layout.layoutPriority = priority;

        root = optionsBar.transform;
        foreach (string p in btns)
        {
            if (string.IsNullOrEmpty(p)) continue;

            root = GetOptionButtonSubMenu(FindMenuOption(root, p));
            root.gameObject.SetActive(false);
        }

        return btn.btn;
    }

    public OptionBarButtonElement FindMenuOption(string path)
    {
        Transform root = optionsBar.transform;
        OptionBarButtonElement btn = null;
        foreach (string p in path.Split('/'))
        {
            btn = FindMenuOption(root, p);
            root = btn.subMenu;
            if (root == null) break;
        }
        return btn;
    }

    private OptionBarButtonElement FindMenuOption(Transform parent, string name)
    {
        foreach(Transform child in parent)
        {
            if(child.name == name) return child.GetComponent<OptionBarButtonElement>();
        }
        return null;
    }

    public void SetOptionPriority(string path, int priority)
    {
        var btn = FindMenuOption(path);
        if(btn != null && btn.TryGetComponent<LayoutElement>(out var layout)) layout.layoutPriority = priority;
        RefreshOptionsPriorityOrder(optionsBar.transform);
    }

    private Transform[] SortChildren(Transform parent)
    {
        List<Transform> children = new();
        foreach (Transform child in parent) children.Add(child);

        children.Sort((t1,t2) =>
        {
            return t1.GetComponent<LayoutElement>().layoutPriority - t2.GetComponent<LayoutElement>().layoutPriority;
        });

        return children.ToArray();
    }

    private void RefreshOptionsPriorityOrder(Transform parent)
    {
        var children = SortChildren(parent);
        for(int i=parent.childCount-1; i>=0; i--)
        {
            children[i].SetAsFirstSibling();
        }
    }

    public Transform GetOptionButtonSubMenu(OptionBarButtonElement parentOption)
    {
        if (parentOption == null) return null;
        if (parentOption.subMenu != null) return parentOption.subMenu;

        //Create sub menu
        GameObject subMenu = Instantiate(optionSubMenuPrefab, canvas.transform);
        parentOption.subMenu = subMenu.transform;
        subMenu.SetActive(true);

        //Position sub menu
        
        /*var parentRect = parentOption.GetComponent<RectTransform>();
        var subRect = subMenu.GetComponent<RectTransform>();
        
        Transform parentPanel = parentOption.transform.parent;
        if (parentPanel != null) LayoutRebuilder.ForceRebuildLayoutImmediate(parentPanel.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(subRect);
        Rect pRect = parentRect.rect;

        Vector3 offset = parentPanel.GetComponent<HorizontalLayoutGroup>() != null ? new Vector3(0, -pRect.height / 2) : new Vector3(pRect.width, pRect.height / 2);
        subRect.position = parentRect.position + (offset * canvas.transform.localScale.x);*/

        return parentOption.subMenu;
    }

    private OptionBarButtonElement CreateOptionBarButton(string name, Transform parent, Action action=null)
    {
        GameObject btn = Instantiate(subOptionButtonPrefab, parent);
        var btnEl = btn.GetComponent<ButtonElement>();
        btn.name = name;
        btnEl.SetText(name);
        if(action != null) btnEl.btn.onClick.AddListener(() => { action.Invoke(); });
        btnEl.ApplyCurrentTheme();
        return btn.GetComponent<OptionBarButtonElement>();
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
            button.onClick.AddListener(() => { popup.Close(); popupStack.Pop(); btnAction?.Invoke(); });
            var lbl = button.transform.GetChild(0).GetComponent<LabelElement>();
            lbl.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Center;
            lbl.SetText(btn.Item1);
            button.transform.SetSiblingIndex(btnsArea.childCount - 2);

            popup.AddChildren(new EditorUIElement[] { lbl, button.GetComponent<ButtonElement>() });
        }

        popup.ApplyCurrentTheme();
        popupStack.Push(popup);
        return popup;
    }

    public void PopupTest() => CreatePopup("Test", "This is a test for the popup window.", ("Cancel", () => { }), ("Ok", () => { }));

    public T Create<T>(Transform parent=null) where T : EditorUIElement
    {
        GameObject obj = new();
        if(parent != null) obj.transform.parent = parent;
        return obj.AddComponent<T>();
    }

    public Button AddObjectToHierarchy(string name, int indent=0, Action onSelect=null)
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

        btn.GetComponent<ButtonElement>().ApplyCurrentTheme();
        lblEl.ApplyCurrentTheme();

        return btn;
    }

    private readonly Stack<Transform> nodePool = new();
    private readonly List<Transform> activeNodes = new();
    private readonly Dictionary<TTObject, bool> collapses = new();

    private Transform CreateHierarchyNode()
    {
        // Root node
        Transform node = CreateContentArea(null, LayoutMode.Vertical, "pooled_node");

        // Header
        Transform header = CreateContentArea(node, LayoutMode.Horizontal, "header");

        // Button
        Button btn = CreateButton(header, TTProperty.FieldGenerateOptions.Default, 1000);
        btn.gameObject.name = "button";
        btn.GetComponent<LayoutElement>().flexibleWidth = float.MaxValue;

        var txt = btn.transform.GetChild(0).GetComponent<TMP_Text>();
        txt.alignment = TextAlignmentOptions.Left;
        txt.margin = new(4, 0, 0, 0);

        // Children container
        Transform children = CreateContentArea(node, LayoutMode.Vertical, "children");

        // Collapse button
        Button toggle = CreateIconButton(header, dropArrowSprite, TTProperty.FieldGenerateOptions.Default, indentSize);
        toggle.gameObject.name = "toggle";

        node.gameObject.SetActive(false);
        return node;
    }

    private Transform GetNode(Transform parent)
    {
        Transform node = nodePool.Count > 0 ? nodePool.Pop() : CreateHierarchyNode();
        node.SetParent(parent, false);
        node.gameObject.SetActive(true);
        activeNodes.Add(node);

        return node;
    }

    public void ClearHierarchyUI()
    {
        foreach (var node in activeNodes)
        {
            node.gameObject.SetActive(false);
            nodePool.Push(node);
        }
        activeNodes.Clear();

        foreach (var key in collapses.Keys.ToArray()) if (key == null) collapses.Remove(key);
    }

    public bool HierarchyObjectHasChildren(TTObject obj)
    {
        foreach (Transform child in obj.transform)
        {
            if (child.TryGetComponent<TTObject>(out var childObj) && childObj.GenerateInHierarchy) return true;
        }
        return false;
    }

    public void GenerateHierarchy(TTObject root, Transform parent)
    {
        if (root == null || !root.GenerateInHierarchy || root.gameObject == null) return;

        Transform node = GetNode(parent);

        Transform header = node.Find("header");
        Transform childrenContainer = node.Find("children");

        Button btn = header.Find("button").GetComponent<Button>();
        TMP_Text txt = btn.transform.GetChild(0).GetComponent<TMP_Text>();
        Button toggleBtn = header.Find("toggle").GetComponent<Button>();

        //Reset state
        btn.onClick.RemoveAllListeners();
        toggleBtn.onClick.RemoveAllListeners();
        if(parent != hierarchyPanel.contentArea) parent.GetComponent<LayoutGroup>().padding = new(indentSize, 0, 0, 0);

        //Set name
        txt.text = root.name;
        var nameProp = root.FindProperty("Name");
        nameProp?.onValueChanged.AddListener((e) =>
        {
            txt.text = GetStr(e.value.ToString(), $"unnamed_{root.name}");
        });
        if(nameProp != null) txt.text = GetStr(nameProp.Value.ToString(), $"unnamed_{root.name}");

        btn.onClick.AddListener(() => root.GeneratePropertyPanel());

        //Toggle collapse
        toggleBtn.gameObject.SetActive(HierarchyObjectHasChildren(root));

        if (!collapses.ContainsKey(root)) collapses.Add(root, true);

        toggleBtn.onClick.AddListener(() =>
        {
            collapses[root] = !collapses[root];
            childrenContainer.gameObject.SetActive(!collapses[root]);
        });
        childrenContainer.gameObject.SetActive(!collapses[root]);

        //Recurse
        foreach (Transform child in root.transform)
        {
            if (child.TryGetComponent<TTObject>(out var childObj)) GenerateHierarchy(childObj, childrenContainer);
        }
    }

    public void RefreshHierarchy() => StartCoroutine(RebuildHierarchy());

    IEnumerator RebuildHierarchy()
    {
        yield return null;
        ClearHierarchyUI();
        foreach (var root in hierarchyRoots) GenerateHierarchy(root, hierarchyPanel.contentArea);
    }

    public void AddHierarchyRoot(TTObject fileObj)
    {
        hierarchyRoots.Add(fileObj);
        RefreshHierarchy();
    }

    public void RemoveHierarchyRoot(TTObject obj)
    {
        if (hierarchyRoots.Contains(obj)) hierarchyRoots.Remove(obj);
        RefreshHierarchy();
    }

    public GameObject CreateGameObject(Transform parent, string name="gameobject")
    {
        GameObject obj = new(name);
        obj.transform.SetParent(parent);
        obj.transform.localScale = Vector3.one;
        return obj;
    }

    public Transform CreateContentArea(Transform parent, LayoutMode layout, string name="gameobject")
    {
        GameObject areaObj = CreateGameObject(parent, name);

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