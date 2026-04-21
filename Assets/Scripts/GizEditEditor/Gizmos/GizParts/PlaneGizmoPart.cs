using System.Linq;
using UnityEngine;

public class PlaneGizmoPart : EditorGizmoPart<Vector2>
{
    public override CursorType CursorType => CursorType.Move;

    public Vector3 vector;
    public Color color;
    public float scale;

    private Vector3 P000, P001, P010, P011, P100, P101, P110, P111;
    private Vector3 hoverScalar;

    private Transform camTransform;
    static Material lineMaterial;
    static float thickness = 0.01f;

    static float hoverScale = 2f;

    private Vector3 dragStartHit, dragStartObjPos;
    private bool prevMouseDown = false;
    private Plane dragPlane = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        camTransform = Camera.main.transform;
        var collider = gameObject.AddComponent<BoxCollider>();
        Vector3 thicknessVec = new(thickness, thickness, thickness);
        Vector3 opp = Vector3.one - vector;
        collider.size = Vector3.Scale(2 * hoverScale * thicknessVec, opp) + (vector*scale);
        collider.center = (vector*scale) / 2;
        Vector3 min = Vector3.Scale(-thicknessVec, opp);
        Vector3 max = Vector3.Scale(thicknessVec, opp) + (vector*scale);
        P000 = new Vector3(min.x, min.y, min.z);
        P001 = new Vector3(min.x, min.y, max.z);
        P010 = new Vector3(min.x, max.y, min.z);
        P011 = new Vector3(min.x, max.y, max.z);
        P100 = new Vector3(max.x, min.y, min.z);
        P101 = new Vector3(max.x, min.y, max.z);
        P110 = new Vector3(max.x, max.y, min.z);
        P111 = new Vector3(max.x, max.y, max.z);

        hoverScalar = opp * hoverScale + vector;
    }

    private void Update()
    {
        transform.eulerAngles = Vector3.zero;
        if (!LeftMouseDown && prevMouseDown) prevMouseDown = false;
    }

    public void Set(Vector3 vec, Color col, float scale = 0.15f)
    {
        vector = vec;
        color = new(col.r, col.g, col.b, 0.2f);
        this.scale = scale;
    }

    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new(shader) { hideFlags = HideFlags.HideAndDontSave };
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);

            lineMaterial.SetInt("_ZWrite", 0);
            lineMaterial.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
        }
    }

    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(transform.localToWorldMatrix);

        //Draw arrow
        Vector3 scalar = IsMouseOver ? hoverScalar : Vector3.one;

        GL.Begin(GL.QUADS);
        GL.Color(color);

        //drawing arrow shaft
        GL.Vertex(Vector3.Scale(P000, scalar));
        GL.Vertex(Vector3.Scale(P010, scalar));
        GL.Vertex(Vector3.Scale(P110, scalar));
        GL.Vertex(Vector3.Scale(P100, scalar));

        GL.Vertex(Vector3.Scale(P100, scalar));
        GL.Vertex(Vector3.Scale(P110, scalar));
        GL.Vertex(Vector3.Scale(P111, scalar));
        GL.Vertex(Vector3.Scale(P101, scalar));

        GL.Vertex(Vector3.Scale(P101, scalar));
        GL.Vertex(Vector3.Scale(P111, scalar));
        GL.Vertex(Vector3.Scale(P011, scalar));
        GL.Vertex(Vector3.Scale(P001, scalar));

        GL.Vertex(Vector3.Scale(P001, scalar));
        GL.Vertex(Vector3.Scale(P011, scalar));
        GL.Vertex(Vector3.Scale(P010, scalar));
        GL.Vertex(Vector3.Scale(P000, scalar));

        GL.Vertex(Vector3.Scale(P010, scalar));
        GL.Vertex(Vector3.Scale(P110, scalar));
        GL.Vertex(Vector3.Scale(P111, scalar));
        GL.Vertex(Vector3.Scale(P011, scalar));

        GL.Vertex(Vector3.Scale(P000, scalar));
        GL.Vertex(Vector3.Scale(P100, scalar));
        GL.Vertex(Vector3.Scale(P101, scalar));
        GL.Vertex(Vector3.Scale(P001, scalar));

        GL.End();
        GL.PopMatrix();
    }

    public override void OnLeftDrag(Vector3 mouseDelta)
    {
        Vector3 objPos = transform.position;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!prevMouseDown)
        {
            Vector3 opp = Vector3.one - vector;
            dragPlane = new(opp,-Vector3.Scale(opp,objPos).magnitude);

            // get initial hit point to calculate offset
            if (dragPlane.Raycast(ray, out float enterDist))
            {
                dragStartHit = ray.GetPoint(enterDist);
                dragStartObjPos = objPos;
            }

            prevMouseDown = true;
            return;
        }

        // project mouse ray onto the same plane each frame
        if (dragPlane.Raycast(ray, out float dist))
        {
            Vector3 hitPoint = ray.GetPoint(dist);
            // get delta from initial hit, projected onto movement axis
            Vector3 delta = hitPoint - dragStartHit;
            Vector3 newVal = dragStartObjPos + delta;
            value = ToPlaneAxis(newVal);

            OnValueChange.Invoke(value);
        }
    }

    private Vector2 ToPlaneAxis(Vector3 input)
    {
        if (vector.x != 1) return new(input.y, input.z);
        if (vector.y != 1) return new(input.x, input.z);
        return new(input.x, input.y);
    }
}
