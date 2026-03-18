using UnityEngine;
using SFB;
using KUtility;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using System.Linq;
using UnityEngine.Events;
using System.Threading.Tasks;

public class TTResourceManager : MonoBehaviour
{
    public Material defaultResourceMaterial;

    public static TTResourceManager Instance { get; private set; }

    [SerializeField]
    private string[] resourcePaths;

    [SerializeField]
    private ResourceIndex[] resourceIndicies;

    private static int animationCounter;
    private readonly Dictionary<string,ResourceAsset> assets = new();
    private readonly List<string> animatedAssets = new();

    private string tcs_path;
    private bool _resourcesLoaded;
    public static bool Loaded => Instance._resourcesLoaded;
    public static UnityEvent OnLoaded = new();

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Get the path of the lego star wars directory
        if(!Settings.TryGet("tcs_path", out tcs_path))
        {
            string[] paths = new string[0];
            while(paths.Length == 0) 
                paths = StandaloneFileBrowser.OpenFolderPanel("Select Lego Star Wars: TCS Folder (folder that contains .exe)", "", false);

            tcs_path = paths[0];
            Settings.Set("tcs_path", tcs_path);
            Settings.Save();
        }

        if (!Directory.Exists(tcs_path))
        {
            string driveErr = tcs_path.StartsWith("C") ? "":"any external drives needed, ";
            EditorUIManager.Instance.Err($"Path to Lego Star Wars ({tcs_path}) could not be found. Make sure the path is correct, {driveErr}or update the path in the settings.");
        }
        else
        {
            LoadResources();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        animationCounter++;
        foreach (var animAsset in animatedAssets) assets[animAsset].UpdateMaterial();
    }

    public void LoadResources()
    {
        StartCoroutine(LoadResourcesRoutine());
    }

    IEnumerator LoadResourcesRoutine()
    {
        EditorUIManager.Instance.ShowProgressBar("Loading Resources", "Loading resource images from Lego Star Wars");
        yield return null;

        int pathsDone = 0;

        foreach (string path in resourcePaths)
        {
            byte[] bytes = File.ReadAllBytes(Path.Combine(tcs_path, path));
            EditorUIManager.Instance.UpdateProgressBar(pathsDone / ((float)resourcePaths.Length), $"Counting resources in {path}...");
            yield return null;
            List<Bitmap> txtrs = new();
            /*yield return StartCoroutine(DDSImage.GetAllDDSFromGSCWithProgress(bytes, (bmp, progress) =>
            {
                txtrs.Add(bmp);
                EditorUIManager.Instance.UpdateProgressBar(((pathsDone+1)*progress) / ((float)resourcePaths.Length), $"Loading resources from {path}...");
            },10));*/
            float ddsProgress = 0;
            Task<Bitmap[]> ddsTask = DDSImage.GetAllDDSFromGSCWithProgressAsync(bytes, (progress) =>
            {
                ddsProgress = progress;
            });
            while (!ddsTask.IsCompleted)
            {
                yield return new WaitForSecondsRealtime(0.1f);
                EditorUIManager.Instance.UpdateProgressBar(((pathsDone + 1) * ddsProgress) / ((float)resourcePaths.Length), $"Loading resources from {path}...");
                yield return null;
            }
            txtrs.AddRange(ddsTask.Result);
            //Debug.Log("TXTRS COUNT " + txtrs.Count);

            var resourceIndex = resourceIndicies[pathsDone];
            int txtrInd = 0;
            for(int i=0; i<resourceIndex.values.Length; i++)
            {
                var indexVal = resourceIndex.values[i];
                assets.Add(indexVal.name, new ResourceAsset(indexVal, txtrs,ref txtrInd));
                if (indexVal.frameCount > 1) animatedAssets.Add(indexVal.name);
            }

            pathsDone++;
        }

        EditorUIManager.Instance.CloseProgressBar();
        _resourcesLoaded = true;
        OnLoaded.Invoke();

        EditorUIManager.Instance.ShowProgressBar("Resources Loaded!", "Testing new progress bar...");
        yield return null;
        for(int i=0; i<100; i++)
        {
            EditorUIManager.Instance.UpdateProgressBar(i / 100f, $"Testing new progress bar {i}/100...");
            yield return null;
        }
        EditorUIManager.Instance.CloseProgressBar();

        //TESTING
        /*ShowAllGeneratedAssets();
        Texture2D skyboxTxtr = GetMaterial("blue_sky2").mainTexture as Texture2D;
        Material skyboxMat = new(Shader.Find("Skybox/6 Sided"));
        skyboxMat.SetTexture("_FrontTex", skyboxTxtr);
        skyboxMat.SetTexture("_BackTex", skyboxTxtr);
        skyboxMat.SetTexture("_LeftTex", skyboxTxtr);
        skyboxMat.SetTexture("_RightTex", skyboxTxtr);
        skyboxMat.SetTexture("_UpTex", skyboxTxtr);
        skyboxMat.SetTexture("_DownTex", skyboxTxtr);
        skyboxMat.SetColor("_Tint", new(1,1,1,1));
        RenderSettings.skybox = skyboxMat;*/
        //CreateObject("test", PrimitiveType.Plane, Vector3.zero, Vector3.zero, Vector3.one * 0.05f, "force_particle", true, "test");
    }

    public static Material GetMaterial(string assetName)
    {
        if(Loaded) return Instance.assets[assetName].GetMaterial();
        EditorUIManager.Instance.Err($"Resources not loaded, cannot get material {assetName}");
        return Instance.defaultResourceMaterial;
    }

    public static GameObject CreateObject(string name, PrimitiveType type, Vector3 pos, Vector3 angle, Vector3 scale, string materialName, bool billboard = false, string label = "", bool collision=false)
    {
        GameObject obj = GameObject.CreatePrimitive(type);
        obj.name = name;
        obj.transform.SetPositionAndRotation(pos,Quaternion.Euler(angle));
        obj.transform.localScale = scale;

        if (billboard) obj.AddComponent<Billboard>();
        //if(!string.IsEmptyOrNull(label)) obj
        obj.GetComponent<Renderer>().material = GetMaterial(materialName);

        //!!! PRIMITIVES ALREADY COME WITH COLLIDER !!!
        if (collision)
        {
            switch (type)
            {
                case PrimitiveType.Sphere: obj.AddComponent<SphereCollider>(); break;
                case PrimitiveType.Cube: obj.AddComponent<BoxCollider>(); break;
                case PrimitiveType.Capsule: obj.AddComponent<CapsuleCollider>(); break;
                case PrimitiveType.Cylinder: obj.AddComponent<CapsuleCollider>(); break;
                default: obj.AddComponent<MeshCollider>(); break;
            }
        }

        return obj;
    }

    public static void ShowAllGeneratedAssets()
    {
        Vector3 pos = Vector3.zero;
        foreach(var pair in Instance.assets)
        {
            CreateObject(pair.Key, PrimitiveType.Plane, pos, Vector3.zero, Vector3.one*0.1f, pair.Key, true);
            pos += new Vector3(1.05f, 0, 0);
        }
    }

    [Serializable]
    public struct ResourceIndex
    {
        public ResourceIndexVal[] values;
    }

    [Serializable]
    public struct ResourceIndexVal
    {
        public string name;
        public int frameCount;
    }

    private struct ResourceAsset
    {
        public string name;
        public Texture2D[] textures;
        private readonly Material mat;

        public ResourceAsset(ResourceIndexVal resourceIndex, List<Bitmap> bmps, ref int index)
        {
            name = resourceIndex.name;
            textures = new Texture2D[resourceIndex.frameCount];
            for (int i = 0; i < textures.Length; i++) textures[i] = bmps[i + index].txtr;
            index += textures.Length;

            mat = new(Instance.defaultResourceMaterial) { mainTexture = textures[0], name = name };
        }

        public readonly void UpdateMaterial()
        {
            if (textures.Length < 2) return;
            mat.mainTexture = textures[animationCounter%textures.Length];
        }

        public readonly Material GetMaterial() => mat;
    }
}
