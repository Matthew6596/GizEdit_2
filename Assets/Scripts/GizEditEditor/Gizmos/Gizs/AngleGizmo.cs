using UnityEngine;

public class AngleGizmo : EditorGizmo
{
    public PlaneGizmoPart xzPlane;
    public float radius = 1.5f;

    private bool started = false;

    private Vector2 parentV2Pos => new(transform.parent.position.x, transform.parent.position.z);

    private void Start()
    {
        if(!started) StartGizmo();
    }

    private void StartGizmo()
    {
        float initAng = GetValue<float>();

        xzPlane = CreateGizmoPart<PlaneGizmoPart, Vector2>(Pos(initAng) + parentV2Pos);
        xzPlane.Set(new Vector3(1, 0, 1), Color.green);
        xzPlane.OnValueChange.AddListener((e) => { UpdatePos(e); });

        started = true;
    }

    private void Update()
    {
        
    }

    private void UpdatePos(Vector2 newPos)
    {
        Vector3 parentPos = transform.parent.position;
        Vector3 pos = new(newPos.x, parentPos.y, newPos.y);
        //xzPlane.transform.position = pos;

        float ang = -Mathf.Atan2(pos.z - parentPos.z, pos.x - parentPos.x) - Mathf.PI/2;
        //xzPlane.transform.position = new Vector3(Mathf.Cos(ang), 0, Mathf.Sin(ang)) * radius + parentPos;
        Value = ang * Mathf.Rad2Deg;
        OnValueChange.Invoke(Value);
    }

    private T1 CreateGizmoPart<T1,T2>(T2 value) where T1 : EditorGizmoPart<T2>
    {
        GameObject obj = new("editor_gizmo_part");
        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.back * radius;
        T1 gizPart = obj.AddComponent<T1>();
        gizPart.value = value;
        return gizPart;
    }

    public void RefreshValues()
    {
        if (!started) StartGizmo();
        float ang = (float)Value;
        xzPlane.value = Pos(ang) + parentV2Pos;
    }

    private Vector2 Pos(float ang) => new(Mathf.Cos(Mathf.Deg2Rad * ang) * radius, Mathf.Sin(Mathf.Deg2Rad * ang) * radius);

    private static Material lineMaterial;
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
        //GL.MultMatrix(transform.localToWorldMatrix);

        GL.Begin(GL.LINES);
        GL.Color(Color.green);

        //GL.Vertex(Vector3.zero);
        //GL.Vertex(xzPlane.transform.position-transform.position);
        GL.Vertex(transform.parent.position);
        GL.Vertex(xzPlane.transform.position);

        GL.End();
        GL.PopMatrix();
    }
}
