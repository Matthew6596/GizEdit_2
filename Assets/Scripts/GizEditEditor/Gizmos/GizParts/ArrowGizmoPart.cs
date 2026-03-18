using UnityEngine;

public class ArrowGizmoPart : EditorGizmoPart<float>
{
    public override CursorType CursorType => CursorType.Move;

    public Vector3 vector;
    public Color color;

    private Vector3 P000, P001, P010, P011, P100, P101, P110, P111;
    private Vector3 hoverScalar;

    private Transform camTransform;
    static Material lineMaterial;
    static float thickness = 0.01f;

    static float hoverScale = 2f;

    private Vector3 offsetPos;
    //private Vector3 mouseDownPos;
    private bool prevMouseDown = false;

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
        P000 = new Vector3(min.x, min.y, min.z);
        P001 = new Vector3(min.x, min.y, max.z);
        P010 = new Vector3(min.x, max.y, min.z);
        P011 = new Vector3(min.x, max.y, max.z);
        P100 = new Vector3(max.x, min.y, min.z);
        P101 = new Vector3(max.x, min.y, max.z);
        P110 = new Vector3(max.x, max.y, min.z);
        P111 = new Vector3(max.x, max.y, max.z);
        hoverScalar = opp * hoverScale + (Vector3.one-opp);
    }

    private void Update()
    {
        if (!LeftMouseDown && prevMouseDown) prevMouseDown = false;
    }

    public void Set(Vector3 vec, Color col)
    {
        vector = vec;
        color = col;
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
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
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

        GL.Vertex(Vector3.Scale(P010, scalar));
        GL.Vertex(Vector3.Scale(P011, scalar));
        GL.Vertex(Vector3.Scale(P001, scalar));
        GL.Vertex(Vector3.Scale(P000, scalar));

        GL.Vertex(Vector3.Scale(P100, scalar));
        GL.Vertex(Vector3.Scale(P110, scalar));
        GL.Vertex(Vector3.Scale(P010, scalar));
        GL.Vertex(Vector3.Scale(P000, scalar));

        GL.Vertex(Vector3.Scale(P001, scalar));
        GL.Vertex(Vector3.Scale(P101, scalar));
        GL.Vertex(Vector3.Scale(P100, scalar));
        GL.Vertex(Vector3.Scale(P000, scalar));

        GL.Vertex(Vector3.Scale(P101, scalar));
        GL.Vertex(Vector3.Scale(P100, scalar));
        GL.Vertex(Vector3.Scale(P110, scalar));
        GL.Vertex(Vector3.Scale(P111, scalar));

        GL.Vertex(Vector3.Scale(P011, scalar));
        GL.Vertex(Vector3.Scale(P001, scalar));
        GL.Vertex(Vector3.Scale(P101, scalar));
        GL.Vertex(Vector3.Scale(P111, scalar));

        GL.Vertex(Vector3.Scale(P110, scalar));
        GL.Vertex(Vector3.Scale(P010, scalar));
        GL.Vertex(Vector3.Scale(P011, scalar));
        GL.Vertex(Vector3.Scale(P111, scalar));

        /*else //draw arrow
        {
            GL.Begin(GL.LINES);
            GL.Color(color);

            GL.Vertex3(0, 0, 0);
            GL.Vertex(vector);
        }*/

        GL.End();
        GL.Begin(GL.TRIANGLES);

        //Draw arrow tip

        GL.End();
        GL.PopMatrix();
    }

    public override void OnLeftDrag(Vector3 mouseDelta)
    {
        //calculate plane about axis facing camera
        //euler rotation = rotation * axis (if y, rotate on Z, x=-90)
        Vector3 camPos = camTransform.position;
        Vector3 clickPos = MouseDownPos;
        Vector3 objPos = transform.position;
        if (!prevMouseDown)
        {
            offsetPos = objPos - clickPos;
            prevMouseDown = true;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 newNorm = ray.direction.normalized;
        Vector3 opp = Vector3.one - vector;

        //1. calculate opp distance
        float d = Vector3.Scale(camPos - clickPos,opp).magnitude;

        //2. get opp coords
        Vector3 tempPoint = Vector3.Scale(newNorm, opp) * d;

        //3. get distance scale
        float a = (opp.x == 0) ? (tempPoint.y / newNorm.y) : (tempPoint.x / newNorm.x);

        //4. get final intersection point
        Vector3 p = tempPoint + (Vector3.Scale(newNorm, vector) * a) + camPos;

        //Debug.Log($"drag info: norm: {newNorm}, opp: {opp}, d: {d}, temp: {tempPoint}, a: {a}, camPos: {camPos}, clickPos: {clickPos}, point: {p}, oldval: {value}, val:{Vector3.Scale(p, vector).magnitude}");
        Vector3 pv = Vector3.Scale(p+offsetPos, vector);
        value = pv.x + pv.y + pv.z; //gets only the value on the axis w/out ifs
        OnValueChange.Invoke(value);

        MouseDownPos = pv;

        //strategy 2
        /*Plane p1 = new(Vector3.right, transform.position.x);
        Plane p2 = new(Vector3.up, transform.position.y);
        Plane p3 = new(Vector3.forward, transform.position.z);
        float smallestDist = float.MaxValue;
        if (opp.x==1&&p1.Raycast(ray, out float f1)) smallestDist = f1; 
        if (opp.y==1&&p2.Raycast(ray, out float f2) && f2 < smallestDist) smallestDist = f2;
        if (opp.z==1&&p3.Raycast(ray, out float f3) && f3 < smallestDist) smallestDist = f3;
        Vector3 newPoint = ray.GetPoint(smallestDist);
        value = Vector3.Scale(newPoint, vector).magnitude; //gets just the value on the axis
        OnValueChange.Invoke(value);*/
    }
}
