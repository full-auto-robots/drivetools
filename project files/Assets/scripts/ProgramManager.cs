using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// highest-level tasks

public class ProgramManager : MonoBehaviour
{
    private static ProgramManager _instance;

    public static ProgramManager Instance
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
    public bool FORCE_NEW_SAVE_FILE;

    private void Awake()
    {
        Instance = this;
        //rw_utils.prefs = null;

        Application.targetFrameRate = 60;
    }

    // should be pretty much the ONLY occurence of this function other than for initialization purposes
    void Start()
    {
        rw_utils.ModifyTxtWithNoSurprises(
            "C:/Users/Public/Documents/FRC/FRC DS Data Storage.ini",
            3,
            rw_utils.RemoveLastDirectory("DashboardCmdLine = " + Application.dataPath) + "/far_drivetools.exe");

        UIManager.Instance.TurnOffDebugObjects();
    }

    void Update()
    {
        if (!NetworkManager.Instance.isNetworkReady && rw_utils.prefs != null) { 
            NetworkManager.Instance.InitializeNetwork(); 
            }
    }
}
