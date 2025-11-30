using System.Collections.Generic;
using UnityEngine;

// fake NetworkTables lolol
// (there are no tables, cuz rn I have no use for them)

public class net_fakent : MonoBehaviour
{
    private static net_fakent _instance;

    public static net_fakent Instance
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

    public List<string> activeKeys;
    public List<string> activeValues;

    public static void WriteString(string key, string value)
    {
        for (int i = 0; i < Instance.activeKeys.Count; i++)
        {
            if (Instance.activeKeys[i] == key)
            {
                Instance.activeValues[i] = value;
                return;
            }
        }

        Instance.activeKeys.Add(key);
        Instance.activeValues.Add(value);
    }

    public static string[] GetKeys()
    {
        return Instance.activeKeys.ToArray();
    }

    public static string GetEntry(string key)
    {
        for (int i = 0; i < Instance.activeKeys.Count; i++)
        {
            if (Instance.activeKeys[i] == key)
            {
                return Instance.activeValues[i];
            }
        }

        return "";
    }
    public static void PostToConsole(string tag, string msg)
    {
        PostToConsole(new net_consolemessage(msg, tag));
    }
    public static void PostToConsole(net_consolemessage msg)
    {
        // formatting looks like this: {[tag] msg}{[tag] msg}{[tag] msg}
        string existingQueue = GetEntry("$console");

        WriteString("$console", existingQueue + "{[" + msg.tag + "]" + msg.message + "}");
    }
}
