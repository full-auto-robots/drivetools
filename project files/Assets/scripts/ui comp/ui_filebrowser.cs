using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Collections.Generic;

// seems unecessary but im doing it anyway
public enum BrowseMode
{
    Browse,
    Recents,
}

// this might just be the fuckin' bane of my existance

public class ui_filebrowser : MonoBehaviour
{
    private ushort viewingMode;
    [Header("CONFIG")]
    public float numHorizontal;
    public float numVertical;
    public string directory;

    [Header("REFERENCES")]
    public GameObject p_fileIcon;
    public Transform t_iconContainer;
    public Transform t_recentIconContainer;

    public float scrollPhase;
    public float scrollSpeed;

    public UnityAction<string> whenSelectionConfirmed;

    // we don't want "recently selected" to be handled by this script,
    // because then we have to just store/assume everything
    // so the 'recent' files are passed in here
    public string[] recentFileDirectories;
    // for now, they'll all just have whatever icons they would normally have

    public GameObject g_recentButton;
    public GameObject g_browseButton;

    public TextMeshProUGUI titleDisplay;

    private Vector3 originalPosition;

    public void SwitchToBrowse()
    {
        viewingMode = (ushort)BrowseMode.Browse;

        t_iconContainer.gameObject.SetActive(true);
        t_recentIconContainer.gameObject.SetActive(false);
    }

    public void SwitchToRecents()
    {
        viewingMode = (ushort)BrowseMode.Recents;

        t_iconContainer.gameObject.SetActive(false);
        t_recentIconContainer.gameObject.SetActive(true);
    }

    void Awake()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        directory = "";

        originalPosition = t_iconContainer.localPosition;
    }

    void Update()
    {
        HandleInteraction();
    }

    void HandleInteraction()
    {
        if (CanvasUtils.IsCursorInteract(transform.GetChild(0).GetChild(0).gameObject, true))
        {
            scrollPhase += Input.GetAxis("Mouse ScrollWheel") * -scrollSpeed;
            t_iconContainer.localPosition = originalPosition + Vector3.up * scrollPhase;
            t_recentIconContainer.localPosition = originalPosition + Vector3.up * scrollPhase;
        }
    }

    public void ConfirmDirectory()
    {
        whenSelectionConfirmed.Invoke(directory);
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void Cancel()
    {
        whenSelectionConfirmed = null;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void ConfirmDirectory(string dir)
    {
        whenSelectionConfirmed.Invoke(dir);
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void DrawDirectory(string dir)
    {
        DrawDirectory(dir, "", new string[0],"Browse:");
    }   
    // the recent file directories are passed in with this same function,
    // so we can safely re-initialize the array
    public void DrawDirectory(string dir, string[] recents)
    {
        DrawDirectory(dir, "", recents,"Browse:");
    }
    public void DrawDirectory(string dir, string requiredExtension)
    {
        DrawDirectory(dir, requiredExtension, new string[0],"Browse:");
    } 

    // loading the UI, given a string directory
    public void DrawDirectory(string dir, string requiredExtension, string[] recents, string title)
    {
        scrollPhase = 0;
        titleDisplay.text = title;
        
        SwitchToBrowse();
        recentFileDirectories = recents; // initializing the array
        g_recentButton.SetActive(recentFileDirectories.Length > 0); // only show the option if there are recent files

        CanvasUtils.DestroyChildren(t_iconContainer.gameObject);

        //Debug.Log(dir);
        directory = dir; // saving in case we want to modify it

        // *** folders first, like in windows explorer
        string[] folderPaths = rw_utils.GetFoldersInPath(dir);

        // not keeping track of existing space (see ConsoleMenu.cs) bc file icons are uniform size

        int horizontalIndex = 0;
        int verticalIndex = 0;
        for (int i = 0; i < recentFileDirectories.Length; i++)
        {
            // TODO: one function that adds the icon so I don't have to c+v this code three times
            Transform t_newIcon = Instantiate(p_fileIcon, t_recentIconContainer).transform;
            t_newIcon.localPosition = -Vector3.up * verticalIndex * t_newIcon.GetComponent<RectTransform>().sizeDelta.y +
            Vector3.right * horizontalIndex * t_newIcon.GetComponent<RectTransform>().sizeDelta.x;

            t_newIcon.GetChild(0).GetComponent<Image>().sprite = rw_utils.GetIcon(recentFileDirectories[i]);
            t_newIcon.GetChild(1).GetComponent<TextMeshProUGUI>().text = rw_utils.GetLastDirectory(recentFileDirectories[i]);

            int temp = i;
            t_newIcon.GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
            {
                ConfirmDirectory(recentFileDirectories[temp]);
            });

            horizontalIndex++;
            if (horizontalIndex >= numHorizontal)
            {
                horizontalIndex = 0;
                verticalIndex++;
            }
        }

        horizontalIndex = 0;
        verticalIndex = 0;
        for (int i = 0; i < folderPaths.Length; i++)
        {
            // spawn in a new folder icon
            Transform t_newIcon = Instantiate(p_fileIcon, t_iconContainer).transform;
            t_newIcon.localPosition = -Vector3.up * verticalIndex * t_newIcon.GetComponent<RectTransform>().sizeDelta.y +
            Vector3.right * horizontalIndex * t_newIcon.GetComponent<RectTransform>().sizeDelta.x;

            t_newIcon.GetChild(0).GetComponent<Image>().sprite = rw_utils.GetIcon(folderPaths[i]);
            string folderPath = rw_utils.GetLastDirectory(folderPaths[i]);
            t_newIcon.GetChild(1).GetComponent<TextMeshProUGUI>().text = folderPath.Substring(1, folderPath.Length - 1);

            t_newIcon.GetChild(0).GetComponent<Button>().onClick.AddListener(() => DrawDirectory(dir + folderPath));

            horizontalIndex++;
            if (horizontalIndex >= numHorizontal)
            {
                horizontalIndex = 0;
                verticalIndex++;
            }
        }

        // *** now files
        // TODO: hiding certain files
        string[] filePaths = rw_utils.GetFilesInPath(dir);
        for (int i = 0; i < filePaths.Length; i++)
        {
            if (rw_utils.GetFileExtension(filePaths[i]) != requiredExtension && requiredExtension.Length > 0)
            {
                continue;
            }
            // spawn in a new folder icon
            Transform t_newIcon = Instantiate(p_fileIcon, t_iconContainer).transform;
            t_newIcon.localPosition = -Vector3.up * verticalIndex * t_newIcon.GetComponent<RectTransform>().sizeDelta.y +
            Vector3.right * horizontalIndex * t_newIcon.GetComponent<RectTransform>().sizeDelta.x;

            t_newIcon.GetChild(0).GetComponent<Image>().sprite = rw_utils.GetIcon(filePaths[i]);
            t_newIcon.GetChild(1).GetComponent<TextMeshProUGUI>().text = rw_utils.GetLastDirectory(filePaths[i]);

            int j = i;
            t_newIcon.GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
            {
                ConfirmDirectory(filePaths[j]);
            });

            horizontalIndex++;
            if (horizontalIndex >= numHorizontal)
            {
                horizontalIndex = 0;
                verticalIndex++;
            }
        }
    }

    public void GoBack()
    {
        DrawDirectory(rw_utils.RemoveLastDirectory(directory));
    }
}
