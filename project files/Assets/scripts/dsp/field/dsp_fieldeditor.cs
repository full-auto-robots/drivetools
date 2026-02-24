using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


// worth noting that this class is on the same gameObject as dsp_field
public class dsp_fieldeditor : MonoBehaviour
{
    public TextMeshProUGUI tx_prompt;
    public GameObject g_parentObj; // so the entire menu can be activated/shutdown at once
    private bool isDrawMode;
    private bool isRobotPlaceMode;
    private bool isMarkerPlaceMode;

    private int activeIndex; // when in drawing mode, which path do we use?

    [Header("REFERENCES")]
    public Transform t_robotContainer;
    public Transform t_robotHeaderContainer;
    public Transform t_lineContainer; // the line renderers
    public Transform t_pathHeaderContainer; // the little list of ui elements
    public Transform t_markerContainer;
    public Transform t_markerHeaderContainer;

    [Header("PREFABS")]
    public GameObject p_pathLineRenderer;
    public GameObject p_robot;
    public GameObject p_marker;

    public GameObject p_pathHeader;
    public GameObject p_robotHeader;
    public GameObject p_markerHeader;

    public Sprite[] drawModeButtons;
    private bool allowedFieldInteraction;

    public GameObject robotSettingsMenu;
    public GameObject markerSettingsMenu;
    private bool isEditing;

    public Button markerEditingButton;

    void Awake()
    {
        isDrawMode = false; // just so we're sure

        robotSettingsMenu.SetActive(false); // in case it was left active
        markerSettingsMenu.SetActive(false); 
        // no path settings, cuz they're not that complicated

        // keep in mind that field markers are stored on a software-wide basis, not per-display
        // so the menu is too
        markerEditingButton.onClick.AddListener(() => MarkerMenu.Instance.Toggle());
    }

    public void Open()
    {
        isEditing = true;
        g_parentObj.SetActive(true);
        tx_prompt.gameObject.SetActive(false);
    }

    public void Close()
    {
        isEditing = false;
        g_parentObj.SetActive(false);
    }

    // same as below but when you click a marker is spawned
    // TODO:
    public void StartPlacingMarker()
    {
        tx_prompt.gameObject.SetActive(true);
        tx_prompt.text = "LMB to place";

        Transform t_newMarkerHeader = Instantiate(p_markerHeader, t_markerHeaderContainer).transform;
        t_newMarkerHeader.localPosition = -Vector3.up * t_newMarkerHeader.GetComponent<RectTransform>().sizeDelta.y * (t_markerHeaderContainer.childCount);
        Transform t_newMarker = Instantiate(p_marker, t_markerContainer).transform;

        t_newMarker.GetComponent<dsp_marker>().rt_field = transform.GetChild(0).GetComponent<RectTransform>();

        isMarkerPlaceMode = true;
        activeIndex = t_markerContainer.childCount - 1;

        allowedFieldInteraction = false;

        int temp = activeIndex;
        t_newMarkerHeader.GetChild(2).GetComponent<Button>().onClick.AddListener(() => CloneRobot(temp));
        t_newMarkerHeader.GetChild(3).GetComponent<Button>().onClick.AddListener(() => DeleteRobot(temp));
        t_newMarkerHeader.GetChild(1).GetComponent<Button>().onClick.AddListener(() => EditMarkerSettings(temp));

        t_newMarkerHeader.GetChild(0).GetComponent<TextMeshProUGUI>().text = t_markerContainer.childCount.ToString();
    }

    public void EditMarkerSettings(int index)
    {
        PopulateMarkerSettings(index);
        markerSettingsMenu.SetActive(true);

        markerSettingsMenu.transform.GetChild(0).GetChild(1).GetComponent<ui_button>().onPress.RemoveAllListeners();
        markerSettingsMenu.transform.GetChild(0).GetChild(1).GetComponent<ui_button>().onPress.AddListener(() => UIManager.Instance.GetColorFromPicker((col) => ChangeMarkerColor(index, col)));

        markerSettingsMenu.transform.GetChild(0).GetChild(3).GetComponent<TMP_InputField>().onEndEdit.RemoveAllListeners();
        markerSettingsMenu.transform.GetChild(0).GetChild(3).GetComponent<TMP_InputField>().onEndEdit.AddListener((x) => ChangeMarkerSize(index, double.Parse(x)));
        markerSettingsMenu.transform.GetChild(0).GetChild(5).GetComponent<TMP_InputField>().onEndEdit.RemoveAllListeners();
        markerSettingsMenu.transform.GetChild(0).GetChild(5).GetComponent<TMP_InputField>().onEndEdit.AddListener((x) => ChangeMarkerZAngle(index, double.Parse(x)));

        markerSettingsMenu.transform.GetChild(0).GetChild(7).GetComponent<TMP_InputField>().onEndEdit.RemoveAllListeners();
        markerSettingsMenu.transform.GetChild(0).GetChild(7).GetComponent<TMP_InputField>().onEndEdit.AddListener((x) => ChangeMarkerDisplayIndex(index, ushort.Parse(x)));
    }
    void PopulateMarkerSettings(int index)
    {
        nav_marker data = t_markerContainer.GetChild(index).GetComponent<dsp_marker>().data;

        markerSettingsMenu.transform.GetChild(0).GetChild(3).GetComponent<TMP_InputField>().text = data.size.ToString();
        markerSettingsMenu.transform.GetChild(0).GetChild(5).GetComponent<TMP_InputField>().text = data.zAngle.ToString();
        markerSettingsMenu.transform.GetChild(0).GetChild(7).GetComponent<TMP_InputField>().text = data.displayIndex.ToString();
    }
    void ChangeMarkerColor(int index, Color color)
    {
        t_markerContainer.GetChild(index).GetComponent<dsp_marker>().data.color = color;
        t_markerContainer.GetChild(index).GetComponent<dsp_marker>().Draw();
    }
    void ChangeMarkerSize(int index, double size)
    {
        t_markerContainer.GetChild(index).GetComponent<dsp_marker>().data.size = size;
        t_markerContainer.GetChild(index).GetComponent<dsp_marker>().Draw();
    }
    void ChangeMarkerZAngle(int index, double zAngle)
    {
        t_markerContainer.GetChild(index).GetComponent<dsp_marker>().data.zAngle = zAngle;
        t_markerContainer.GetChild(index).GetComponent<dsp_marker>().Draw();
    }
    void ChangeMarkerDisplayIndex(int index, ushort newIndex)
    {
        t_markerContainer.GetChild(index).GetComponent<dsp_marker>().data.displayIndex = newIndex;
        t_markerContainer.GetChild(index).GetComponent<dsp_marker>().Draw();
    }

    /*
    * welcome to the least efficient section of code ever!!!!
    */
    public void ChangeRobotColor(int index, Color color)
    {
        t_robotContainer.GetChild(index).GetComponent<dsp_robot>().data.color = color;
        t_robotContainer.GetChild(index).GetComponent<dsp_robot>().Draw();
    }
    public void ChangeRobotWidth(int index, double width)
    {
        t_robotContainer.GetChild(index).GetComponent<dsp_robot>().data.width = width;
        t_robotContainer.GetChild(index).GetComponent<dsp_robot>().Draw();
    }
    public void ChangeRobotLength(int index, double length)
    {
        t_robotContainer.GetChild(index).GetComponent<dsp_robot>().data.length = length;
        t_robotContainer.GetChild(index).GetComponent<dsp_robot>().Draw();
    }
    public void ChangeRobotTeamNumber(int index, ushort teamNumber)
    {
        t_robotContainer.GetChild(index).GetComponent<dsp_robot>().data.teamNumber = teamNumber;
        t_robotContainer.GetChild(index).GetComponent<dsp_robot>().Draw();
    }

    public void ChangeRobotX(int index, TMP_InputField input)
    {
        nav_robot data = t_robotContainer.GetChild(index).GetComponent<dsp_robot>().data;
        t_robotContainer.GetChild(index).GetComponent<dsp_robot>().data.position = new DoubleVector3(double.Parse(input.text), data.position.y, data.position.z);
        t_robotContainer.GetChild(index).GetComponent<dsp_robot>().Draw();
    }
    public void ChangeRobotX(int index, double x)
    {
        nav_robot data = t_robotContainer.GetChild(index).GetComponent<dsp_robot>().data;
        t_robotContainer.GetChild(index).GetComponent<dsp_robot>().data.position = new DoubleVector3(x, data.position.y, data.position.z);
        t_robotContainer.GetChild(index).GetComponent<dsp_robot>().Draw();
    }
    public void ChangeRobotY(int index, TMP_InputField input)
    {
        nav_robot data = t_robotContainer.GetChild(index).GetComponent<dsp_robot>().data;
        t_robotContainer.GetChild(index).GetComponent<dsp_robot>().data.position = new DoubleVector3(data.position.x, double.Parse(input.text), data.position.z);
        t_robotContainer.GetChild(index).GetComponent<dsp_robot>().Draw();
    }
    public void ChangeRobotY(int index, double y)
    {
        nav_robot data = t_robotContainer.GetChild(index).GetComponent<dsp_robot>().data;
        t_robotContainer.GetChild(index).GetComponent<dsp_robot>().data.position = new DoubleVector3(data.position.x, y, data.position.z);
        t_robotContainer.GetChild(index).GetComponent<dsp_robot>().Draw();
    }
    public void ChangeRobotZ(int index, TMP_InputField input)
    {
        nav_robot data = t_robotContainer.GetChild(index).GetComponent<dsp_robot>().data;
        t_robotContainer.GetChild(index).GetComponent<dsp_robot>().data.position = new DoubleVector3(data.position.x, data.position.y, double.Parse(input.text));
        t_robotContainer.GetChild(index).GetComponent<dsp_robot>().Draw();
    }
    public void ChangeRobotZ(int index, double z)
    {
        nav_robot data = t_robotContainer.GetChild(index).GetComponent<dsp_robot>().data;
        t_robotContainer.GetChild(index).GetComponent<dsp_robot>().data.position = new DoubleVector3(data.position.x, data.position.y, z);
        t_robotContainer.GetChild(index).GetComponent<dsp_robot>().Draw();
    }
    public void ChangeRobotZAngle(int index, TMP_InputField input)
    {
        t_robotContainer.GetChild(index).GetComponent<dsp_robot>().data.zAngle = double.Parse(input.text);
        t_robotContainer.GetChild(index).GetComponent<dsp_robot>().Draw();
    }
    public void ChangeRobotZAngle(int index, double a)
    {
        t_robotContainer.GetChild(index).GetComponent<dsp_robot>().data.zAngle = a;
        t_robotContainer.GetChild(index).GetComponent<dsp_robot>().Draw();
    }
    /**/


    // making sure the input field have the right values
    public void PopulateRobotSettings(int index)
    {
        nav_robot data = t_robotContainer.GetChild(index).GetComponent<dsp_robot>().data;

        robotSettingsMenu.transform.GetChild(0).GetChild(3).GetComponent<TMP_InputField>().text = data.length.ToString();
        robotSettingsMenu.transform.GetChild(0).GetChild(5).GetComponent<TMP_InputField>().text = data.width.ToString();

        robotSettingsMenu.transform.GetChild(0).GetChild(7).GetComponent<TMP_InputField>().text = data.position.x.ToString();
        robotSettingsMenu.transform.GetChild(0).GetChild(8).GetComponent<TMP_InputField>().text = data.position.y.ToString();
        robotSettingsMenu.transform.GetChild(0).GetChild(9).GetComponent<TMP_InputField>().text = data.position.z.ToString();

        robotSettingsMenu.transform.GetChild(0).GetChild(11).GetComponent<TMP_InputField>().text = data.zAngle.ToString();

        robotSettingsMenu.transform.GetChild(0).GetChild(13).GetComponent<TMP_InputField>().text = data.teamNumber.ToString();
    }
    public void EditRobotSettings(int index)
    {
        PopulateRobotSettings(index);
        robotSettingsMenu.SetActive(true);

        robotSettingsMenu.transform.GetChild(0).GetChild(1).GetComponent<ui_button>().onPress.RemoveAllListeners();
        robotSettingsMenu.transform.GetChild(0).GetChild(1).GetComponent<ui_button>().onPress.AddListener(() => UIManager.Instance.GetColorFromPicker((col) => ChangeRobotColor(index, col)));

        robotSettingsMenu.transform.GetChild(0).GetChild(3).GetComponent<TMP_InputField>().onEndEdit.RemoveAllListeners();
        robotSettingsMenu.transform.GetChild(0).GetChild(3).GetComponent<TMP_InputField>().onEndEdit.AddListener((x) => ChangeRobotLength(index, double.Parse(x)));
        robotSettingsMenu.transform.GetChild(0).GetChild(5).GetComponent<TMP_InputField>().onEndEdit.RemoveAllListeners();
        robotSettingsMenu.transform.GetChild(0).GetChild(5).GetComponent<TMP_InputField>().onEndEdit.AddListener((x) => ChangeRobotWidth(index, double.Parse(x)));

        robotSettingsMenu.transform.GetChild(0).GetChild(7).GetComponent<TMP_InputField>().onEndEdit.RemoveAllListeners();
        robotSettingsMenu.transform.GetChild(0).GetChild(7).GetComponent<TMP_InputField>().onEndEdit.AddListener((x) => ChangeRobotX(index, double.Parse(x)));
        robotSettingsMenu.transform.GetChild(0).GetChild(8).GetComponent<TMP_InputField>().onEndEdit.RemoveAllListeners();
        robotSettingsMenu.transform.GetChild(0).GetChild(8).GetComponent<TMP_InputField>().onEndEdit.AddListener((x) => ChangeRobotY(index, double.Parse(x)));
        robotSettingsMenu.transform.GetChild(0).GetChild(9).GetComponent<TMP_InputField>().onEndEdit.RemoveAllListeners();
        robotSettingsMenu.transform.GetChild(0).GetChild(9).GetComponent<TMP_InputField>().onEndEdit.AddListener((x) => ChangeRobotZ(index, double.Parse(x)));

        robotSettingsMenu.transform.GetChild(0).GetChild(11).GetComponent<TMP_InputField>().onEndEdit.RemoveAllListeners();
        robotSettingsMenu.transform.GetChild(0).GetChild(11).GetComponent<TMP_InputField>().onEndEdit.AddListener((x) => ChangeRobotZAngle(index, double.Parse(x)));
        
        robotSettingsMenu.transform.GetChild(0).GetChild(13).GetComponent<TMP_InputField>().onEndEdit.RemoveAllListeners();
        robotSettingsMenu.transform.GetChild(0).GetChild(13).GetComponent<TMP_InputField>().onEndEdit.AddListener((x) => ChangeRobotTeamNumber(index, ushort.Parse(x)));
    }

    public void ChangePathColor(int index, Color col)
    {
        t_lineContainer.GetChild(index).GetComponent<ui_linerenderer>().color = col;
        t_lineContainer.GetChild(index).GetComponent<ui_linerenderer>().Draw();
    }

    // robot spawns when cursor is placed
    public void StartPlacingRobot()
    {
        tx_prompt.gameObject.SetActive(true);
        tx_prompt.text = "LMB to place";

        Transform t_newRobotHeader = Instantiate(p_robotHeader, t_robotHeaderContainer).transform;
        Transform t_newRobot = Instantiate(p_robot, t_robotContainer).transform;
        t_newRobotHeader.localPosition = -Vector3.up * t_newRobotHeader.GetComponent<RectTransform>().sizeDelta.y * (t_robotHeaderContainer.childCount);

        t_newRobot.GetComponent<dsp_robot>().rt_field = transform.GetChild(0).GetComponent<RectTransform>();

        isRobotPlaceMode = true;
        activeIndex = t_robotContainer.childCount - 1;

        int temp = activeIndex;
        t_newRobotHeader.GetChild(1).GetComponent<Button>().onClick.AddListener(() => CloneRobot(temp));
        t_newRobotHeader.GetChild(2).GetComponent<Button>().onClick.AddListener(() => DeleteRobot(temp));
        t_newRobotHeader.GetChild(3).GetComponent<Button>().onClick.AddListener(() => EditRobotSettings(temp));

        t_newRobotHeader.GetChild(0).GetComponent<TextMeshProUGUI>().text = t_robotContainer.childCount.ToString();

        allowedFieldInteraction = false;
    }

    // okay, so the way the user creates paths (right now the only thing the editor is used for):
    // they press a button that enters drawing mode, then they click wherever a point should be (in the order that they appear)
    // then once thats done they can drag around the points, and add more/delete paths
    // I'll have a list, like in unity
    public void StartDrawingPath()
    {
        tx_prompt.gameObject.SetActive(true);
        tx_prompt.text = "LMB to place point \n ENTER to stop drawing";

        isDrawMode = true;

        // we create an object to actually DRAW the path
        Transform t_newPath = Instantiate(p_pathLineRenderer, t_lineContainer).transform;
        Transform t_newPathHeader = Instantiate(p_pathHeader, t_pathHeaderContainer).transform;
        t_newPathHeader.localPosition = -Vector3.up * t_newPathHeader.GetComponent<RectTransform>().sizeDelta.y * (t_pathHeaderContainer.childCount);

        activeIndex = t_lineContainer.transform.childCount - 1; // newest child index

        t_newPath.GetChild(0).gameObject.name = activeIndex.ToString();

        t_newPath.GetComponent<ui_linerenderer>().color = Color.white;

        int temp = activeIndex;
        t_newPathHeader.GetChild(1).GetComponent<Button>().onClick.AddListener(() => StartDrawingPath(temp));
        t_newPathHeader.GetChild(2).GetComponent<Button>().onClick.AddListener(() => ClonePath(temp));
        t_newPathHeader.GetChild(3).GetComponent<Button>().onClick.AddListener(() => DeletePath(temp));
        t_newPathHeader.GetChild(4).GetComponent<Button>().onClick.AddListener(() => ChangeDrawMode(temp));
        t_newPathHeader.GetChild(5).GetComponent<Button>().onClick.AddListener(() => UIManager.Instance.GetColorFromPicker((col) => ChangePathColor(temp, col)));

        t_newPathHeader.GetChild(0).GetComponent<TextMeshProUGUI>().text = t_lineContainer.childCount.ToString();

        // no need to refresh indices after making a path, cuz its appended to the end

        allowedFieldInteraction = false;
    }

    // because of the way we're using child indices, we need a way to refresh them sometimes
    public void RefreshPathIndices()
    {
        // in THEORY the two transforms should have same child count
        for (int i = 0; i < t_lineContainer.childCount; i++)
        {
            Transform t_currentLine = t_lineContainer.GetChild(i);
            Transform t_currentHeader = t_pathHeaderContainer.GetChild(i);

            int temp = i;


            t_currentHeader.GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
            t_currentHeader.GetChild(1).GetComponent<Button>().onClick.AddListener(() => StartDrawingPath(temp));


            t_currentHeader.GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
            t_currentHeader.GetChild(2).GetComponent<Button>().onClick.AddListener(() => ClonePath(temp));


            t_currentHeader.GetChild(3).GetComponent<Button>().onClick.RemoveAllListeners();
            t_currentHeader.GetChild(3).GetComponent<Button>().onClick.AddListener(() => DeletePath(temp));

            t_currentHeader.GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
            t_currentHeader.GetChild(4).GetComponent<Button>().onClick.AddListener(() => ChangeDrawMode(temp));
            
            t_currentHeader.GetChild(5).GetComponent<Button>().onClick.RemoveAllListeners();
            t_currentHeader.GetChild(5).GetComponent<Button>().onClick.AddListener(() => UIManager.Instance.GetColorFromPicker((col) => ChangePathColor(temp, col)));
        }
    }

    public void RefreshRobotIndices()
    {
        for (int i = 0; i < t_robotContainer.childCount; i++)
        {
            Transform t_currentHeader = t_robotHeaderContainer.GetChild(i);

            int temp = i;


            t_currentHeader.GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
            t_currentHeader.GetChild(1).GetComponent<Button>().onClick.AddListener(() => CloneRobot(temp));


            t_currentHeader.GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
            t_currentHeader.GetChild(2).GetComponent<Button>().onClick.AddListener(() => DeleteRobot(temp));

            t_currentHeader.GetChild(3).GetComponent<Button>().onClick.RemoveAllListeners();
            t_currentHeader.GetChild(3).GetComponent<Button>().onClick.AddListener(() => EditRobotSettings(temp));
        }
    }


    // packaging up what we have as a nav_field class, then posting to NT
    public void SendToNT()
    {
        nav_field toSend = new nav_field();

        toSend.robots = new nav_robot[t_robotContainer.childCount];
        toSend.markers = new nav_marker[t_markerContainer.childCount];
        toSend.paths = new nav_path[t_pathHeaderContainer.childCount];

        for (int i = 0; i < t_pathHeaderContainer.childCount; i++)
        {
            nav_path newPath = new nav_path();

            newPath.drawMode = t_lineContainer.GetChild(i).GetComponent<ui_linerenderer>().drawMode;

            newPath.positions = DoubleVector3.ToArray(t_lineContainer.GetChild(i).GetComponent<ui_linerenderer>().points);

            newPath.SetColor(t_lineContainer.GetChild(i).GetComponent<ui_linerenderer>().color);

            for (int n = 0; n < newPath.positions.Length; n++)
            {
                newPath.positions[n] = newPath.positions[n].Mul(GetComponent<dsp_field>().PixelsToMeters());
            }

            toSend.paths[i] = newPath;
        }
        for (int i = 0; i < t_robotContainer.childCount; i++)
        {
            nav_robot newRobot = t_robotContainer.GetChild(i).GetComponent<dsp_robot>().data;

            toSend.robots[i] = newRobot;
        }
        for (int i = 0; i < t_markerContainer.childCount; i++)
        {
            nav_marker newMarker = t_markerContainer.GetChild(i).GetComponent<dsp_marker>().data;

            toSend.markers[i] = newMarker;
        }

        string result = toSend.EncodeToString();

        NetworkManager.Instance.SetValueAt(GetComponent<dsp_field>().ntKey, result);
        NetworkManager.Instance.Pull();

        // for convinience
        Close();

        // we want to now not only save this particular NT value to the layout file,
        // we also want to flag it so it's kept track of

        NetworkManager.Instance.flaggedKeys.Add(GetComponent<dsp_field>().ntKey);
        DisplayManager.Instance.ForceSaveCurrentLayoutToDisk();
    }

    public void ChangeDrawMode(int index)
    {
        t_lineContainer.GetChild(index).GetComponent<ui_linerenderer>().AdvancePolyDrawMode();
        t_pathHeaderContainer.GetChild(index).GetChild(4).GetComponent<Image>().sprite = drawModeButtons[t_lineContainer.GetChild(index).GetComponent<ui_linerenderer>().drawMode];

    }

    public void ClonePath(int index)
    {
        Debug.Log("Cloning path...");

        StartDrawingPath(); // just to handle the initialization, don't like copy+paste yk?

        t_lineContainer.GetChild(activeIndex).GetComponent<ui_linerenderer>().Draw(t_lineContainer.GetChild(index).GetComponent<ui_linerenderer>().points);

        isDrawMode = false;
    }
    public void CloneRobot(int index)
    {
        Debug.Log("Cloning bot...");

        StartPlacingRobot();

        t_robotContainer.GetChild(activeIndex).GetComponent<dsp_robot>().data = new nav_robot();
        nav_robot robotData = t_robotContainer.GetChild(activeIndex).GetComponent<dsp_robot>().data;
        robotData.zAngle = 0;

        t_robotContainer.GetChild(activeIndex).GetComponent<dsp_robot>().metersToPixelsConversionFactor = GetComponent<dsp_field>().MetersToPixels();

        robotData.color = Color.white;

        robotData.length = 0.88;
        robotData.width = 0.88;
        robotData.teamNumber = 2386;

        t_robotContainer.GetChild(activeIndex).GetComponent<dsp_robot>().data.position = t_robotContainer.GetChild(index).GetComponent<dsp_robot>().data.position;
        t_robotContainer.GetChild(activeIndex).GetComponent<dsp_robot>().Draw();

        isRobotPlaceMode = false;
    }

    public void DeletePath(int index)
    {
        Debug.Log("Deleting path...");

        // its nice not having lists of paths, no need to RemoveAt()

        Destroy(t_lineContainer.GetChild(index).gameObject);
        Destroy(t_pathHeaderContainer.GetChild(index).gameObject);

        RefreshPathIndices(); // important
    }
    public void DeleteRobot(int index)
    {
        Debug.Log("Deleting bot...");

        Destroy(t_robotContainer.GetChild(index).gameObject);
        Destroy(t_robotHeaderContainer.GetChild(index).gameObject);

        RefreshRobotIndices();
    }

    // instead of starting a path from scratch, here we are re-drawing one
    public void StartDrawingPath(int index)
    {
        tx_prompt.gameObject.SetActive(true);
        tx_prompt.text = "LMB to place point \n ENTER to stop drawing";
        activeIndex = index;
        isDrawMode = true;

        t_lineContainer.GetChild(activeIndex).GetComponent<ui_linerenderer>().Draw(new Vector2[0]);
    }

    public void StopDrawingPath() {
        isDrawMode = false;
        tx_prompt.gameObject.SetActive(false);
    }

    void Update()
    {
        GetComponent<dsp_field>().rt_packageButton.gameObject.SetActive(isEditing);

        if (allowedFieldInteraction)
        {
            if (isDrawMode && Sys.IsPointInBounds(GetComponent<dsp_field>().img, Input.mousePosition))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    t_lineContainer.transform.position = GetComponent<dsp_field>().activeImg.position - new Vector3(GetComponent<dsp_field>().activeImg.sizeDelta.x / 2f, GetComponent<dsp_field>().activeImg.sizeDelta.y / 2f, 0);

                    // the existing copy of the points array
                    List<Vector2> existingPoints = t_lineContainer.GetChild(activeIndex).GetComponent<ui_linerenderer>().points.ToList();

                    // could have done this with a loop, but its less verbose to just go from array -> list -> array
                    existingPoints.Add(Input.mousePosition - t_lineContainer.transform.position);

                    t_lineContainer.GetChild(activeIndex).GetComponent<ui_linerenderer>().Draw(existingPoints.ToArray());
                }

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    StopDrawingPath();
                }
            }
            if (isRobotPlaceMode)
            {
                if (t_robotContainer.GetChild(activeIndex).GetComponent<dsp_robot>().data.length == 0)
                {
                    t_robotContainer.GetChild(activeIndex).GetComponent<dsp_robot>().data = new nav_robot();
                    nav_robot robotData = t_robotContainer.GetChild(activeIndex).GetComponent<dsp_robot>().data;
                    robotData.zAngle = 0;

                    t_robotContainer.GetChild(activeIndex).GetComponent<dsp_robot>().metersToPixelsConversionFactor = GetComponent<dsp_field>().MetersToPixels();

                    robotData.color = Color.white;

                    robotData.length = 0.88;
                    robotData.width = 0.88;
                    robotData.teamNumber = 2386;
                }

                t_robotContainer.GetChild(activeIndex).GetComponent<dsp_robot>().data.position = new DoubleVector3(Input.mousePosition - GetComponent<dsp_field>().GetOrigin()).Mul(GetComponent<dsp_field>().PixelsToMeters());
                t_robotContainer.GetChild(activeIndex).GetComponent<dsp_robot>().Draw();

                t_robotContainer.GetChild(activeIndex).GetComponent<dsp_robot>().data.zAngle += Input.GetAxis("Mouse ScrollWheel") * 30;

                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return))
                {
                    isRobotPlaceMode = false;
                }
            }
            if (isMarkerPlaceMode)
            {
                if (t_markerContainer.GetChild(activeIndex).GetComponent<dsp_marker>().data.zAngle == 0)
                {
                    t_markerContainer.GetChild(activeIndex).GetComponent<dsp_marker>().data = new nav_marker();
                    nav_marker markerData = t_markerContainer.GetChild(activeIndex).GetComponent<dsp_marker>().data;
                    markerData.zAngle = 0;
                    markerData.size = 1;
                    markerData.displayIndex = 0;
                    markerData.color = Color.white;

                    t_markerContainer.GetChild(activeIndex).GetComponent<dsp_marker>().metersToPixelsConversionFactor = GetComponent<dsp_field>().MetersToPixels();
                }

                t_markerContainer.GetChild(activeIndex).GetComponent<dsp_marker>().data.position = new DoubleVector3(Input.mousePosition - GetComponent<dsp_field>().GetOrigin()).Mul(GetComponent<dsp_field>().PixelsToMeters());
                t_markerContainer.GetChild(activeIndex).GetComponent<dsp_marker>().Draw();

                t_markerContainer.GetChild(activeIndex).GetComponent<dsp_marker>().data.zAngle += Input.GetAxis("Mouse ScrollWheel") * 30;

                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return))
                {
                    isMarkerPlaceMode = false;
                }
            }
        }
        else { allowedFieldInteraction = true; }
    }
}
