using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// an array of points, packaged as a single struct

[System.Serializable]
public class nav_path
{
    public ushort drawMode; // this way paths can be used as polygons, trails, or just straight-up groups of points
    // USE THE ENUM TO REF

    public DoubleVector3[] positions;

    // since NO PUBLIC RELEASE has happened yet, 
    // I can feel free to change how the paths are encoded/decoded without backwards compat.

    // here we have colors for every individual point
    public Color[] pointColors;
    // and one color for all segments
    public Color segmentColor;

    public nav_path() { }

    public nav_path(DoubleVector3[] positions)
    {
        this.positions = positions;
    }
    public nav_path(DoubleVector3[] positions, ushort drawMode)
    {
        this.positions = positions;
        this.drawMode = drawMode;
    }
    // one col for everything
    public nav_path(DoubleVector3[] positions, Color color, ushort drawMode)
    {
        this.positions = positions;
        this.drawMode = drawMode;

        pointColors = new Color[positions.Length];
        for (int i = 0; i < positions.Length; i++) { pointColors[i] = color; }
        segmentColor = color;
    }
    public nav_path(DoubleVector3[] positions, Color color)
    {
        this.positions = positions;

        pointColors = new Color[positions.Length];
        for (int i = 0; i < positions.Length; i++) { pointColors[i] = color; }
        segmentColor = color;
    }
    // every color individual
    public nav_path(DoubleVector3[] positions, Color segmentColor, Color[] pointColors, ushort drawMode)
    {
        this.positions = positions;
        this.drawMode = drawMode;

        this.pointColors = pointColors;
        this.segmentColor = segmentColor;
    }
    public nav_path(DoubleVector3[] positions, Color segmentColor, Color[] pointColors)
    {
        this.positions = positions;

        this.pointColors = pointColors;
        this.segmentColor = segmentColor;
    }

    public void SetColor(Color color)
    {
        pointColors = new Color[positions.Length];
        for (int i = 0; i < positions.Length; i++) { pointColors[i] = color; }
        segmentColor = color;
    }

    // encoding ONE COLOR ONLY
    public static string EncodeToString(nav_path input)
    {
        string result = "";

        result += "[";

        result += "{" + input.drawMode.ToString() + "}";
        result += "{" + "[" + "{" + input.segmentColor.r.ToString() + "}" + "{" + input.segmentColor.g.ToString() + "}" + "{" + input.segmentColor.b.ToString() + "}" + "]" + "}";

        result += "{";
        for (int i = 0; i < input.positions.Length; i++)
        {
            result += "[" + "{" + input.positions[i].x.ToString() + "}" + "{" + input.positions[i].y.ToString() + "}" + "{" + input.positions[i].z.ToString() + "}" + "]";
        }
        result += "}";

        result += "]";

        return "$" + result;
    }
    // INDIVIDUAL COLORS
    public static string EncodeToStringComplex(nav_path input)
    {
        string result = "";

        // result += "[";

        // result += "{" + input.drawMode.ToString() + "}";

        // result += "{";
        // for (int i = 0; i < input.positions.Length; i++)
        // {
        //     result += "[" + "{" + input.positions[i].x.ToString() + "}" + "{" + input.positions[i].y.ToString() + "}" + "{" + input.positions[i].z.ToString() + "}" + "]";
        // }
        // result += "}";

        // result += "]";

        return "$" + result;
    }

    public static nav_path DecodeFromString(string input)
    {
        //Debug.Log(input);

        nav_path result = new nav_path();
        //Debug.Log("%" + input);

        input = input.Substring(1, input.Length - 2);

        result.drawMode = ushort.Parse(input.Substring(Sys.FindFirstOccurance(input, '{') + 1, Sys.FindClosingBracket(input, Sys.FindFirstOccurance(input, '{')) - 1 - Sys.FindFirstOccurance(input, '{')));
        input = Sys.SubtractVariable(input);

        float r = float.Parse(input.Substring(Sys.FindOccurance(input, '{', 1) + 1, Sys.FindClosingBracket(input, Sys.FindOccurance(input, '{', 1)) - 1 - Sys.FindOccurance(input, '{', 1)));
        float g = float.Parse(input.Substring(Sys.FindOccurance(input, '{', 2) + 1, Sys.FindClosingBracket(input, Sys.FindOccurance(input, '{', 2)) - 1 - Sys.FindOccurance(input, '{', 2)));
        float b = float.Parse(input.Substring(Sys.FindOccurance(input, '{', 3) + 1, Sys.FindClosingBracket(input, Sys.FindOccurance(input, '{', 3)) - 1 - Sys.FindOccurance(input, '{', 3)));

        // color
        input = Sys.SubtractVariable(input);

        List<DoubleVector3> points = new List<DoubleVector3>();

        int limitIndex = Sys.FindClosingBracket(input, 0);
        int currentIndex = Sys.FindFirstOccurance(input, '[');
        int numSkips = 0;
        while (currentIndex != -1 && currentIndex < limitIndex)
        {
            List<string> components = new List<string>();

            for (int i = 0; i < 3; i++)
            {
                int startIndex = Sys.FindOccurance(input, '{', i, currentIndex);
                int endIndex = Sys.FindClosingBracket(input, startIndex);

                components.Add(input.Substring(startIndex + 1, endIndex - startIndex - 1));
            }

            points.Add(new DoubleVector3(double.Parse(components[0]), double.Parse(components[1]), double.Parse(components[2])));

            numSkips++;
            currentIndex = Sys.FindOccurance(input, '[', numSkips);
        }

        result.positions = points.ToArray();

        result.SetColor(new Color(r, g, b, 1));

        return result;
    }
}
