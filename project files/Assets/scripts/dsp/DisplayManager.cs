using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public enum DisplayType
{
    Text,
    Field2D,
    Field3D,
    Button,
    Compass,
    Image,
}

// 3rd level script, here's where display creation logic and such happens
public class DisplayManager : MonoBehaviour
{
    private static DisplayManager _instance;

    public static DisplayManager Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
            {
                _instance = value;
            }
            else if (_instance != value)
            {
                Debug.Log("Duplicate NetworkManager instance in scene!");
                Destroy(value);
            }
        }
    }

    private void Awake()
    {
        Instance = this;
        UnlockDisplays();
        g_emailMenu.SetActive(false);
        g_displayEditor.SetActive(false);
    }

    void Start()
    {
        if (MarkerMenu.Instance.currentFieldMarkers.Length == 0) { MarkerMenu.Instance.UseDefaultMarkers(); }
    }

    [Header("GENERAL REFS")]
    public Transform t_activeDisplayContainer;
    public Transform t_nodeOptions;
    public Transform t_bg;

    public GameObject g_displayEditor;
    public List<GameObject> activeDisplayObjects;
    public List<dsp_layout> previousLayouts; // used for un-doing

    public Sprite point_sprite;

    [Header("DISPLAY PREFABS")]
    public GameObject p_field2d;
    public GameObject p_field3d;
    public GameObject p_button;

    // referenced by field data classes
    public Sprite[] markerImages;

    public GameObject g_3dcam;
    public GameObject g_3dField;

    public bool areDisplaysLocked;
    public Image i_blockButton;
    public Sprite lockedSprite;
    public Sprite unlockedSprite;

    public GameObject g_emailMenu;

    public void ToggleDisplayLock()
    {
        if (areDisplaysLocked)
        {
            UnlockDisplays();
        }
        else
        {
            LockDisplays();
        }
    }

    public void OpenEmailMenu()
    {
        g_emailMenu.SetActive(true);
    }

    public void LockDisplays()
    {
        areDisplaysLocked = true;
        i_blockButton.sprite = lockedSprite;
    }
    public void UnlockDisplays()
    {
        areDisplaysLocked = false;
        i_blockButton.sprite = unlockedSprite;
    }

    // just like in drivetools V1, there aren't any buttons on nodes for deleting/editing them
    // instead, we have a nice little right-click menu
    public void OpenNodeOptions(dsp_mainvisual obj)
    {
        t_nodeOptions.position = Input.mousePosition;
        t_nodeOptions.gameObject.SetActive(true);
        t_nodeOptions.GetComponent<DisplayOptionssMenu>().isActive = true;
        t_nodeOptions.GetComponent<DisplayOptionssMenu>().Load(obj);
    }

    public int CountDisplaysOfType(ushort type)
    {
        int count = 0;
        for (int i = 0; i < activeDisplayObjects.Count; i++)
        {
            if (ushort.Parse(activeDisplayObjects[i].name[1].ToString()) == type)
            {
                count++;
            }
        }
        return count;
    }

    public void Check3DRequirement()
    {
        bool found = false;
        for (int i = 0; i < activeDisplayObjects.Count; i++)
        {
            if (activeDisplayObjects[i].transform.GetChild(0).GetComponentInChildren<dsp_field3d>() != null)
            {
                found = true;
            }
        }

        if (found)
        {
            Enable3D();
        }
        else
        {
            Disable3D();
        }
    }

    /*
    * we really don't want to render the 3D stuff if we don't have to (for performance reasons)
    * so we only do as necessary
    */
    public void Enable3D()
    {
        g_3dcam.SetActive(true);
        g_3dField.SetActive(true);
    }
    public void Disable3D()
    {
        g_3dcam.SetActive(false);
        g_3dField.SetActive(false);
    }

    public dsp_layout GetCurrentDisplayLayout()
    {
        dsp_layout result = new dsp_layout();
        result.displays = new List<dsp_genericdata>();

        for (int i = 0; i < activeDisplayObjects.Count; i++)
        {
            dsp_genericdata displayData = new dsp_genericdata();

            displayData.posX = activeDisplayObjects[i].transform.position.x;
            displayData.posY = activeDisplayObjects[i].transform.position.y;

            displayData.sizeX = activeDisplayObjects[i].GetComponent<RectTransform>().sizeDelta.x;
            displayData.sizeY = activeDisplayObjects[i].GetComponent<RectTransform>().sizeDelta.y;

            displayData.type = ushort.Parse(activeDisplayObjects[i].name[1].ToString());

            dsp_mainvisual comp = activeDisplayObjects[i].GetComponent<dsp_mainvisual>();
            displayData.ntKey = comp.ntKey;

            if (activeDisplayObjects[i].GetComponent<dsp_mainvisual>().propertyNames != null)
            {
                // copying over the property names/values (custom settings, basically) from the main visual
                for (int j = 0; j < activeDisplayObjects[i].GetComponent<dsp_mainvisual>().propertyNames.Count; j++)
                {
                    displayData.propertyNames.Add(comp.propertyNames[j]);
                    displayData.propertyValues.Add(comp.propertyValues[j]);
                }
            }

            result.displays.Add(displayData);
        }

        result.toolBarPositionX = UIManager.Instance.t_toolbar.position.x;
        result.toolBarPositionY = UIManager.Instance.t_toolbar.position.y;
        result.toolbarMode = UIManager.Instance.g_toolbar.GetComponent<ToolbarMenu>().parent.state;

        result.networkTables = NetworkManager.Instance.PackageFlaggedNTValues();

        result.fieldMarkers = MarkerMenu.Instance.ConvertMarkersToBytes();

        return result;
    }

    public void UpdatePreviousLayout()
    {
        if (previousLayouts == null) previousLayouts = new List<dsp_layout>();
        previousLayouts.Add(GetCurrentDisplayLayout());
    }

    // deletes all gameobjects and resets the list
    // doesn't save anything
    public void ClearAllDisplayObjects()
    {
        for (int i = activeDisplayObjects.Count - 1; i >= 0; i--)
        {
            Destroy(activeDisplayObjects[i]); //
        }
        activeDisplayObjects.Clear();
    }

    public void ForceSaveCurrentLayoutToDisk()
    {
        dsp_layout currentLayout = GetCurrentDisplayLayout();

        rw_utils.SaveLayout(currentLayout, rw_utils.prefs.workingLayoutSaveDirectory, "layout");
        
    }

    public void SaveCurrentLayoutToDisk()
    {
        dsp_layout currentLayout = GetCurrentDisplayLayout();

        UIManager.Instance.SelectDirectory(rw_utils.RemoveTrailingSlash(rw_utils.prefs.workingLayoutSaveDirectory), ".bbb", new string[0], (x) =>
        {
            rw_utils.SaveLayout(currentLayout, x + "/", "layout");
        }, "Save Layout:");
        
    }

    public void LoadLayoutFromDisk()
    {
        UIManager.Instance.SelectDirectory(rw_utils.RemoveTrailingSlash(rw_utils.prefs.workingLayoutLoadDirectory), ".lyt", rw_utils.prefs.recentLayoutDirectories.ToArray(), (x) => LoadLayoutFromDirectory(x), "Select Layout:");
    }

    public void LoadLayoutFromDirectory(string dir)
    {
        rw_utils.prefs.workingLayoutLoadDirectory = dir;
        if (!rw_utils.prefs.recentLayoutDirectories.Contains(dir))
        {
            rw_utils.prefs.recentLayoutDirectories.Add(dir);
        }
        rw_utils.prefs.previousLoadDirectory = dir;
        LoadDisplayLayout(rw_utils.LoadLayout(dir));
    }

    public void LoadDisplayLayout(dsp_layout data)
    {
        if (data == null) { return; }
        // make sure there are no existing display objects loaded
        ClearAllDisplayObjects();

        for (int i = 0; i < data.displays.Count; i++)
        {
            CreateNewDisplayObject(data.displays[i].type, data.displays[i].posX, data.displays[i].posY, data.displays[i].sizeX, data.displays[i].sizeY, data.displays[i].ntKey, data.displays[i].propertyNames, data.displays[i].propertyValues);
        }

        UIManager.Instance.t_toolbar.position = new Vector2(data.toolBarPositionX, data.toolBarPositionY);
        UIManager.Instance.g_toolbar.GetComponent<ToolbarMenu>().parent.state = data.toolbarMode;

        NetworkManager.Instance.flaggedKeys.Clear();
        if (data.networkTables != null)
        {
            for (int i = 0; i < data.networkTables.dataKeys.Length; i++)
            {
                NetworkManager.Instance.SetValueAt(data.networkTables.dataKeys[i], data.networkTables.dataValues[i]);
                NetworkManager.Instance.flaggedKeys.Add(data.networkTables.dataKeys[i]);
            }
        }

        if (data.fieldMarkers != null)
        {
            if (data.fieldMarkers.Count > 10)
            {
                MarkerMenu.Instance.UseMarkers(MarkerMenu.Instance.ConvertMarkersToTextures(data.fieldMarkers));
            }
            else
            {
                MarkerMenu.Instance.UseDefaultMarkers();
            }
        }
        else
        {
            MarkerMenu.Instance.UseDefaultMarkers();
        }
    }

    public void CreateNewDisplayObject() // just a shortcut
    {
        CreateNewDisplayObject(0);
    }

    public void DeleteDisplayObject(dsp_mainvisual obj)
    {
        Destroy(obj.gameObject);
        activeDisplayObjects.Remove(obj.gameObject);
        Check3DRequirement();
    }

    public void EditDisplay(dsp_mainvisual obj)
    {
        g_displayEditor.SetActive(true);
        g_displayEditor.transform.parent.GetComponent<DisplayEditingMenu>().Load(obj);
    }

    public void CreateNewDisplayObject(ushort type)
    {
        CreateNewDisplayObject(type, -1, -1, -1, -1); // -1 means that it doesn't matter and the default size should be used
    }
    public void CreateNewDisplayObject(ushort type, string key)
    {
        CreateNewDisplayObject(type, -1, -1, -1, -1, key, null, null);
    }

    public void CreateNewDisplayObject(ushort type, double xPos, double yPos, double xSize, double ySize)
    {
        CreateNewDisplayObject(type, xPos, yPos, xSize, ySize, "", null, null);
    }

    public void CreateNewDisplayObject(ushort type, double xPos, double yPos, double xSize, double ySize, string key, List<string> props, List<string> vals)
    {
        ushort newObjectType = type;

        string actualKey = string.IsNullOrEmpty(key) ? "null" : key;

        // first, the basics
        GameObject g_newDisplay = new GameObject();
        g_newDisplay.transform.SetParent(t_activeDisplayContainer);
        activeDisplayObjects.Add(g_newDisplay);

        // I'll make it a text node by default
        g_newDisplay.name = "t" + ((int)newObjectType).ToString();

        g_newDisplay.transform.localPosition = Vector3.zero;

        // now the visual/mechanical stuff
        g_newDisplay.AddComponent<Image>();
        g_newDisplay.GetComponent<Image>().color = Color.black;
        g_newDisplay.AddComponent<dsp_mainvisual>();
        if (props != null)
        {
            g_newDisplay.GetComponent<dsp_mainvisual>().propertyNames = props;
            g_newDisplay.GetComponent<dsp_mainvisual>().propertyValues = vals;
        }
        g_newDisplay.AddComponent<dsp_pointhandler>();

        // we need to add the corners now
        // nodes work a little differently than they did back in the days of drivetools V1, 
        // now the corners essentially run the show and everything else is positioned to fit them
        for (int i = 0; i < 9; i++)
        {
            RectTransform t_newCorner = new GameObject().AddComponent<RectTransform>();
            t_newCorner.SetParent(g_newDisplay.transform);
            t_newCorner.gameObject.name = i.ToString();

            t_newCorner.gameObject.AddComponent<Image>();
            t_newCorner.gameObject.GetComponent<Image>().sprite = point_sprite;
            t_newCorner.gameObject.AddComponent<dsp_point>().Initialize();
        }

        g_newDisplay.GetComponent<dsp_pointhandler>().Initialize();

        if (xSize != -1)
        {
            g_newDisplay.GetComponent<dsp_pointhandler>().positions[5] = new Vector3((float)(xPos - xSize / 2f), (float)(yPos + ySize / 2f), 0);
            g_newDisplay.GetComponent<dsp_pointhandler>().positions[6] = new Vector3((float)(xPos + xSize / 2f), (float)(yPos + ySize / 2f), 0);
            g_newDisplay.GetComponent<dsp_pointhandler>().positions[7] = new Vector3((float)(xPos - xSize / 2f), (float)(yPos - ySize / 2f), 0);
            g_newDisplay.GetComponent<dsp_pointhandler>().positions[8] = new Vector3((float)(xPos + xSize / 2f), (float)(yPos - ySize / 2f), 0);

            g_newDisplay.GetComponent<dsp_pointhandler>().DistributePoints();
        }

        // this has to come AFTER, because any visual components may NOT occupy the first 9 child indices

        // ***
        // TODO: move this into the field class itself
        // ***

        if (newObjectType == (ushort)DisplayType.Text)
        {
            GameObject g_text = new GameObject();
            TextMeshProUGUI comp = g_text.AddComponent<TextMeshProUGUI>();
            comp.alignment = TextAlignmentOptions.Center;

            g_text.transform.SetParent(g_newDisplay.transform);
            g_text.transform.localPosition = Vector3.zero;

            g_text.name = "text component";

            // *** text stuff ***
            comp.text = "null";
            comp.color = Color.white;

            g_newDisplay.AddComponent<dsp_text>().ntKey = actualKey;
            g_newDisplay.GetComponent<dsp_mainvisual>().ntKey = actualKey;

            g_newDisplay.GetComponent<dsp_text>().textComponent = comp;
        }
        else if (newObjectType == (ushort)DisplayType.Field2D)
        {
            // liiiitle more complex, this one
            GameObject g_fieldDisplay = Instantiate(p_field2d, Vector3.zero, Quaternion.identity);

            g_fieldDisplay.transform.SetParent(g_newDisplay.transform);

            g_fieldDisplay.transform.localPosition = Vector3.zero;

            g_fieldDisplay.GetComponent<dsp_field>().Initialize();
            // this part with the key really is just copy+paste, could be better
            g_fieldDisplay.GetComponent<dsp_field>().ntKey = actualKey;
            g_newDisplay.GetComponent<dsp_mainvisual>().ntKey = actualKey;
        }
        else if (newObjectType == (ushort)DisplayType.Field3D)
        {
            Enable3D();

            GameObject g_fieldDisplay = Instantiate(p_field3d, Vector3.zero, Quaternion.identity);
            g_fieldDisplay.transform.SetParent(g_newDisplay.transform);

            g_fieldDisplay.transform.localPosition = Vector3.zero;

            g_fieldDisplay.GetComponent<dsp_field3d>().Initialize();
            g_fieldDisplay.GetComponent<dsp_field3d>().ntKey = actualKey;
            g_newDisplay.GetComponent<dsp_mainvisual>().ntKey = actualKey;
        }
        else if (newObjectType == (ushort)DisplayType.Button)
        {
            GameObject g_buttonDisplay = Instantiate(p_button, g_newDisplay.transform);
            g_buttonDisplay.transform.localPosition = Vector3.zero;

            g_buttonDisplay.GetComponent<dsp_button>().Initialize();

            // just so we know there's SUPPOSED to be no data key here
            g_buttonDisplay.GetComponent<dsp_mainvisual>().ntKey = "$null";
            g_buttonDisplay.GetComponent<dsp_button>().commandName = "button1"; // default name
        }
        else if (newObjectType == (ushort)DisplayType.Image)
        {
            GameObject g_image = new GameObject();
            RawImage comp = g_image.AddComponent<RawImage>();

            g_image.transform.SetParent(g_newDisplay.transform);
            g_image.transform.localPosition = Vector3.zero;

            g_image.name = "image component";

            // *** image stuff ***
            comp.color = Color.white;

            g_newDisplay.AddComponent<dsp_image>().ntKey = actualKey;
            g_newDisplay.GetComponent<dsp_mainvisual>().ntKey = actualKey;

            g_newDisplay.GetComponent<dsp_image>().imageComponent = comp;

            g_newDisplay.GetComponent<dsp_image>().LoadAnyImages();
        }

        UIManager.Instance.RefreshDebugObjects();
    }

    public void LoadPreviousLayout()
    {
        if (previousLayouts.Count > 0)
        {
            LoadDisplayLayout(previousLayouts[previousLayouts.Count - 1]);
            if (previousLayouts.Count > 0)
            {
                previousLayouts.RemoveAt(previousLayouts.Count - 1);
            }
        }
    }

    void Update()
    {
        // interact, position, update, render

        for (int i = 0; i < activeDisplayObjects.Count; i++)
        {
            for (int c = 0; c < 9; c++)
            {
                activeDisplayObjects[i].transform.GetChild(c).GetComponent<dsp_point>().HandleInteract();
            }
            activeDisplayObjects[i].GetComponent<dsp_pointhandler>().HandlePositions();
        }

        // so the points are handled, but intersections still may remain
        // now we deal with them
        for (int i = 0; i < activeDisplayObjects.Count; i++)
        {
            activeDisplayObjects[i].GetComponent<dsp_pointhandler>().HandleIntersects(activeDisplayObjects);
        }

        for (int i = 0; i < activeDisplayObjects.Count; i++)
        {
            if (activeDisplayObjects[i].GetComponent<dsp_mainvisual>().updateAction != null) activeDisplayObjects[i].GetComponent<dsp_mainvisual>().updateAction.Invoke(NetworkManager.Instance.latestData);

            dsp_pointhandler comp = activeDisplayObjects[i].GetComponent<dsp_pointhandler>();
            activeDisplayObjects[i].GetComponent<dsp_mainvisual>().SetPoints(comp.positions[7], comp.positions[8], comp.positions[5], comp.positions[6]);
        }
    }
}
