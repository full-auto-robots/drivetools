using UnityEditor;
using UnityEngine;

public class dsp_field3d : MonoBehaviour
{
    public RectTransform rt_img;
    public RectTransform rt_mask;
    public FieldCamera fieldCam;

    public Transform t_robotContainer;
    public Transform t_pathContainer;
    public Transform t_markerContainer;

    public float imgWidth;
    public float imgHeight;
    public string ntKey;

    private nav_field fieldData;
    public GameObject p_robot;
    public GameObject p_marker;

    public GameObject p_lineRenderer;

    private bool isInitialized;

    public void Initialize()
    {
        fieldCam = GameObject.Find("field camera").GetComponent<FieldCamera>();

        t_robotContainer = GameObject.Find("3d field").transform.GetChild(0);
        t_pathContainer = GameObject.Find("3d field").transform.GetChild(1);
        t_markerContainer = GameObject.Find("3d field").transform.GetChild(2);

        transform.parent.GetComponent<dsp_mainvisual>().updateAction = (x) => UpdateData(x);

        isInitialized = true;
    }

    void Update()
    {
        // TODO: NOT DO THIS IN Update()!
        ntKey = transform.parent.GetComponent<dsp_mainvisual>().ntKey;

        if (!isInitialized) return;

        RectTransform parentRect = transform.parent.GetComponent<RectTransform>();
        Vector2 parentSize = parentRect.sizeDelta;

        rt_mask.sizeDelta = parentRect.sizeDelta;
        if (parentSize.x < parentSize.y * (imgWidth / imgHeight))
        {
            rt_img.sizeDelta = new Vector2(parentSize.x, parentSize.x * imgHeight / imgWidth);
        }
        else
        {
            rt_img.sizeDelta = new Vector2(parentSize.y * imgWidth / imgHeight, parentSize.y);
        }

        if (Input.GetKey(KeyCode.LeftShift) && CanvasUtils.IsCursorInteract(parentRect.gameObject, true))
        {
            fieldCam.isBeingControlled = true;
        }
        else
        {
            fieldCam.isBeingControlled = false;
        }
    }

    void Draw()
    {
        // okay, so a lot of this code is the same as whats in dsp_field.cs (the field2d display)
        // not ALL of it is the same, there are subtle differences,
        // but still I really dislike having this be so close to copy + paste
        // it feels like a really ugly sol'n


        for (int i = 0; i < fieldData.robots.Length; i++)
        {
            if (t_robotContainer.childCount <= i)
            {
                // need a new robot
                GameObject g_newRobot = Instantiate(p_robot, Vector3.zero, Quaternion.identity);
                g_newRobot.transform.SetParent(t_robotContainer);

                Debug.Log(fieldData.robots[i].position.ToVector3());

                Vector3 rawPos = fieldData.robots[i].position.ToVector3();
                g_newRobot.transform.position = new Vector3(rawPos.x, rawPos.z, rawPos.y);

                g_newRobot.transform.eulerAngles = new Vector3(0, (float)-fieldData.robots[i].zAngle, 0);
            }
            else
            {
                // already a robot here
                GameObject g_existingRobot = t_robotContainer.GetChild(i).gameObject;

                Vector3 rawPos = fieldData.robots[i].position.ToVector3();
                g_existingRobot.transform.position = new Vector3(rawPos.x, rawPos.z, rawPos.y);

                g_existingRobot.transform.eulerAngles = new Vector3(0, (float)-fieldData.robots[i].zAngle, 0);
            }
        }
        // clearing any uneeded robots
        for (int i = t_robotContainer.childCount - 1; i >= fieldData.robots.Length; i--)
        {
            Destroy(t_robotContainer.GetChild(i).gameObject);
        }

        // *** PATHS: ***
        for (int i = 0; i < fieldData.paths.Length; i++)
        {
            // so paths need two parts, an image for each point and an image for the lines
            // as opposed to handling this all here, we may as well have our own class for it

            if (t_pathContainer.childCount <= i)
            {
                // need a new robot
                GameObject g_newPath = Instantiate(p_lineRenderer, Vector3.zero, Quaternion.identity);
                g_newPath.transform.SetParent(t_pathContainer);

                LineRenderer comp = g_newPath.GetComponent<LineRenderer>();
                comp.startWidth = 0.2f;
                comp.endWidth = 0.2f;

                if (fieldData.paths[i].drawMode == (ushort)LineDisplayMode.LoopedLine) {
                    comp.positionCount = fieldData.paths[i].positions.Length + 1;
                    Vector3[] pos = new Vector3[fieldData.paths[i].positions.Length + 1];

                    Vector3 raw;
                    for (int j = 0; j < fieldData.paths[i].positions.Length; j++)
                    {
                        raw = fieldData.paths[i].positions[j].ToVector3();
                        pos[j] = new Vector3(raw.x, raw.z + 1, raw.y);
                    }
                    raw = fieldData.paths[i].positions[0].ToVector3();
                    pos[fieldData.paths[i].positions.Length] = new Vector3(raw.x, raw.z + 1, raw.y);

                    comp.SetPositions(pos);
                } else {
                    comp.positionCount = fieldData.paths[i].positions.Length;
                    comp.SetPositions(DoubleVector3.ToVector3Array(fieldData.paths[i].positions));
                }
            }
            else
            {
                // already a path here
                GameObject g_existingPath = t_pathContainer.GetChild(i).gameObject;

                LineRenderer comp = g_existingPath.GetComponent<LineRenderer>();
                comp.startWidth = 0.2f;
                comp.endWidth = 0.2f;

                if (fieldData.paths[i].drawMode == (ushort)LineDisplayMode.LoopedLine) {
                    comp.positionCount = fieldData.paths[i].positions.Length + 1;
                    Vector3[] pos = new Vector3[fieldData.paths[i].positions.Length + 1];

                    Vector3 raw;
                    for (int j = 0; j < fieldData.paths[i].positions.Length; j++)
                    {
                        raw = fieldData.paths[i].positions[j].ToVector3();
                        pos[j] = new Vector3(raw.x, raw.z + 1, raw.y);
                    }
                    raw = fieldData.paths[i].positions[0].ToVector3();
                    pos[fieldData.paths[i].positions.Length] = new Vector3(raw.x, raw.z + 1, raw.y);

                    comp.SetPositions(pos);
                } else {
                    comp.positionCount = fieldData.paths[i].positions.Length;
                    comp.SetPositions(DoubleVector3.ToVector3Array(fieldData.paths[i].positions));
                }
            }
        }
        // clearing any uneeded paths
        for (int i = t_pathContainer.childCount - 1; i >= fieldData.paths.Length; i--)
        {
            Destroy(t_pathContainer.GetChild(i).gameObject);
        }

        // *** MARKERS: ***
        for (int i = 0; i < fieldData.markers.Length; i++)
        {
            // so paths need two parts, an image for each point and an image for the lines
            // as opposed to handling this all here, we may as well have our own class for it

            if (t_markerContainer.childCount <= i)
            {
                // need a new marker
                GameObject g_newMarker = Instantiate(p_marker, Vector3.zero, Quaternion.identity);
                g_newMarker.transform.SetParent(t_markerContainer);

                Vector3 v = fieldData.markers[i].position.ToVector3();
                g_newMarker.transform.position = new Vector3(v.x, v.z, v.y);
            }
            else
            {
                // already a marker here
                GameObject g_existingMarker = t_markerContainer.GetChild(i).gameObject;

                Vector3 v = fieldData.markers[i].position.ToVector3();
                g_existingMarker.transform.position = new Vector3(v.x, v.z, v.y);
            }
        }
        // clearing any uneeded markers
        for (int i = t_markerContainer.childCount - 1; i >= fieldData.markers.Length; i--)
        {
            Destroy(t_markerContainer.GetChild(i).gameObject);
        }
    }
    
    void UpdateData(net_ntdatalist data)
    {
        for (int i = 0; i < data.dataKeys.Length; i++)
        {
            if (data.dataKeys[i] == ntKey)
            {
                fieldData = nav_field.DecodeFromString(data.dataValues[i]);

                Draw();

                return;
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
