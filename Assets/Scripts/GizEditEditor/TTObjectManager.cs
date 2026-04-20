using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TTObjectManager : MonoBehaviour
{
    public static TTObjectManager Instance { get; private set; }

    public GameObject[] TTObjectPrefabs;

    private readonly static List<UnityEvent> onPropertyInitialization = new();
    private static int propertyInitializationPriority = 0;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void InitializeAllProperties()
    {
        for (int i = 0; i < onPropertyInitialization.Count; i++)
        {
            onPropertyInitialization[i].Invoke();
            onPropertyInitialization[i].RemoveAllListeners();
        }
        onPropertyInitialization.Clear();
    }

    public static void AddPropertyInitializationListener(UnityAction action)
    {
        while (onPropertyInitialization.Count <= propertyInitializationPriority) onPropertyInitialization.Add(new());
        onPropertyInitialization[propertyInitializationPriority].AddListener(action);
    }

    public static void LowerPropertyInitializationPriority(int amt=1) => propertyInitializationPriority+=amt; //higher values get invoked later
    public static void IncreasePropertyInitializationPriority(int amt=1) => propertyInitializationPriority-=amt; //lower values get invoked sooner

    public static T Create<T>(string objectName) where T : TTObject
    {
        GameObject prefab = Instance.TTObjectPrefabs.Where((obj) => obj.name == objectName).FirstOrDefault();
        T ttobj = null;
        if (prefab == null)
        {
            //Debug.LogWarning($"Failed to find prefab for '{objectName}' in TTObjectManager");
            GameObject obj = new();
            ttobj = obj.AddComponent<T>();
        }
        else ttobj = Instantiate(prefab).GetComponent<T>();

        ttobj.name = objectName;
        ttobj.InitStaticProperties();

        return ttobj;
    }

    public static void UnloadAll()
    {
        foreach(var fileObj in GameObject.FindObjectsByType<TTFileObject>(FindObjectsInactive.Include, FindObjectsSortMode.None)) fileObj.Destroy();
        EditorUIManager.Instance.RefreshHierarchy();
    }
}
