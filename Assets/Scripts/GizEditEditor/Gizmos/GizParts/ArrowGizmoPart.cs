using System.Linq;
using UnityEngine;

public class ArrowGizmoPart : EditorGizmoPart<float>
{
    public override CursorType CursorType => CursorType.Move;

    public Vector3 vector;
    public Color color;

    private Vector3 arrowCorner1, arrowCorner2, arrowCorner3, arrowCorner4, arrowPoint, arrowP1, arrowP2, arrowP3, arrowP4, startP1, startP2, startP3, startP4;
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
        collider.size = Vector3.Scale(2 * hoverScale * thicknessVec,opp) + vector;
        collider.center = vector / 2;
        Vector3 min = Vector3.Scale(-thicknessVec,opp);
        Vector3 max = Vector3.Scale(thicknessVec, opp)+vector;
        Vector3 P000, P001, P010, P011, P100, P101, P110, P111;
        P000 = new Vector3(min.x, min.y, min.z);
        P001 = new Vector3(min.x, min.y, max.z);
        P010 = new Vector3(min.x, max.y, min.z);
        P011 = new Vector3(min.x, max.y, max.z);
        P100 = new Vector3(max.x, min.y, min.z);
        P101 = new Vector3(max.x, min.y, max.z);
        P110 = new Vector3(max.x, max.y, min.z);
        P111 = new Vector3(max.x, max.y, max.z);
        var sortedDist = new Vector3[] { P000, P001, P010, P011, P100, P101, P110, P111 }.OrderBy(v=>v.sqrMagnitude).Reverse().ToArray();
        startP1 = sortedDist[4];
        startP2 = sortedDist[5];
        startP3 = sortedDist[6];
        startP4 = sortedDist[7];
        arrowCorner1 = sortedDist[0];
        arrowCorner2 = sortedDist[1];
        arrowCorner3 = sortedDist[2];
        arrowCorner4 = sortedDist[3];
        arrowPoint = (arrowCorner1 + arrowCorner2 + arrowCorner3 + arrowCorner4)/4;
        float arrowEdgeInset = 0.8f;
        arrowP1 = arrowCorner1 * arrowEdgeInset;
        arrowP2 = arrowCorner2 * arrowEdgeInset;
        arrowP3 = arrowCorner3 * arrowEdgeInset;
        arrowP4 = arrowCorner4 * arrowEdgeInset;
        hoverScalar = opp * hoverScale + (Vector3.one-opp);
        arrowCorner1 = Vector3.Scale(arrowP1,hoverScalar);
        arrowCorner2 = Vector3.Scale(arrowP2,hoverScalar);
        arrowCorner3 = Vector3.Scale(arrowP3,hoverScalar);
        arrowCorner4 = Vector3.Scale(arrowP4,hoverScalar);
    }

    private void Update()
    {
        transform.eulerAngles = Vector3.zero;
        if (!LeftMouseDown && prevMouseDown) prevMouseDown = false;
    }

    public void Set(Vector3 vec, Color col)
    {
        vector = vec;
        color = new(col.r, col.g, col.b, 0.5f);
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
        GL.Vertex(Vector3.Scale(startP3, scalar));
        GL.Vertex(Vector3.Scale(startP4, scalar));
        GL.Vertex(Vector3.Scale(startP1, scalar));
        GL.Vertex(Vector3.Scale(startP2, scalar));

        GL.Vertex(Vector3.Scale(arrowP1, scalar));
        GL.Vertex(Vector3.Scale(arrowP3, scalar));
        GL.Vertex(Vector3.Scale(startP3, scalar));
        GL.Vertex(Vector3.Scale(startP1, scalar));

        GL.Vertex(Vector3.Scale(startP2, scalar));
        GL.Vertex(Vector3.Scale(arrowP2, scalar));
        GL.Vertex(Vector3.Scale(arrowP1, scalar));
        GL.Vertex(Vector3.Scale(startP1, scalar));

        //GL.Vertex(Vector3.Scale(P101, scalar));
        //GL.Vertex(Vector3.Scale(P100, scalar));
        //GL.Vertex(Vector3.Scale(P110, scalar));
        //GL.Vertex(Vector3.Scale(P111, scalar));

        GL.Vertex(Vector3.Scale(startP4, scalar));
        GL.Vertex(Vector3.Scale(startP2, scalar));
        GL.Vertex(Vector3.Scale(arrowP2, scalar));
        GL.Vertex(Vector3.Scale(arrowP4, scalar));

        GL.Vertex(Vector3.Scale(arrowP3, scalar));
        GL.Vertex(Vector3.Scale(startP3, scalar));
        GL.Vertex(Vector3.Scale(startP4, scalar));
        GL.Vertex(Vector3.Scale(arrowP4, scalar));

        //connect shaft to corners
        GL.Vertex(Vector3.Scale(arrowP1, scalar));
        GL.Vertex(Vector3.Scale(arrowP2, scalar));
        GL.Vertex(Vector3.Scale(arrowCorner2, scalar));
        GL.Vertex(Vector3.Scale(arrowCorner1, scalar));

        GL.Vertex(Vector3.Scale(arrowP2, scalar));
        GL.Vertex(Vector3.Scale(arrowP3, scalar));
        GL.Vertex(Vector3.Scale(arrowCorner3, scalar));
        GL.Vertex(Vector3.Scale(arrowCorner2, scalar));

        GL.Vertex(Vector3.Scale(arrowP3, scalar));
        GL.Vertex(Vector3.Scale(arrowP4, scalar));
        GL.Vertex(Vector3.Scale(arrowCorner4, scalar));
        GL.Vertex(Vector3.Scale(arrowCorner3, scalar));

        GL.Vertex(Vector3.Scale(arrowP4, scalar));
        GL.Vertex(Vector3.Scale(arrowP1, scalar));
        GL.Vertex(Vector3.Scale(arrowCorner1, scalar));
        GL.Vertex(Vector3.Scale(arrowCorner4, scalar));

        GL.End();
        GL.Begin(GL.TRIANGLES);
        GL.Color(color);

        //Draw arrow tip
        GL.Vertex(Vector3.Scale(arrowCorner1, scalar));
        GL.Vertex(Vector3.Scale(arrowCorner3, scalar));
        GL.Vertex(Vector3.Scale(arrowPoint, scalar));

        GL.Vertex(Vector3.Scale(arrowCorner3, scalar));
        GL.Vertex(Vector3.Scale(arrowCorner2, scalar));
        GL.Vertex(Vector3.Scale(arrowPoint, scalar));

        GL.Vertex(Vector3.Scale(arrowCorner2, scalar));
        GL.Vertex(Vector3.Scale(arrowCorner4, scalar));
        GL.Vertex(Vector3.Scale(arrowPoint, scalar));

        GL.Vertex(Vector3.Scale(arrowCorner4, scalar));
        GL.Vertex(Vector3.Scale(arrowCorner1, scalar));
        GL.Vertex(Vector3.Scale(arrowPoint, scalar));

        GL.End();
        GL.PopMatrix();
    }

    public override void OnLeftDrag(Vector3 mouseDelta)
    {
        Vector3 objPos = transform.position;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!prevMouseDown)
        {
            // build a plane through the click point
            // normal faces camera but projected perpendicular to movement axis
            Vector3 camDir = (camTransform.position - objPos).normalized;
            // remove the movement axis component from camDir to get plane normal
            Vector3 planeNormal = camDir - Vector3.Dot(camDir, vector) * vector;

            // fallback if camera is looking straight down the axis
            if (planeNormal.sqrMagnitude < 0.001f)
                planeNormal = camTransform.up;
            else
                planeNormal = planeNormal.normalized;

            dragPlane = new Plane(planeNormal, objPos);

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
            float axisDelta = Vector3.Dot(delta, vector);
            value = Vector3.Dot(dragStartObjPos, vector) + axisDelta;
            OnValueChange.Invoke(value);
        }
    }
}
