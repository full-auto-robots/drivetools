using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum NetworkMode
{
    Simulated,
    NT3,
    NT4,
}

// grabs data EITHER FROM THE REAL NT, OR THE FAKE ONE (which the simulated robot pushes to)

// some data (related to buttons and the field2d node) will be more like commands, but still handled the same i think

public class NetworkManager : MonoBehaviour
{
    private static NetworkManager _instance;

    public static NetworkManager Instance
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

    [Header("CONFIG")]
    public NetworkMode mode;
    public float ntPullInterval; // how many seconds between NT refreshes
    public float ntRecordInterval; // so that we don't have timestamps every 0.1 seconds
    private float lastPullTime;
    private float lastRecordTime;
    public Color defaultColor;
    public Color viewingColor;

    [Header("DATA")]
    public bool isNetworkReady;
    public bool isTableReady;
    public bool isRecording;
    public bool isInViewingMode;
    public float timeBeingViewed;
    public net_ntrecording viewedRecording;
    public double timeSinceRecordingStart;
    public net_ntrecording currentRecording; // the one thats in-progress

    // NOT DOING A RUNTIME LIST!! RECORDINGS ARE NOW INDIVIDUAL FILES
    //public List<net_ntrecording> allRecordings; // might store this on-disk as an array, but it'll be a list during runtime

    public net_ntdatalist latestData; // updates a few times a second, as per ntPullInterval

    [Header("REFERENCES")]
    public SimulatedRobot sim;
    public Image i_recordingButton;
    public Image i_connectionIcon;
    public Image i_protocolIcon;
    public SliderHandle timeBar;
    public TextMeshProUGUI startText;
    public TextMeshProUGUI endText;
    public List<string> flaggedKeys;

    public Sprite nt3_protocol;
    public Sprite nt4_protocol;
    public Sprite sim_protocol;

    private NetworkTables client;

    public net_ntdatalist PackageFlaggedNTValues()
    {
        net_ntdatalist result = new net_ntdatalist();
        result.dataKeys = new string[flaggedKeys.Count];
        result.dataValues = new string[flaggedKeys.Count];

        for (int i = 0; i < flaggedKeys.Count; i++)
        {
            result.dataKeys[i] = flaggedKeys[i];
            result.dataValues[i] = latestData.GetValueAt(flaggedKeys[i]);
        }

        return result;
    }

    public void EnterViewingMode()
    {
        isInViewingMode = true;
        DisplayManager.Instance.t_bg.GetComponent<Image>().color = viewingColor;
        UIManager.Instance.t_recordingBar.GetChild(0).gameObject.SetActive(true);
    }
    public void ExitViewingMode()
    {
        isInViewingMode = false;
        DisplayManager.Instance.t_bg.GetComponent<Image>().color = defaultColor;
        UIManager.Instance.t_recordingBar.GetChild(0).gameObject.SetActive(false);
    }

    public void CloseRecording()
    {
        DisplayManager.Instance.ClearAllDisplayObjects();
        ExitViewingMode();
    }

    public void SetValueAt(string key, string newValue)
    {
        if (mode == NetworkMode.Simulated)
        {
            net_fakent.WriteString(key, newValue);
        } else if (mode == NetworkMode.NT3)
        {
            // TODO: confirm that this works!!!
            //client.SetString(key, newValue); //&@
        } else if (mode == NetworkMode.NT4)
        {
            // TODO:
        }
    }

    public void ViewRecording()
    {
        UIManager.Instance.SelectDirectory(rw_utils.prefs.defaultRecordingDirectory, ".rec",  (x) => ViewRecordingAtDirectory(x));
    }

    public void ViewRecordingAtDirectory(string dir)
    {
        net_ntrecording recording = rw_utils.LoadRecording(dir);
        if (recording != null)
        {
            Debug.Log("Loaded recording!");

            EnterViewingMode();
            DisplayManager.Instance.LoadDisplayLayout(recording.layoutUponRecording);

            viewedRecording = recording;
        }
        else
        {
            Debug.Log("Failed to load recording!");
        }
    }

    public void StartRecording()
    {
        isRecording = true;

        timeSinceRecordingStart = Time.time;

        // init the recording class
        currentRecording = new net_ntrecording();
        currentRecording.timeStamps = new List<double>();
        currentRecording.snapshots = new List<net_ntdatalist>();

        i_recordingButton.sprite = Sys.icons[3];
    }

    public void StopRecording()
    {
        isRecording = false;

        currentRecording.layoutUponRecording = DisplayManager.Instance.GetCurrentDisplayLayout();
        rw_utils.SaveRecording(currentRecording);
        Debug.Log("Writing data to disk...");

        // just making sure the vars are clear
        currentRecording = null;
        timeSinceRecordingStart = -1;

        i_recordingButton.sprite = Sys.icons[2];
    }

    void OnApplicationQuit()
    {
        // TODO: writing recording upon quit
        KillNT();
        rw_utils.SavePreferences(rw_utils.prefs);

        //Debug.Log(rw_utils.prefs.showSplashScreen);
    }

    public void AddSnapshotToCurrentRecording()
    {
        currentRecording.timeStamps.Add(Time.time - timeSinceRecordingStart);
        currentRecording.snapshots.Add(latestData);
    }

    void Update()
    {
        i_connectionIcon.color = IsConnectedGeneral() ? Color.green : Color.red;
        i_protocolIcon.color = IsConnectedGeneral() ? Color.green : Color.red;
        i_protocolIcon.sprite = (ushort)mode == 1 ? nt3_protocol : nt4_protocol;

        if (!isInViewingMode)
        {
            // the clock that controls the network updates
            if (Time.time > lastPullTime + ntPullInterval)
            {
                Pull();
            }
        }
        else
        {
            float totalTime = (float)viewedRecording.timeStamps[viewedRecording.timeStamps.Count - 1];
            timeBeingViewed = timeBar.GetValue() * totalTime;
            // how long the recording is is just the last timestamp
            endText.text = Sys.ConvertToClockFormat(totalTime);
            startText.text = Sys.ConvertToClockFormat(timeBeingViewed);

            latestData = viewedRecording.GetSnapShot(timeBeingViewed);
        }
    }

    public void Pull()
    {
        lastPullTime = Time.time;

        if (mode != NetworkMode.Simulated)
        {
            mode = (NetworkMode)(rw_utils.prefs.ntProtocol + 1);
        }

        if (mode == NetworkMode.Simulated)
        {
            // simulated pull
            // (its impossible to NOT be connected because its all local)

            // good to pull data
            net_ntdatalist newData = new net_ntdatalist();

            string[] unsortedStringKeys = net_fakent.GetKeys();

            string[] unsortedStringValues = new string[unsortedStringKeys.Length];

            for (int i = 0; i < unsortedStringValues.Length; i++)
            {
                unsortedStringValues[i] = net_fakent.GetEntry(unsortedStringKeys[i]);
            }

            newData.dataKeys = unsortedStringKeys;
            newData.dataValues = unsortedStringValues;

            latestData = newData;

            Debug.Log("Pulling data from local NT... [" + newData.dataKeys.Length + "] keys found.");
        }
        else if (mode == NetworkMode.NT3)
        {
            // proper networktables pull

            if (IsConnectedToNT3())
            {
                if (!isTableReady)
                {
                    // no data to pull, gotta get a ref to the table

                    Debug.Log("No reference to table! Fetching reference...");
                    Debug.Log("Got reference to table!");
                    isTableReady = true;
                }
                if (isTableReady)
                {
                    Debug.Log("Pulling data from" + rw_utils.prefs.ntTableName + "...");
                    // good to pull data
                    net_ntdatalist newData = new net_ntdatalist();

                    List<string> keys = new List<string>();

                    for (int i = 0; i < DisplayManager.Instance.activeDisplayObjects.Count; i++)
                    {
                        keys.Add(DisplayManager.Instance.activeDisplayObjects[i].GetComponent<dsp_mainvisual>().ntKey);
                    }

                    string[] keyArray = new string[keys.Count];
                    string[] valueArray = new string[keyArray.Length];

                    for (int i = 0; i < keyArray.Length; i++)
                    {
                        keyArray[i] = keys[i];
                        //valueArray[i] = client.GetString(keyArray[i]); //&@
                    }

                    if (keyArray.Length > 0) {
                        newData.dataKeys = keyArray;
                        newData.dataValues = valueArray;
                        
                    } else {
                        newData.dataKeys = new string[0];
                        newData.dataValues = new string[0];
                    }

                    latestData = newData;
                }
            }
            else
            {
                Debug.Log("No connection. Cannot pull data!");
            }
        }
        else if (mode == NetworkMode.NT4)
        {
            // proper networktables pull

            if (IsConnectedToNT4())
            {
                // TODO: pull NT4 data
            }
            else
            {
                Debug.Log("No connection. Cannot pull data!");
            }
        }

        if (isRecording && Time.time > lastRecordTime + ntRecordInterval)
        {
            lastRecordTime = Time.time;
            AddSnapshotToCurrentRecording();
        }
    }

    public string[] SeparateKeysOfType(string[] raw, string type)
    {
        // if type is "", that means we're looking for actual strings
        // otherwise we're looking for a class and type is the name

        List<string> result = new List<string>();

        for (int i = 0; i < raw.Length; i++)
        {
            if (type == "")
            {
                if (raw[i][0] != '$')
                {
                    result.Add(raw[i]);
                }
            }
            else
            {
                if (raw[i][0] == '$')
                {
                    if (raw[i].Substring(1, type.Length) == type)
                    {
                        result.Add(raw[i]);
                    }
                }
            }
        }

        return result.ToArray();
    }

    // *** network-specific stuff (NT3,NT4, etc.) ***

    public void InitializeNetwork()
    {
        Debug.Log("STARTING NETWORK!");

        ExitViewingMode();

        if (mode != NetworkMode.Simulated)
        {
            mode = (NetworkMode)(rw_utils.prefs.ntProtocol + 1);
        }

        if (mode == NetworkMode.NT3)
        {
            InitializeNT3Client();

            // the simulated robot will be OFF by default
        }
        else if (mode == NetworkMode.NT4)
        {
            InitializeNT4Client();

            // the simulated robot will be OFF by default
        }
        else if (mode == NetworkMode.Simulated)
        {
            sim.isActive = true;
        }

        isNetworkReady = true;

        i_recordingButton.sprite = Sys.icons[2];
    }

    public void ReInitializeNetwork()
    {
        if (!isNetworkReady) return;

        Debug.Log("REFRESHING NETWORK!");
        ExitViewingMode();

        if (mode != NetworkMode.Simulated)
        {
            mode = (NetworkMode)(rw_utils.prefs.ntProtocol + 1);
        }

        if (mode == NetworkMode.NT3)
        {
            ReInitializeNT3Client();

            // the simulated robot will be OFF by default
        }
        else if (mode == NetworkMode.NT4)
        {
            ReInitializeNT4Client();

            // the simulated robot will be OFF by default
        }
        else if (mode == NetworkMode.Simulated)
        {
            sim.isActive = true;
        }

        i_recordingButton.sprite = Sys.icons[2];
    }

    public void KillNT()
    {
        Debug.Log("Stopping networktables... (not really)");
    }

    public void InitializeNT3Client()
    {
        Debug.Log("Starting networktables...");
        //client = new NetworkTablesClient("drivetools", rw_utils.prefs.serverIP); //&@
    }

    // when the user updates the settings
    public void ReInitializeNT3Client()
    {
        //client = new NetworkTablesClient("drivetools", rw_utils.prefs.serverIP); //&@
    }

    public void InitializeNT4Client()
    {
        // Debug.Log("Starting networktables...");
        
        // nt4_source = new Nt4Source(rw_utils.prefs.serverIP, "DriveTools", true, 5810);

        // //nt4_source.Connect();
    }

    public void ReInitializeNT4Client()
    {
        // nt4_source.Disconnect();

        // nt4_source = new Nt4Source(rw_utils.prefs.serverIP, "DriveTools", true, 5810);
        // //nt4_source.Connect();
    }

    // allows check w/o specifying comm. mode
    public bool IsConnectedGeneral()
    {
        if (mode == NetworkMode.Simulated)
        {
            return IsConnectedSimulated();
        }
        else if (mode == NetworkMode.NT3)
        {
            return IsConnectedToNT3();
        }
        else
        {
            return IsConnectedToNT4();
        }
    }

    public bool IsConnectedSimulated()
    {
        return rw_utils.prefs.serverIP == "10." + rw_utils.prefs.teamNumber.ToString().Substring(0, 2) + "." + rw_utils.prefs.teamNumber.ToString().Substring(2, 2) + ".2";
    }

    public bool IsConnectedToNT3()
    {
        return true;
    }

    public bool IsConnectedToNT4()
    {
        return false;
    }
}
