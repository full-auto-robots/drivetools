using System.Collections.Generic;
using UnityEngine;

// i COULD have multiple classes for different categories of settings,
// but having it all in one class really does make my life easier

[System.Serializable]
public class rw_userprefs
{
    // the last directory that you had loaded
    public string previousLoadDirectory;

    // network settings! ***
    public int teamNumber; // used for NT
    public string serverIP;
    public string ntTableName;
    public ushort ntProtocol; // 0 is NT3, 1 is NT4
    // ***

    // file stuff ***
    public string defaultLayoutDirectory;
    public string defaultRecordingDirectory;

    // ***

    // misc. ***
    public bool enableAlerts;
    public bool showSplashScreen;
    // ***

    public List<string> recentLayoutDirectories;

    // working directories for saving/loading layouts
    // just a qol feature, the file explorer will open the directory that you were using last
    public string workingLayoutSaveDirectory;
    public string workingLayoutLoadDirectory;

    public static rw_userprefs FactoryDefaults()
    {
        rw_userprefs result = new rw_userprefs();

        result.recentLayoutDirectories = new List<string>();

        result.teamNumber = 2386; // trojans pride
        result.serverIP = "10.23.86.2"; // TROJNANS PRIDE

        result.defaultLayoutDirectory = rw_utils.saveDirectory + "/" + "v" + rw_utils.buildVersion + "/";
        result.defaultRecordingDirectory = rw_utils.saveDirectory + "/" + "v" + rw_utils.buildVersion + "/";

        result.workingLayoutSaveDirectory = result.defaultLayoutDirectory;
        result.workingLayoutLoadDirectory = result.defaultLayoutDirectory;

        result.enableAlerts = false;

        result.showSplashScreen = true;

        result.ntTableName = "SmartDashboard";

        result.ntProtocol = 0;

        return result;
    }
}
