using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

// this might just be the fuckin' bane of my existance

public class ui_filebrowser : MonoBehaviour
{
    [Header("CONFIG")]
    public float numHorizontal;
    public float numVertical;
    public string directory;

    [Header("REFERENCES")]
    public GameObject p_fileIcon;
    public Transform t_iconContainer;

    public float scrollPhase;
    public float scrollSpeed;

    public UnityAction<string> whenSelectionConfirmed;

    void Awake()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        directory = "";
    }

    void Update()
    {
        HandleInteraction();
    }

    void HandleInteraction()
    {
        if (CanvasUtils.IsCursorInteract(t_iconContainer.parent.gameObject, true))
        {
            scrollPhase += Input.GetAxis("Mouse ScrollWheel") * -scrollSpeed;
            t_iconContainer.localPosition = Vector3.up * scrollPhase;
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
        DrawDirectory(dir, "");
    }

    // loading the UI, given a string directory
    public void DrawDirectory(string dir, string requiredExtension)
    {
        CanvasUtils.DestroyChildren(t_iconContainer.gameObject);

        //Debug.Log(dir);
        directory = dir; // saving in case we want to modify it

        // *** folders first, like in windows explorer
        string[] folderPaths = rw_utils.GetFoldersInPath(dir);

        // not keeping track of existing space (see ConsoleMenu.cs) bc file icons are uniform size

        int horizontalIndex = 0;
        int verticalIndex = 0;
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
