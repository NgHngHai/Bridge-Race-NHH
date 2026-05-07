using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [Header("Animation")]
    [SerializeField] private Animator transitionAnimator;

    private Dictionary<System.Type, UICanvas> canvasesActive = new Dictionary<System.Type, UICanvas>();
    private Dictionary<System.Type, UICanvas> canvasPrefabs = new Dictionary<System.Type, UICanvas>();

    [SerializeField] private Transform parent;
    [SerializeField] UICanvas[] prefabs;

    private void Awake()
    {
        // load UI prefabs from Resources/UI folder
        prefabs = Resources.LoadAll<UICanvas>("UI/");
        for (int i = 0; i < prefabs.Length; i++)
        {
            canvasPrefabs.Add(prefabs[i].GetType(), prefabs[i]);
        }
    }

    // open canvas directly
    public T OpenUI<T>() where T : UICanvas
    {
        T canvas = GetUI<T>();

        canvas.Setup();
        canvas.OpenDirectly();

        return canvas;
    }

    // close canvas after time
    public void CloseUI<T>(float time) where T : UICanvas
    {
        if (IsUILoaded<T>())
        {
            canvasesActive[typeof(T)].Close(time);
        }
    }

    // close canvas immediately
    public void CloseUIDirectly<T>() where T : UICanvas
    {
        if (IsUILoaded<T>())
        {
            canvasesActive[typeof(T)].CloseDirectly();
        }
    }

    // check if canvas is loaded
    public bool IsUILoaded<T>() where T : UICanvas
    {
        return canvasesActive.ContainsKey(typeof(T)) && canvasesActive[typeof(T)] != null;
    }

    // check if canvas is opened
    public bool IsUIOpened<T>() where T : UICanvas
    {
        return IsUILoaded<T>() && canvasesActive[typeof(T)].gameObject.activeSelf;
    }

    // get active canvas
    public T GetUI<T>() where T : UICanvas
    {
        if (!IsUILoaded<T>())
        {
            T prefab = GetUIPrefab<T>();
            T canvas = Instantiate(prefab, parent);

            canvasesActive.Add(typeof(T), canvas);
        }

        return canvasesActive[typeof(T)] as T;
    }

    private T GetUIPrefab<T>() where T : UICanvas
    {
        return canvasPrefabs[typeof(T)] as T;
    }

    // close all canvases
    public void CloseAllUI()
    {
        foreach (var canvas in canvasesActive.Values)
        {
            if (canvas != null && canvas.gameObject.activeSelf)
            {
                canvas.CloseDirectly();
            }
        }
    }
}
