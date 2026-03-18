using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class EditorUIElement : MonoBehaviour 
{
    private RectTransform _rect;
    public RectTransform Rect { 
        get 
        {
            if(_rect==null) return GetComponent<RectTransform>(); 
            return _rect;
        } 
        protected set
        {
            _rect = value;
        }
    }

    private EventTrigger ev;

    public void AddEventTrigger(UnityAction<BaseEventData> callback, EventTriggerType type)
    {
        if(ev == null) ev = gameObject.AddComponent<EventTrigger>();
        EventTrigger.TriggerEvent triggerEvent = new();
        triggerEvent.AddListener(callback);
        ev.triggers.Add(new EventTrigger.Entry() { callback = triggerEvent, eventID = type });
    }

    public abstract void ApplyCurrentTheme();

    public void AddPreferredWidth(int w)
    {
        var layoutEl = gameObject.AddComponent<LayoutElement>();
        layoutEl.preferredWidth = w;
    }
}

public class EditorPanel : EditorUIElement
{
    [SerializeField]
    private TMP_Text _title;
    public string Title { get => _title.text; set {  _title.text = value; } }

    public EditorUIElement[] children;
    public Transform contentArea;
    public EditorColorType colorType;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Rect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        //Dont close entire thing, keep top tab like dropdown area (depends on panel type)
        gameObject.SetActive(false);
    }

    public void Clear()
    {
        //foreach (var child in children) Destroy(child.gameObject);
        //children = new EditorUIElement[0];
        for(int i = contentArea.childCount - 1; i>=0; i--) Destroy(contentArea.GetChild(i).gameObject);
    }

    public T FindElement<T>() where T : EditorUIElement
    {
        return children.Where((c) => c is T).FirstOrDefault() as T;
    }

    public T[] FindElements<T>() where T: EditorUIElement
    {
        return children.Where((c) => c is T) as T[];
    }

    public override void ApplyCurrentTheme()
    {
        if(TryGetComponent(out Image img)) img.color = EditorTheme.GetColor<ImageElement>(colorType,img.color);
        //foreach (var child in children) child.ApplyCurrentTheme();
        //foreach (var el in contentArea.GetComponentsInChildren<EditorUIElement>()) el.ApplyCurrentTheme();
    }
}
