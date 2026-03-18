using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ResizeElement : EditorUIElement
{
    private enum DirH { None, Left, Right }
    private enum DirV { None, Down, Up }

    public EditorPanel parentPanel;

    [SerializeField]
    private DirH horizontal;
    [SerializeField] 
    private DirV vertical;

    private bool dragging = false;
    private Vector2 initDragPos;

    private void Awake()
    {
        //On Hover
        AddEventTrigger((e) =>
        {
#pragma warning disable CS8524
            CursorSet.SetCursor((horizontal,vertical) switch
            {
                (DirH.None,DirV.None) => CursorType.Unavailable,
                (DirH.None,DirV.Down) => CursorType.Drag_NS,
                (DirH.None,DirV.Up) => CursorType.Drag_NS,
                (DirH.Left,DirV.None) => CursorType.Drag_EW,
                (DirH.Left,DirV.Down) => CursorType.Drag_NESW,
                (DirH.Left,DirV.Up) => CursorType.Drag_NWSE,
                (DirH.Right,DirV.None) => CursorType.Drag_EW,
                (DirH.Right,DirV.Down) => CursorType.Drag_NWSE,
                (DirH.Right,DirV.Up) => CursorType.Drag_NESW,
            });
#pragma warning restore CS8524
        }, EventTriggerType.PointerEnter);

        //On Exit
        AddEventTrigger((e) =>
        {
            CursorSet.SetCursor(CursorType.Normal);
        }, EventTriggerType.PointerExit);

        //On Down
        AddEventTrigger((e) =>
        {
            initDragPos = e.currentInputModule.input.mousePosition;
            dragging = true;
        }, EventTriggerType.PointerDown);

        //On Move
        AddEventTrigger((e) =>
        {
            if (dragging)
            {
                //parentPanel.Rect.rect.
            }
        }, EventTriggerType.Move);

        //On Up
        AddEventTrigger((e) =>
        {
            dragging = false;
        }, EventTriggerType.PointerUp);
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

    }
}
