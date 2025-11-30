using System.Collections.Generic;
using UnityEngine;

public class dsp_pointhandler : MonoBehaviour
{
    public int grabIndex;

    public Vector3[] positions; // one for each point
    public bool isPresent; // basically, does this display have a collider
    public bool isFullScreen;
    private Vector3[] savedPositions;

    private bool clickedAlready;
    public bool lockProportions;
    public float lockPointIndex;

    private float heightToWidthRatio;

    void Awake()
    {
        isPresent = true;
        grabIndex = -1;
    }

    public void SetAsFullscreen()
    {
        isPresent = false;
        for (int i = 0; i < positions.Length; i++) savedPositions[i] = positions[i];

        positions[5] = new Vector3(0, Screen.height, 0);
        positions[6] = new Vector3(Screen.width, Screen.height, 0);
        positions[7] = new Vector3(0, 0, 0);
        positions[8] = new Vector3(Screen.width, 0, 0);

        DistributePoints();

        isFullScreen = true;
    }

    public void ExitFullscreen()
    {
        isPresent = true;
        for (int i = 0; i < positions.Length; i++) positions[i] = savedPositions[i];

        DistributePoints();
        isFullScreen = false;
    }

    public void Initialize()
    {
        positions = new Vector3[9];
        savedPositions = new Vector3[9];
        for (int i = 0; i < 9; i++)
        {
            positions[i] = transform.GetChild(i).position;
        }
    }

    public bool IsPointInside(Vector2 point)
    {
        return point.x > positions[7].x + 5 && point.x < positions[8].x - 5 && point.y > positions[7].y + 5 && point.y < positions[5].y - 5;
    }

    void Update()
    {
        // this one kind of has to be local, even though its annoying
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitFullscreen();
        }

        // this too, the double-clicking to enter fullscreen
        if (Input.GetMouseButtonDown(0) && clickedAlready)
        {
            SetAsFullscreen();
        }
    }

    public void SetRatio()
    {
        heightToWidthRatio = (positions[5].y - positions[7].y) / (positions[6].x - positions[5].x);
    }

    public void HandleIntersects(List<GameObject> otherDisplays)
    {
        if (grabIndex != -1) { return; }
        if (!isPresent) { return; }
        for (int i = 0; i < otherDisplays.Count; i++)
        {
            if (!otherDisplays[i].GetComponent<dsp_pointhandler>().isPresent) { continue; }
            if (otherDisplays[i].GetComponent<dsp_pointhandler>().grabIndex != -1) { continue; }
            for (int p = 1; p <= 4; p++)
            {
                if (IsPointInside(otherDisplays[i].GetComponent<dsp_pointhandler>().positions[p]))
                {
                    if (p == 2)
                    {
                        positions[5] = new Vector3(otherDisplays[i].GetComponent<dsp_pointhandler>().positions[p].x, positions[5].y, positions[5].z);
                        positions[7] = new Vector3(otherDisplays[i].GetComponent<dsp_pointhandler>().positions[p].x, positions[7].y, positions[7].z);
                    }
                    else if (p == 1)
                    {
                        positions[6] = new Vector3(otherDisplays[i].GetComponent<dsp_pointhandler>().positions[p].x, positions[6].y, positions[6].z);
                        positions[8] = new Vector3(otherDisplays[i].GetComponent<dsp_pointhandler>().positions[p].x, positions[8].y, positions[8].z);
                    }
                    else if (p == 4)
                    {
                        positions[5] = new Vector3(positions[5].x, otherDisplays[i].GetComponent<dsp_pointhandler>().positions[p].y, positions[5].z);
                        positions[6] = new Vector3(positions[6].x, otherDisplays[i].GetComponent<dsp_pointhandler>().positions[p].y, positions[6].z);
                    }
                    else if (p == 3)
                    {
                        positions[8] = new Vector3(positions[8].x, otherDisplays[i].GetComponent<dsp_pointhandler>().positions[p].y, positions[8].z);
                        positions[7] = new Vector3(positions[7].x, otherDisplays[i].GetComponent<dsp_pointhandler>().positions[p].y, positions[7].z);
                    }
                }
            }
        }

        DistributePoints();
    }

    public void DistributePoints()
    {
        // the corners
        transform.GetChild(5).position = positions[5];
        transform.GetChild(6).position = positions[6];
        transform.GetChild(7).position = positions[7];
        transform.GetChild(8).position = positions[8];

        transform.GetChild(0).position = positions[5] + (positions[8] - positions[5]) / 2;
        positions[0] = positions[5] + (positions[8] - positions[5]) / 2;

        transform.GetChild(1).position = positions[5] + (positions[7] - positions[5]) / 2;
        positions[1] = positions[5] + (positions[7] - positions[5]) / 2;

        transform.GetChild(2).position = positions[6] + (positions[8] - positions[6]) / 2;
        positions[2] = positions[6] + (positions[8] - positions[6]) / 2;

        transform.GetChild(3).position = positions[5] + (positions[6] - positions[5]) / 2;
        positions[3] = positions[5] + (positions[6] - positions[5]) / 2;

        transform.GetChild(4).position = positions[7] + (positions[8] - positions[7]) / 2;
        positions[4] = positions[7] + (positions[8] - positions[7]) / 2;
    }

    public void HandlePositions()
    {
        // how these are set up depends on whether we're scaling proportionally or not

        if (grabIndex == 0)
        {
            Vector3 difference = transform.GetChild(0).position - positions[0];
            for (int i = 1; i < 9; i++)
            {
                transform.GetChild(i).position += difference;
                positions[i] = transform.GetChild(i).position;
            }
            positions[0] = transform.GetChild(0).position;
        }
        else if (grabIndex == 1)
        {
            Vector3 difference = transform.GetChild(1).position - positions[1];

            transform.GetChild(5).position += new Vector3(difference.x, 0, 0);
            positions[5] = transform.GetChild(5).position;
            transform.GetChild(7).position += new Vector3(difference.x, 0, 0);
            positions[7] = transform.GetChild(7).position;

            transform.GetChild(1).position = new Vector3(transform.GetChild(1).position.x, positions[1].y, positions[1].z);
            positions[1] = transform.GetChild(1).position;
        }
        else if (grabIndex == 2)
        {
            Vector3 difference = transform.GetChild(2).position - positions[2];

            transform.GetChild(6).position += new Vector3(difference.x, 0, 0);
            positions[6] = transform.GetChild(6).position;
            transform.GetChild(8).position += new Vector3(difference.x, 0, 0);
            positions[8] = transform.GetChild(8).position;

            transform.GetChild(2).position = new Vector3(transform.GetChild(2).position.x, positions[2].y, positions[2].z);
            positions[2] = transform.GetChild(2).position;
        }
        else if (grabIndex == 3)
        {
            Vector3 difference = transform.GetChild(3).position - positions[3];

            transform.GetChild(6).position += new Vector3(0, difference.y, 0);
            positions[6] = transform.GetChild(6).position;
            transform.GetChild(5).position += new Vector3(0, difference.y, 0);
            positions[5] = transform.GetChild(5).position;

            transform.GetChild(3).position = new Vector3(positions[3].x, transform.GetChild(3).position.y, positions[3].z);
            positions[3] = transform.GetChild(3).position;
        }
        else if (grabIndex == 4)
        {
            Vector3 difference = transform.GetChild(4).position - positions[4];

            transform.GetChild(7).position += new Vector3(0, difference.y, 0);
            positions[7] = transform.GetChild(7).position;
            transform.GetChild(8).position += new Vector3(0, difference.y, 0);
            positions[8] = transform.GetChild(8).position;

            transform.GetChild(4).position = new Vector3(positions[4].x, transform.GetChild(4).position.y, positions[4].z);
            positions[4] = transform.GetChild(4).position;
        }
        else if (grabIndex == 5)
        {
            Vector3 difference = transform.GetChild(5).position - positions[5];

            if (lockProportions)
            {
                if (Mathf.Abs(difference.x) < Mathf.Abs(difference.y))
                {
                    difference.y = difference.x * heightToWidthRatio * -1;
                } else
                {
                    difference.x = difference.y / heightToWidthRatio * -1;
                }
            }
            
            transform.GetChild(7).position += new Vector3(difference.x, 0, 0);
            positions[7] = transform.GetChild(7).position;
            transform.GetChild(6).position += new Vector3(0, difference.y, 0);
            positions[6] = transform.GetChild(6).position;

            positions[5] = positions[5] += difference;
            transform.GetChild(5).position = positions[5];
        }
        else if (grabIndex == 6)
        {
            Vector3 difference = transform.GetChild(6).position - positions[6];

            if (lockProportions)
            {
                if (Mathf.Abs(difference.x) < Mathf.Abs(difference.y))
                {
                    difference.y = difference.x * heightToWidthRatio;
                } else
                {
                    difference.x = difference.y / heightToWidthRatio;
                }
            }

            transform.GetChild(8).position += new Vector3(difference.x, 0, 0);
            positions[8] = transform.GetChild(8).position;
            transform.GetChild(5).position += new Vector3(0, difference.y, 0);
            positions[5] = transform.GetChild(5).position;

            positions[6] = positions[6] += difference;
            transform.GetChild(6).position = positions[6];
        }
        else if (grabIndex == 7)
        {
            Vector3 difference = transform.GetChild(7).position - positions[7];

            if (lockProportions)
            {
                if (Mathf.Abs(difference.x) < Mathf.Abs(difference.y))
                {
                    difference.y = difference.x * heightToWidthRatio;
                } else
                {
                    difference.x = difference.y / heightToWidthRatio;
                }
            }

            transform.GetChild(5).position += new Vector3(difference.x, 0, 0);
            positions[5] = transform.GetChild(5).position;
            transform.GetChild(8).position += new Vector3(0, difference.y, 0);
            positions[8] = transform.GetChild(8).position;

            positions[7] = positions[7] += difference;
            transform.GetChild(7).position = positions[7];
        }
        else if (grabIndex == 8)
        {
            Vector3 difference = transform.GetChild(8).position - positions[8];

            if (lockProportions)
            {
                if (Mathf.Abs(difference.x) < Mathf.Abs(difference.y))
                {
                    difference.y = difference.x * heightToWidthRatio * -1;
                } else
                {
                    difference.x = difference.y / heightToWidthRatio * -1;
                }
            }

            transform.GetChild(6).position += new Vector3(difference.x, 0, 0);
            positions[6] = transform.GetChild(6).position;
            transform.GetChild(7).position += new Vector3(0, difference.y, 0);
            positions[7] = transform.GetChild(7).position;

            positions[8] = positions[8] += difference;
            transform.GetChild(8).position = positions[8];
        }

        DistributePoints();
    }
}
