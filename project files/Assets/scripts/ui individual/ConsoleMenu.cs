using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConsoleMenu : MonoBehaviour
{
    public RectTransform rt_mask;
    public RectTransform rt_bottomMask;

    public Transform t_messageContainer;
    public Transform t_filterContainer;

    public GameObject p_msg;
    public List<net_consolemessage> messages; // this will be whats updated by NT

    public float scrollSpeed;

    private float lastConsoleUpdate;
    public float consoleUpdateInterval;

    private float scrollPhase;
    private float filterScrollPhase;
    private int lastMsgCount;

    public GameObject p_filter;

    void Awake()
    {
        scrollPhase = -100;
        filterScrollPhase = 50;
    }

    public void LogMessaage(string msg, string tag)
    {
        LogMessaage(new net_consolemessage(msg, tag));
    }
    public void LogMessaage(net_consolemessage msg)
    {
        messages.Add(msg);
    }

    void Update()
    {
        HandleInteraction();
    }

    string[] GetAllActiveTags()
    {
        List<string> result = new List<string>();

        for (int i = 0; i < messages.Count; i++)
        {
            if (!result.Contains(messages[i].tag))
            {
                result.Add(messages[i].tag);
            }
        }

        return result.ToArray();
    }

    void HandleInteraction()
    {
        if (CanvasUtils.IsCursorInteract(rt_mask.gameObject, true))
        {
            scrollPhase += Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
            t_messageContainer.localPosition = Vector3.up * scrollPhase;
        }

        if (CanvasUtils.IsCursorInteract(rt_bottomMask.gameObject, true))
        {
            filterScrollPhase += Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
            t_filterContainer.localPosition = Vector3.right * filterScrollPhase;
        }

        if (Time.time > lastConsoleUpdate + consoleUpdateInterval)
        {
            PullMessages();
            Refresh();
            lastConsoleUpdate = Time.time;
        }
    }

    public void PullMessages()
    {
        Debug.Log("Pulling console messages...");

        string rawData = NetworkManager.Instance.latestData.GetValueAt("$console");

        if (rawData.Length == 0) return;

        List<string> newMsgs = new List<string>();

        int currentIndex = 0;
        while (currentIndex != -1 && currentIndex < rawData.Length)
        {
            int startIndex = currentIndex;
            int endIndex = Sys.FindClosingBracket(rawData, startIndex);

            newMsgs.Add(rawData.Substring(startIndex + 1, endIndex - startIndex - 1));

            if (endIndex < rawData.Length - 1)
            {
                rawData = rawData.Substring(endIndex + 1, rawData.Length - (endIndex + 1));
                currentIndex = 0;
            }
            else
            {
                currentIndex = -1;
            }
        }

        for (int i = 0; i < newMsgs.Count; i++)
        {
            net_consolemessage processed = new net_consolemessage();

            //Debug.Log("msg    " + newMsgs[i]);

            int tagEndIndex = Sys.FindClosingBracket(newMsgs[i], 0);

            processed.tag = newMsgs[i].Substring(1, tagEndIndex - 1);

            //Debug.Log("taag    " + tagEndIndex);

            processed.message = newMsgs[i].Substring(tagEndIndex + 1, newMsgs[i].Length - (tagEndIndex + 1));

            messages.Add(processed);
        }

        NetworkManager.Instance.SetValueAt("$console", "");
    }

    // TODO: maybe a better approach that doesn't involve purging everything at the start???
    public void Refresh()
    {
        Vector2 keySizeDelta = p_msg.GetComponent<RectTransform>().sizeDelta;

        // rt_bg.sizeDelta = new Vector2(keySizeDelta.x, keySizeDelta.y * NetworkManager.Instance.latestData.dataKeys.Length);
        // rt_grabbyBit.sizeDelta = new Vector2(keySizeDelta.x, rt_grabbyBit.sizeDelta.y);

        CanvasUtils.DestroyChildren(t_messageContainer.gameObject);

        float existingSpacing = 0;
        for (int i = 0; i < messages.Count; i++)
        {
            // may as well pass .zero because we have to set the parent and do all that
            Transform t_newKeyEntry = Instantiate(p_msg, Vector3.zero, Quaternion.identity).transform;

            t_newKeyEntry.SetParent(t_messageContainer);
            t_newKeyEntry.localPosition = -Vector3.up * existingSpacing;
            t_newKeyEntry.GetChild(1).GetComponent<TextMeshProUGUI>().text = messages[i].message;
            //t_newKeyEntry.GetChild(1).GetComponent<Image>().color = Sys.typeColors[Sys.GetTypeIndex(NetworkManager.Instance.latestData.dataValues[i])];

            existingSpacing += 0 + t_newKeyEntry.GetComponent<RectTransform>().sizeDelta.y;
            if (i > lastMsgCount - 1) scrollPhase += 0 + t_newKeyEntry.GetComponent<RectTransform>().sizeDelta.y;
        }
        lastMsgCount = messages.Count;
        t_messageContainer.localPosition = Vector3.up * scrollPhase;

        // dealing with the whole filter thing
        string[] tags = GetAllActiveTags();
        CanvasUtils.DestroyChildren(t_filterContainer.gameObject);

        existingSpacing = 0;
        for (int i = 0; i < tags.Length; i++)
        {
            Transform t_newFilter = Instantiate(p_filter, t_filterContainer).transform;
            t_newFilter.GetComponent<TextMeshProUGUI>().text = "[" + tags[i] + "]";

            t_newFilter.localPosition = Vector3.right * existingSpacing;

            existingSpacing += 0 + t_newFilter.GetComponent<RectTransform>().sizeDelta.x;
        }
    }
}
