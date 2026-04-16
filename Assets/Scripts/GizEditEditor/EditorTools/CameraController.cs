using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public float acceleration=10, friction=0.9f;
    public float clickLength = 0.35f, cameraSensitivity = 0.2f;
    public float scrollSensitivity = 0.1f;

    private Vector2 moveInp, lookInp;
    private bool movingUp, movingDown, canLookL, canLookR, potentialClickL, potentialClickR;
    private bool CanLook => canLookL | canLookR;

    private Vector3 velocity;

    private MouseInteractable hoveredObj;
    private Vector3 hoverObjMousePos;

    private Rect viewportRect;

    private Camera cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
    }

    private void FixedUpdate()
    {
        
    }

    private void Update()
    {
        if (CanLook) //(or move)
        {
            if (hoveredObj != null && canLookL) return;

            cam.transform.Rotate(Vector3.left, lookInp.y * cameraSensitivity, Space.Self); //To-do: also clamp rotation
            transform.Rotate(Vector3.up, lookInp.x * cameraSensitivity, Space.Self);

            float verticalMovement = movingUp ? acceleration : 0;
            if (movingDown) verticalMovement -= acceleration;
            velocity += new Vector3(moveInp.x * acceleration, verticalMovement, moveInp.y * acceleration);
            velocity *= friction;

            transform.Translate(new(velocity.x * Time.smoothDeltaTime, 0, velocity.z * Time.smoothDeltaTime), Space.Self);
            transform.Translate(new(0, velocity.y * Time.smoothDeltaTime, 0), Space.World);

            return;
        }
        else
        {
            velocity = Vector3.zero;
        }

        Vector3 mousePos = Input.mousePosition;
        if (InsideViewportOrWindow(mousePos))
        {
            Ray r = cam.ScreenPointToRay(mousePos);
            var hits = Physics.RaycastAll(r);

            if (hits.Length > 0)
            {
                //Get the nearest hit (prioritize gizmos)
                RaycastHit hit;
                var gizHits = hits.Where(h => h.collider.gameObject.GetComponent<EditorGizmoPart>() != null).ToArray();
                if (gizHits != null && gizHits.Length > 0)
                {
                    if (gizHits.Length > 1) Array.Sort(gizHits, (a, b) => a.distance.CompareTo(b.distance));
                    hit = gizHits[0];
                }
                else
                {
                    if (hits.Length > 1) Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
                    hit = hits[0];
                }
                

                Transform hitParent = hit.transform.parent;

                void SelectHoveredObj(MouseInteractable newObj)
                {
                    if (hoveredObj != null && hoveredObj != newObj) DeselectHoveredObj();
                    CursorSet.SetCursor(newObj.CursorType);
                    hoveredObj = newObj;
                    newObj.IsMouseOver = true;
                    hoverObjMousePos = hit.point;
                }

                if ((hitParent != null && hitParent.TryGetComponent(out MouseInteractable obj1))) SelectHoveredObj(obj1);
                else if (hit.transform.TryGetComponent(out MouseInteractable obj2)) SelectHoveredObj(obj2);
            }
            else if (hoveredObj != null)
            {
                DeselectHoveredObj();
            }
        }
        else
        {
            if (hoveredObj != null) DeselectHoveredObj();
        }
    }

    private void DeselectHoveredObj()
    {
        hoveredObj.LeftMouseDown = false;
        hoveredObj.RightMouseDown = false;
        hoveredObj.IsMouseOver = false;
        hoveredObj = null;
        CursorSet.SetCursor(CursorType.Normal);
    }

    public void RefreshViewportRect()
    {
        Rect viewportPixelsRect = EditorUIManager.Instance.ViewportRect;
        Rect canvasRect = FindFirstObjectByType<Canvas>().GetComponent<RectTransform>().rect;
        Vector2 bottomLeftPos = new(viewportPixelsRect.xMin / canvasRect.width, 1-(viewportPixelsRect.yMax / canvasRect.height));
        Vector2 topRightPos = new(viewportPixelsRect.xMax / canvasRect.width, 1-(viewportPixelsRect.yMin / canvasRect.height));
        viewportRect = new(bottomLeftPos, topRightPos - bottomLeftPos);
    }

    private bool InsideViewportOrWindow(Vector3 screenPos)
    {
        Vector3 viewportPos = cam.ScreenToViewportPoint(screenPos);
        return viewportRect.Contains(viewportPos); //or check for inside popup windows
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInp = ctx.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        lookInp = ctx.ReadValue<Vector2>();
    }

    public void OnLeftClick(InputAction.CallbackContext ctx)
    {
        void CancelAll()
        {
            potentialClickL = false;
            canLookL = false;
            if (hoveredObj != null) hoveredObj.LeftMouseDown = false;
        }

        if (!InsideViewportOrWindow(Input.mousePosition))
        {
            CancelAll();
            return;
        }

        if (ctx.performed)
        {
            potentialClickL = true;
            StartCoroutine(clickCheckL());
            canLookL = true;

            if (hoveredObj != null) 
            {
                hoveredObj.LeftMouseDown = true;
                hoveredObj.MouseDownPos = hoverObjMousePos;
            }
        }
        else if (ctx.canceled)
        {
            if (potentialClickL)
            {
                PrimaryClick(Input.mousePosition);
            }
            CancelAll();
        }
    }

    private void PrimaryClick(Vector3 pos)
    {
        //if (click was inside viewport)
        EditorUIManager.Instance.ClearPropertyPanel();
        EditorGizmoManager.DestroyAllGizmos();

        /*Ray r = cam.ScreenPointToRay(pos);
        if (Physics.Raycast(r, out RaycastHit hit))
        {
            if (hit.transform.parent.TryGetComponent(out MouseInteractable obj))
            {
                obj.OnLeftClick();
            }
        }*/
        if (hoveredObj != null) hoveredObj.OnLeftClick();
    }

    public void OnRightClick(InputAction.CallbackContext ctx)
    {
        void CancelAll()
        {
            potentialClickR = false;
            canLookR = false;
            if (hoveredObj != null) hoveredObj.RightMouseDown = false;
        }

        if (!InsideViewportOrWindow(Input.mousePosition))
        {
            CancelAll();
            return;
        }

        if (ctx.performed)
        {
            potentialClickR = true;
            StartCoroutine(clickCheckR());
            canLookR = true;

            if (hoveredObj != null) hoveredObj.RightMouseDown = true;
        }
        else if (ctx.canceled)
        {
            if (potentialClickR)
            {
                SecondaryClick(Input.mousePosition);
            }
            CancelAll();
        }
    }

    private void SecondaryClick(Vector3 pos)
    {
        /*Ray r = cam.ScreenPointToRay(pos);
        if (Physics.Raycast(r, out RaycastHit hit))
        {
            if (hit.transform.parent.TryGetComponent(out MouseInteractable obj))
            {
                obj.OnRightClick();
            }
        }
        else
        {
            //some right click action like one of those fancy dancy context menus
        }*/
        if (hoveredObj != null) hoveredObj.OnRightClick();
        else { }
    }

    public void OnScroll(InputAction.CallbackContext ctx)
    {
        if (!InsideViewportOrWindow(Input.mousePosition)) return;

        Vector2 scroll = ctx.ReadValue<Vector2>() * scrollSensitivity;

        if (cam.orthographic) //change orthogrphic size
        {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - scroll.y, 0.1f, 50);
        }
        else if (CanLook) //change movement speed
        {
            acceleration = Mathf.Clamp(acceleration + scroll.y, 0.25f, 15);
        }
        else //change fov
        {
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView + (scroll.y/scrollSensitivity), 30, 135);
        }
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) movingUp = true;
        else if (ctx.canceled) movingUp = false;
    }

    public void OnCrouch(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) movingDown = true;
        else if (ctx.canceled) movingDown = false;
    }

    public void OnTogglePerspective(InputAction.CallbackContext ctx)
    {
        if(ctx.performed && CanLook) cam.orthographic = !cam.orthographic;
    }

    public void OnMouseDelta(InputAction.CallbackContext ctx)
    {
        Vector2 v = ctx.ReadValue<Vector2>();
        if(v!=Vector2.zero && hoveredObj != null) hoveredObj.OnMouseMove(v); 
    }

    IEnumerator clickCheckL()
    {
        yield return new WaitForSecondsRealtime(clickLength);
        potentialClickL = false;
    }

    IEnumerator clickCheckR()
    {
        yield return new WaitForSecondsRealtime(clickLength);
        potentialClickR = false;
    }

    public void TeleportToLastSelectedObject()
    {
        TTObject tpObj;
        if (TTObject.LastSelectedObject == null)
        {
            var objs = FindObjectsByType<TTObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            tpObj = objs.Where(o=>o.FindProperty("Position")!=null).FirstOrDefault();
            if(tpObj == null)
            {
                EditorUIManager.Instance.Err("Couldn't find any TTObject with 'Position' property to teleport to.", null, "No Object Found");
                return;
            }
        } else tpObj = TTObject.LastSelectedObject;

        transform.position = tpObj.transform.position - cam.transform.forward*5;
    }
}
