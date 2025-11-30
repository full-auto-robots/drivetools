using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// basically, this script is responsible for drawing the display object,
// given the positions of its four corners

public class dsp_mainvisual : MonoBehaviour
{
    public string ntKey;
    public bool isLocked;
    private Vector2 bottomLeft;
    private Vector2 bottomRight;

    private Vector2 topLeft;
    private Vector2 topRight;
    // there are technically 9 points associated with a display, but only four need be used to figure out its pose

    public UnityAction<net_ntdatalist> updateAction; // to be called by DisplayManager.cs to push data to be updated


    // tbh this was just the first solution I thought of,
    // having the individual dsp_ components put their data here so the display editing menu can use it

    // these lists must be synced!!
    public List<GameObject> customEditingComps; // prefabs
    public List<string> customEditingNames; // just the names for the above GameObjects once initialized
    public List<UnityAction<GameObject>> onceInitialize; // this is dumb, but it's A way (not the BEST way) of getting gameObject references back to the node

    // fed directly to genericdata
    public List<string> propertyNames;
    public List<string> propertyValues;

    void Awake()
    {
        propertyNames = new List<string>();
        propertyValues = new List<string>();
    }

    // this should be called AFTER the interaction stuff has been handled
    public void SetPoints(Vector2 bottomLeft, Vector2 bottomRight, Vector2 topLeft, Vector2 topRight)
    {
        this.bottomLeft = bottomLeft;
        this.bottomRight = bottomRight;
        this.topLeft = topLeft;
        this.topRight = topRight;

        Render();
    }

    void Render()
    {
        Vector2 center = bottomLeft + (topRight - bottomLeft) / 2;
        Vector3 difference = new Vector3(center.x, center.y, 0) - transform.position;

        transform.position += difference;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (i < 9)
            {
                transform.GetChild(i).position -= difference;
            }
            else
            {
                transform.GetChild(i).localPosition = Vector3.zero;
            }
        }

        GetComponent<RectTransform>().sizeDelta = new Vector2(bottomRight.x - bottomLeft.x, topRight.y - bottomRight.y);
    }

    void Update()
    {
        if (CanvasUtils.IsCursorInteract(gameObject, true) && !Input.GetKey(KeyCode.LeftShift) && !DisplayManager.Instance.areDisplaysLocked)
        {
            // moving/dragging the whole node with left click
            if (Input.GetMouseButtonDown(0) && GetComponent<dsp_pointhandler>().grabIndex == -1)
            {
                transform.GetChild(0).GetComponent<dsp_point>().isGrabbed = true;
                transform.GetChild(0).GetComponent<dsp_point>().carryOffset = transform.GetChild(0).position - Input.mousePosition;
                transform.GetChild(0).GetComponent<dsp_point>().SetParentIndex();
            }
            //
            if (Input.GetMouseButtonDown(1))
            {
                DisplayManager.Instance.OpenNodeOptions(this);
            }
        }
    }
}
