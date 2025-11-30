using UnityEngine.UI;
using UnityEngine;

public class dsp_field : MonoBehaviour
{
    public float imgWidth;
    public float imgHeight;

    public RectTransform imgMask;
    public RectTransform img;
    public RectTransform activeImg;
    private Vector2 imgToActiveRatio;

    public string ntKey;

    private nav_field fieldData;

    public GameObject p_robot;
    public Transform t_robotContainer;

    public float fieldScaleMultiplier;
    public float zoomSpeed;

    public float textureRotationAngle;

    public RectTransform[] rt_rotationButtons;
    public RectTransform rt_editButton;
    public RectTransform rt_packageButton;
    public RectTransform rt_stopDrawingButton;

    public GameObject p_lineRenderer;
    public Transform t_pathContainer;
    public Transform t_markerContainer;
    public GameObject p_marker;

    public void Initialize()
    {
        imgWidth = 2048;
        imgHeight = 1024;

        fieldScaleMultiplier = 1;
        zoomSpeed = 0.5f;

        transform.parent.GetComponent<dsp_mainvisual>().updateAction = (x) => UpdateData(x);

        imgToActiveRatio = new Vector2(activeImg.sizeDelta.x / 2048f, activeImg.sizeDelta.y / 1024f);

        // the rotation buttons
        rt_rotationButtons = new RectTransform[4];
        // risky to use indices but whatever
        transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => SetRotation(0)); // blue left, red right
        rt_rotationButtons[0] = transform.GetChild(3).GetComponent<RectTransform>();
        transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => SetRotation(90)); // blue bottom
        rt_rotationButtons[1] = transform.GetChild(4).GetComponent<RectTransform>();
        transform.GetChild(5).GetComponent<Button>().onClick.AddListener(() => SetRotation(180)); // red left, blue right
        rt_rotationButtons[2] = transform.GetChild(5).GetComponent<RectTransform>();
        transform.GetChild(6).GetComponent<Button>().onClick.AddListener(() => SetRotation(270)); // blue top, red bottom
        rt_rotationButtons[3] = transform.GetChild(6).GetComponent<RectTransform>();

        GetComponent<dsp_fieldeditor>().Close();
    }

    public Vector3 GetOrigin()
    {
        Vector2 v = ConvertToPixels(new Vector2(0, 0));
        return activeImg.position + new Vector3(v.x, v.y, 0);
    }

    // edit mode is where you setup a nav_field on the dashboard side, and then its available to the robot for use
    // (basically the reverse of how things normally work)
    // its held in a separate class for clarity (dsp_fieldeditor.cs)
    public void ToggleEditMode()
    {
        if (GetComponent<dsp_fieldeditor>().g_parentObj.activeSelf)
        {
            GetComponent<dsp_fieldeditor>().Close();
        }
        else
        {
            GetComponent<dsp_fieldeditor>().Open();
        }
    }

    public void PostCoords(Vector3 coords)
    {
        string existingQueue = NetworkManager.Instance.latestData.GetValueAt("$field_commands");
        existingQueue += "{" + new DoubleVector3(coords).EncodeToString() + "}";

        NetworkManager.Instance.SetValueAt("$field_commands", existingQueue);
    }

    // gotta be a function for buttons to use
    public void SetRotation(float theta)
    {
        textureRotationAngle = theta;
    }

    // may want to assign this to a UnityAction
    void Update()
    {
        RectTransform parentRect = transform.parent.GetComponent<RectTransform>();

        // TODO: NOT DO THIS IN Update()!
        ntKey = transform.parent.GetComponent<dsp_mainvisual>().ntKey;

        Vector2 parentSize = parentRect.sizeDelta;

        for (int i = 0; i < rt_rotationButtons.Length; i++)
        {
            rt_rotationButtons[i].position = new Vector3(rt_rotationButtons[i].sizeDelta.x / 2f, rt_rotationButtons[i].sizeDelta.y / 2f, 0) + parentRect.position - new Vector3(parentRect.sizeDelta.x, parentRect.sizeDelta.y, 0) / 2f + Vector3.right * i * (rt_rotationButtons[i].sizeDelta.x + 10);
        }
        rt_editButton.position = new Vector3(rt_editButton.sizeDelta.x / 2f, -rt_editButton.sizeDelta.y / 2f, 0) + parentRect.position - new Vector3(parentRect.sizeDelta.x, -parentRect.sizeDelta.y, 0) / 2f;
        rt_packageButton.position = new Vector3(rt_packageButton.sizeDelta.x * 1.5f, -rt_packageButton.sizeDelta.y / 2f, 0) + parentRect.position - new Vector3(parentRect.sizeDelta.x, -parentRect.sizeDelta.y, 0) / 2f;
        rt_stopDrawingButton.position = new Vector3(rt_stopDrawingButton.sizeDelta.x * 2.5f, -rt_stopDrawingButton.sizeDelta.y / 2f, 0) + parentRect.position - new Vector3(parentRect.sizeDelta.x, -parentRect.sizeDelta.y, 0) / 2f;

        transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, textureRotationAngle);

        imgMask.sizeDelta = new Vector2(parentSize.x, parentSize.y);
        if (parentSize.x < parentSize.y * (imgWidth / imgHeight))
        {
            img.sizeDelta = new Vector2(parentSize.x, parentSize.x * imgHeight / imgWidth) * fieldScaleMultiplier;
            activeImg.sizeDelta = new Vector2(img.sizeDelta.x * imgToActiveRatio.x, img.sizeDelta.y * imgToActiveRatio.y);
        }
        else
        {
            img.sizeDelta = new Vector2(parentSize.y * imgWidth / imgHeight, parentSize.y);
            activeImg.sizeDelta = new Vector2(img.sizeDelta.x * imgToActiveRatio.x, img.sizeDelta.y * imgToActiveRatio.y);
        }

        if (CanvasUtils.IsCursorInteract(imgMask.gameObject, true))
        {
            fieldScaleMultiplier += Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;

            if (Input.GetMouseButtonDown(0))
            {
                Vector3 rawMouseCoords = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
                rawMouseCoords = rawMouseCoords - activeImg.transform.position;
                rawMouseCoords += new Vector3(activeImg.sizeDelta.x / 2f, activeImg.sizeDelta.y / 2f, 0);
                rawMouseCoords *= (float)PixelsToMeters();
                PostCoords(rawMouseCoords);
            }
            if (Input.GetKeyDown("c"))
            {
                NetworkManager.Instance.SetValueAt("$field_commands", "");
            }
        }
    }

    void Draw()
    {
        for (int i = 0; i < fieldData.robots.Length; i++)
        {

            if (t_robotContainer.childCount <= i)
            {
                // need a new robot
                GameObject g_newRobot = Instantiate(p_robot, Vector3.zero, Quaternion.identity);
                g_newRobot.transform.SetParent(t_robotContainer);

                dsp_robot comp = g_newRobot.GetComponent<dsp_robot>();

                comp.rt_field = transform.GetChild(0).GetComponent<RectTransform>();

                comp.data = fieldData.robots[i];
                comp.metersToPixelsConversionFactor = MetersToPixels();
                comp.r_fieldImg = img;
                comp.Draw();
            }
            else
            {
                // already a robot here
                GameObject g_existingRobot = t_robotContainer.GetChild(i).gameObject;

                dsp_robot comp = g_existingRobot.GetComponent<dsp_robot>();

                comp.rt_field = transform.GetChild(0).GetComponent<RectTransform>();

                comp.data = fieldData.robots[i];
                comp.metersToPixelsConversionFactor = MetersToPixels();
                comp.r_fieldImg = img;
                comp.Draw();
            }
        }
        // clearing any uneeded robots
        for (int i = t_robotContainer.childCount - 1; i >= fieldData.robots.Length; i--)
        {
            Destroy(t_robotContainer.GetChild(i).gameObject);
        }

        for (int i = 0; i < fieldData.paths.Length; i++)
        {
            // so paths need two parts, an image for each point and an image for the lines
            // as opposed to handling this all here, we may as well have our own class for it

            if (t_pathContainer.childCount <= i)
            {
                // need a new robot
                GameObject g_newPath = Instantiate(p_lineRenderer, Vector3.zero, Quaternion.identity);
                g_newPath.transform.SetParent(t_pathContainer);

                g_newPath.transform.position = activeImg.position;

                ui_linerenderer comp = g_newPath.GetComponent<ui_linerenderer>();

                //Debug.Log(i + "   " + fieldData.paths[i].segmentColor);
                comp.color = fieldData.paths[i].segmentColor;

                comp.points = new Vector2[fieldData.paths[i].positions.Length];
                comp.drawMode = fieldData.paths[i].drawMode;
                comp.ApplyDrawMode();
                for (int j = 0; j < fieldData.paths[i].positions.Length; j++)
                {
                    comp.points[j] = ConvertToPixels(new Vector2((float)fieldData.paths[i].positions[j].x, (float)fieldData.paths[i].positions[j].y));
                }
                comp.Draw();
            }
            else
            {
                // already a path here
                GameObject g_existingPath = t_pathContainer.GetChild(i).gameObject;

                g_existingPath.transform.position = activeImg.position;

                ui_linerenderer comp = g_existingPath.GetComponent<ui_linerenderer>();

                comp.color = fieldData.paths[i].segmentColor;

                comp.points = new Vector2[fieldData.paths[i].positions.Length];
                comp.drawMode = fieldData.paths[i].drawMode;
                comp.ApplyDrawMode();
                for (int j = 0; j < fieldData.paths[i].positions.Length; j++)
                {
                    comp.points[j] = ConvertToPixels(new Vector2((float)fieldData.paths[i].positions[j].x, (float)fieldData.paths[i].positions[j].y));
                }
                comp.Draw();
            }
        }
        // clearing any uneeded robots
        for (int i = t_pathContainer.childCount - 1; i >= fieldData.paths.Length; i--)
        {
            Destroy(t_pathContainer.GetChild(i).gameObject);
        }

        for (int i = 0; i < fieldData.markers.Length; i++)
        {
            if (t_markerContainer.childCount <= i)
            {
                // need a new robot
                GameObject g_newMarker = Instantiate(p_marker, t_markerContainer);

                dsp_marker comp = g_newMarker.GetComponent<dsp_marker>();

                comp.rt_field = transform.GetChild(0).GetComponent<RectTransform>();
                comp.data = fieldData.markers[i];

                comp.metersToPixelsConversionFactor = MetersToPixels();
                comp.r_fieldImg = img;
                comp.Draw();
            }
            else
            {
                // already a robot here
                GameObject g_existingMarker = t_markerContainer.GetChild(i).gameObject;

                dsp_marker comp = g_existingMarker.GetComponent<dsp_marker>();

                comp.rt_field = transform.GetChild(0).GetComponent<RectTransform>();
                comp.data = fieldData.markers[i];

                comp.metersToPixelsConversionFactor = MetersToPixels();
                comp.r_fieldImg = img;
                comp.Draw();
            }
        }

        for (int i = t_markerContainer.childCount - 1; i >= fieldData.markers.Length; i--)
        {
            Destroy(t_markerContainer.GetChild(i).gameObject);
        }
    }

    public Vector2 ConvertToPixels(Vector2 raw) {
        return Sys.AdjustVectorXY(new Vector3(activeImg.sizeDelta.x / 2f, activeImg.sizeDelta.y / 2f, 0) * -1
        + new Vector3((float)(raw.x * MetersToPixels()), (float)(raw.y * MetersToPixels()), 0), textureRotationAngle);
    }

    double MetersToPixels(double input)
    {
        return input / (8.0518f / (img.sizeDelta.y));
    }
    public double MetersToPixels()
    {
        return activeImg.sizeDelta.y / 8.0518f;
    }
    public double PixelsToMeters()
    {
        return 8.0518f / activeImg.sizeDelta.y;
    }

    void UpdateData(net_ntdatalist data)
    {
        //Debug.Log(data.dataKeys);
        if (data.dataKeys == null) { return; }
        for (int i = 0; i < data.dataKeys.Length; i++)
        {

            // ... okay, so this requires some explaining
            
            if (data.dataKeys[i] == ntKey)
            {
                string className = "";
                if (Sys.FindFirstOccurance(data.dataValues[i], '(') != -1)
                {
                    className = data.dataValues[i].Substring(1, Sys.FindFirstOccurance(data.dataValues[i], '(') - 1);
                }

                if (className == "field")
                {
                    fieldData = nav_field.DecodeFromString(data.dataValues[i]);

                    Draw();

                    return;
                }
                else if (className.Length > 0)
                {
                    fieldData = new nav_field();
                    fieldData.robots = new nav_robot[1];
                    fieldData.paths = new nav_path[0];
                    fieldData.markers = new nav_marker[0];

                    fieldData.robots[0] = nav_robot.DecodeFromString(data.dataValues[i]);

                    Draw();

                    return;
                }
            }
        }

        // only runs if data is not found
        nav_field nullData = new nav_field();

        nullData.robots = new nav_robot[0];
        nullData.markers = new nav_marker[0];
        nullData.paths = new nav_path[0];

        fieldData = nullData;
        Draw();
    }
}
