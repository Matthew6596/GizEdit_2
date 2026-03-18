using UnityEngine;

public abstract class MouseInteractable : MonoBehaviour
{
    public abstract CursorType CursorType { get; }
    public bool LeftMouseDown { get; set; }
    public bool RightMouseDown { get; set; }
    public bool IsMouseOver { get; set; }
    public Vector3 MouseDownPos { get; set; }

    public void OnMouseMove(Vector3 mouseDelta)
    {
        if(LeftMouseDown) OnLeftDrag(mouseDelta);
        if(RightMouseDown) OnRightDrag(mouseDelta);
    }

    public virtual void OnLeftClick() { }
    public virtual void OnRightClick() { }
    public virtual void OnLeftDrag(Vector3 mouseDelta) { }
    public virtual void OnRightDrag(Vector3 mouseDelta) { }
}
