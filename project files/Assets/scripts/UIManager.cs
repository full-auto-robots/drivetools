using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// this script takes on 2nd highest-level (reports to ProgramManager.cs) UI-related tasks

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;

    public static UIManager Instance
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
    }

    public Transform t_canvas;

    public Transform t_toolbar;
    public Transform t_recordingBar;

    public NTKeyMenu keyMenu;
    public ConsoleMenu console;
    public SettingsMenu settingsMenu;
    public NewDisplayMenu newDisplayMenu;
    public ui_filebrowser fileBrowser;
    public GameObject g_toolbar;
    public GameObject g_infoPage;
    public GameObject g_setupMenu;
    public QuickSetup setupMenu;

    public SplashScreen splashScreen;

    // is debug view on? stuff like field active areas, docking debug
    public bool isDebugViewActive;

    public GameObject g_colorPickerMenu;

    void Start()
    {
        g_colorPickerMenu.transform.GetChild(0).gameObject.SetActive(false);
        g_infoPage.SetActive(false);
    }

    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }
    public void ToggleInfoPage()
    {
        g_infoPage.SetActive(!g_infoPage.activeSelf);
    }

    public void GetColorFromPicker(UnityAction<Color> toRunAfter)
    {
        g_colorPickerMenu.transform.GetChild(0).gameObject.SetActive(true);

        g_colorPickerMenu.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<ui_colorpicker>().onEndInteraction.RemoveAllListeners();
        g_colorPickerMenu.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<ui_colorpicker>().onEndInteraction.AddListener(toRunAfter);
    }

    public void RefreshDebugObjects()
    {
        if (isDebugViewActive)
        {
            TurnOnDebugObjects();
        }
        else { TurnOffDebugObjects(); }
    }

    public void TurnOffDebugObjects()
    {
        DebugComponent[] comps = t_canvas.GetComponentsInChildren<DebugComponent>(true);
        for (int i = 0; i < comps.Length; i++)
        {
            comps[i].Deactivate();
        }
    }

    public void TurnOnDebugObjects()
    {
        DebugComponent[] comps = t_canvas.GetComponentsInChildren<DebugComponent>(true);
        for (int i = 0; i < comps.Length; i++)
        {
            comps[i].Activate();
        }
    }

    void Update()
    {
        // this weird sort-of backtick character is the "debug view on/off" button, basically
        // see DebugComponent.cs for more info
        if (Input.GetKeyDown("`"))
        {
            isDebugViewActive = !isDebugViewActive;
            // temp
            if (isDebugViewActive)
            {
                TurnOnDebugObjects();
            }
            else
            {
                TurnOffDebugObjects();
            }
        }


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            fileBrowser.transform.GetChild(0).gameObject.SetActive(false);
            fileBrowser.whenSelectionConfirmed = null;
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown("n")) // new text object
            {
                // new display
                DisplayManager.Instance.UpdatePreviousLayout();
                DisplayManager.Instance.CreateNewDisplayObject();
            }

            // saving the current layout
            if (Input.GetKeyDown("l"))
            {
                DisplayManager.Instance.SaveCurrentLayoutToDisk();
            }

            if (Input.GetKeyDown("z"))
            {
                DisplayManager.Instance.LoadPreviousLayout();
            }

            if (Input.GetKeyDown("r"))
            {
                if (NetworkManager.Instance.isRecording) NetworkManager.Instance.StopRecording();
                else NetworkManager.Instance.StartRecording();
            }

            // using "t" as the toggle-toolbar-key
            if (Input.GetKeyDown("t"))
            {
                g_toolbar.SetActive(!g_toolbar.activeSelf);
            }

            // testing the alert system
            if (Input.GetKeyDown("y"))
            {
                AlertManager.Instance.FullscreenAlert(Time.time.ToString(), "WARNING:", -1);
            }
        }
    }

    public void TrySplashScreen()
    {
        //Debug.Log(rw_utils.prefs.showSplashScreen);
        if (rw_utils.prefs.showSplashScreen) splashScreen.Run();
    }

    // starting to really hate these repeating functions
    // it might be time for menuArray again lol
    // ***
    public void ToggleNTKeyMenu()
    {
        // keyMenu.transform.GetChild(0).gameObject.SetActive(!keyMenu.transform.GetChild(0).gameObject.activeSelf); // toggle
        // keyMenu.Refresh();
    }

    public void ToggleConsole()
    {
        console.transform.GetChild(0).gameObject.SetActive(!console.transform.GetChild(0).gameObject.activeSelf); // toggle
        console.Refresh();
    }

    public void ToggleSettings()
    {
        settingsMenu.transform.GetChild(0).gameObject.SetActive(!settingsMenu.transform.GetChild(0).gameObject.activeSelf); // toggle
        //console.Refresh();
    }
    public void ToggleNewDisplayMenu()
    {
        newDisplayMenu.transform.GetChild(0).gameObject.SetActive(!newDisplayMenu.transform.GetChild(0).gameObject.activeSelf); // toggle
        newDisplayMenu.Refresh();
    }
    // ***


    // usually we're not going to be using this function, because all it does is open the window
    // USUALLY the point is to SELECT a directory, in which case we use the select directory function
    public void OpenFileBrowser(string dir)
    {
        fileBrowser.transform.GetChild(0).gameObject.SetActive(true);
        fileBrowser.DrawDirectory(dir);
    }

    
    public void SelectDirectory(string defaultDir, string extension, UnityAction<string> afterSelect) // since there can't be an immediate return, I'm going to use a unity action
    {
        SelectDirectory(defaultDir, extension, new string[0], afterSelect, "");
    }
    public void SelectDirectory(string defaultDir, string extension, string[] recentFiles, UnityAction<string> afterSelect, string title)
    {
        fileBrowser.transform.GetChild(0).gameObject.SetActive(true);
        fileBrowser.DrawDirectory(defaultDir, extension, recentFiles, title);

        fileBrowser.whenSelectionConfirmed = afterSelect;
    }
}
