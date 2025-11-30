using UnityEngine.UI;
using UnityEngine;

public enum LineDisplayMode
{
    Polygon, Line, Points, LoopedLine
}

public class ui_linerenderer : MonoBehaviour
{
    [Header("CONFIG")]
    public bool showSegments;
    public bool showPoints;
    public ushort drawMode;
    // public bool useWorldSpace; // TODO: implement this

    [Header("REFERENCES")]
    public Vector2[] points;
    public GameObject p_segment;
    public GameObject p_point;

    public Transform t_pointContainer;
    public Transform t_segmentsContainer;

    public Color color;

    // hides/shows things based on CONFIG
    public void Refresh()
    {
        for (int i = 0; i < t_pointContainer.childCount; i++)
        {
            t_pointContainer.GetChild(i).gameObject.SetActive(showPoints);
        }
        for (int i = 0; i < t_segmentsContainer.childCount; i++)
        {
            t_segmentsContainer.GetChild(i).gameObject.SetActive(showSegments);
        }
    }

    public void ApplyDrawMode(ushort mode)
    {
        drawMode = mode;
        if (drawMode != (ushort)LineDisplayMode.Points)
        {
            showSegments = true;
        }
        else
        {
            showSegments = false;
        }
        showPoints = true;

        Draw(); // easier just to regen the whole thing
    }

    public void ApplyDrawMode()
    {
        ApplyDrawMode(drawMode);
    }

    public void Draw(Vector2[] points)
    {
        this.points = points;
        Draw();
    }

    public void Draw()
    {
        CanvasUtils.DestroyChildren(t_pointContainer.gameObject);
        CanvasUtils.DestroyChildren(t_segmentsContainer.gameObject);

        for (int i = 0; i < points.Length; i++)
        {
            // the point obj ***
            Transform t_newPoint = Instantiate(p_point, t_pointContainer).transform;
            t_newPoint.localPosition = new Vector3(points[i].x, points[i].y, 0);

            t_newPoint.GetComponent<Image>().color = color;

            if (i < points.Length - 1)
            {
                // the line obj ***
                Transform t_newLine = Instantiate(p_segment, t_segmentsContainer).transform;
                t_newLine.localPosition = new Vector3(points[i].x, points[i].y, 0);

                t_newLine.GetComponent<Image>().color = color;

                Vector2 toNextPoint = points[i + 1] - points[i];
                t_newLine.eulerAngles = new Vector3(0, 0, Mathf.Atan2(toNextPoint.y, toNextPoint.x) * 180 / Mathf.PI);
                t_newLine.GetComponent<RectTransform>().sizeDelta = new Vector2(toNextPoint.magnitude, t_newLine.GetComponent<RectTransform>().sizeDelta.y);
            }
            else if (drawMode == (ushort)LineDisplayMode.LoopedLine)
            {
                Transform t_newLine = Instantiate(p_segment, t_segmentsContainer).transform;
                t_newLine.localPosition = new Vector3(points[i].x, points[i].y, 0);

                t_newLine.GetComponent<Image>().color = color;

                Vector2 toNextPoint = points[0] - points[i];
                t_newLine.eulerAngles = new Vector3(0, 0, Mathf.Atan2(toNextPoint.y, toNextPoint.x) * 180 / Mathf.PI);
                t_newLine.GetComponent<RectTransform>().sizeDelta = new Vector2(toNextPoint.magnitude, t_newLine.GetComponent<RectTransform>().sizeDelta.y);
            }
        }

        Refresh();
    }

    // stepping through the draw modes
    public void AdvanceDrawMode()
    {
        Debug.Log("Advancing draw mode...");

        if (drawMode < 3)
        {
            drawMode++;
        }
        else { drawMode = 0; }

        ApplyDrawMode();
    }

    // same thing, but only using LoopedLine and Polygon (bc in the field editor we want to see lines)
    public void AdvancePolyDrawMode()
    {
        Debug.Log("Advancing draw mode [polygon only]...");

        if (drawMode == 3)
        {
            drawMode = 0;
        }
        else { drawMode = 3; }

        ApplyDrawMode();
    }

    // draws a bunch of points as though part of a graph, where the function is just rand(x)
    public void Test(int numPoints, float length, float amplitude)
    {
        Vector2[] testPoints = new Vector2[numPoints];

        for (int i = 0; i < testPoints.Length; i++)
        {
            testPoints[i] = new Vector2((float)i / (float)numPoints * length, Random.Range(-amplitude, amplitude));
        }

        Draw(testPoints);
    }
}
