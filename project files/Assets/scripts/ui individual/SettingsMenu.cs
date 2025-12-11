using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public ui_tabs selectionTabs;
    public Transform t_panelContainer;

    public float scrollSpeed;


    public TextMeshProUGUI tx_layoutDir;
    public TextMeshProUGUI tx_recordingDir;

    public TMP_InputField in_teamNumber;
    public TMP_InputField in_serverIP;
    public TMP_InputField in_tableName;
    public TMP_Dropdown in_ntProtocol;

    public ui_checkbox splashScreenCheckbox;

    void Start()
    {
        selectionTabs.InitializeFromChildren(t_panelContainer);

        for (int i = 0; i < selectionTabs.transform.childCount; i++)
        {
            int j = i;
            selectionTabs.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(() => OpenTab(j));
        }

        OpenTab(0);
        TryLoadPreferences();
        UIManager.Instance.TrySplashScreen();
    }

    public void ShowSetupMenu()
    {
        UIManager.Instance.g_setupMenu.SetActive(true);
    }
    public void ShowWarningMenu()
    {
        UIManager.Instance.g_warningMenu.SetActive(true);
    }

    public void ConfirmSetupMenu()
    {
        int num = 0;
        string rawNum;
        if (int.TryParse(UIManager.Instance.setupMenu.teamNumberInput.text, out num))
        {
            rw_utils.prefs.teamNumber = num;
            rawNum = UIManager.Instance.setupMenu.teamNumberInput.text;
            
            rw_utils.prefs.serverIP = "10." + rawNum.Substring(0, 2) + "." + rawNum.Substring(2, 2) + ".2";
        }  
        
        rw_utils.prefs.ntProtocol = (ushort)(UIManager.Instance.setupMenu.networkProtocolInput.value);

        if (UIManager.Instance.setupMenu.autoSetInput.value == 0)
        {
            // trying to make the FRC Driverstation think we're the default
            rw_utils.ModifyTxtWithNoSurprises(
                "C:/Users/Public/Documents/FRC/FRC DS Data Storage.ini",
                3,
                rw_utils.RemoveLastDirectory("DashboardCmdLine = " + Application.dataPath) + "/far_drivetools.exe");
        }
    }

    void TryLoadPreferences()
    {
        rw_userprefs loaded = rw_utils.LoadPreferences();
        if (loaded == null || ProgramManager.Instance.FORCE_NEW_SAVE_FILE)
        {
            rw_utils.prefs = rw_userprefs.FactoryDefaults();
            ShowWarningMenu();
            Debug.Log("Failed to load prefs. Using factory defaults...");
        }
        else
        {
            rw_utils.prefs = loaded;
            if (!string.IsNullOrEmpty(rw_utils.prefs.previousLoadDirectory)) {
                DisplayManager.Instance.LoadLayoutFromDirectory(rw_utils.prefs.previousLoadDirectory);
            }
            Debug.Log("Successfully loaded user prefs.");
        }

        // whatever DOES get loaded, make it visible
        PopulateInputs();
    }

    public void OpenTab(int childIndex)
    {
        for (int i = 0; i < t_panelContainer.childCount; i++)
        {
            if (i == childIndex)
            {
                t_panelContainer.GetChild(i).gameObject.SetActive(true);
            }
            else { t_panelContainer.GetChild(i).gameObject.SetActive(false); }
        }
    }

    // populating the settings menus with whats been saved to disk
    public void PopulateInputs()
    {
        tx_layoutDir.text = rw_utils.prefs.defaultLayoutDirectory;
        tx_recordingDir.text = rw_utils.prefs.defaultRecordingDirectory;

        in_serverIP.text = rw_utils.prefs.serverIP;
        in_teamNumber.text = rw_utils.prefs.teamNumber.ToString();

        splashScreenCheckbox.Set(rw_utils.prefs.showSplashScreen);

        in_tableName.text = rw_utils.prefs.ntTableName;
        in_ntProtocol.value = (int)rw_utils.prefs.ntProtocol;

        Debug.Log("Refreshing settings menus...");
    }
    // updating the saved settings with the user inputs
    public void UpdateFromInputted()
    {
        rw_utils.prefs.teamNumber = int.Parse(in_teamNumber.text);
        rw_utils.prefs.serverIP = in_serverIP.text;

        rw_utils.prefs.defaultLayoutDirectory = tx_layoutDir.text;
        rw_utils.prefs.defaultRecordingDirectory = tx_recordingDir.text;

        rw_utils.prefs.showSplashScreen = splashScreenCheckbox.value;

        rw_utils.prefs.ntTableName = in_tableName.text;
        rw_utils.prefs.ntProtocol = (ushort)in_ntProtocol.value;

        Debug.Log("Pushing settings changes...");

        NetworkManager.Instance.ReInitializeNetwork();
    }

    void Update()
    {
        if (CanvasUtils.IsCursorInteract(selectionTabs.gameObject, true))
        {
            for (int i = 0; i < selectionTabs.transform.childCount; i++)
            {
                selectionTabs.transform.GetChild(i).localPosition += Vector3.right * Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
            }
        }
    }

    public void ChangeLayoutDirectory()
    {
        UIManager.Instance.SelectDirectory(rw_utils.prefs.defaultLayoutDirectory, "", (x) => { rw_utils.prefs.defaultLayoutDirectory = x; PopulateInputs(); });
    }
    public void ChangeRecordingDirectory()
    {
        UIManager.Instance.SelectDirectory(rw_utils.prefs.defaultRecordingDirectory, "", (x) => { rw_utils.prefs.defaultRecordingDirectory = x; PopulateInputs(); });
    }
}
