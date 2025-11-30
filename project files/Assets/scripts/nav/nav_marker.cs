using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// simple struct, basically an integer that refs an image, plus position and rotation obv

[System.Serializable]
public class nav_marker
{
    public ushort displayIndex; // references an array to get the image needed

    public DoubleVector3 position;
    public double zAngle;
    public double size; // meters
    public Color color;

    public nav_marker() { }
    public nav_marker(ushort displayIndex, Vector3 position, float zAngle)
    {
        this.displayIndex = displayIndex;
        this.position = new DoubleVector3(position);
        this.zAngle = zAngle;
        this.color = Color.white;
        this.size = 0.5f;
    }
    public nav_marker (ushort displayIndex, Vector3 position, float zAngle, float size, Color color)
    {
        this.displayIndex = displayIndex;
        this.position = new DoubleVector3(position);
        this.zAngle = zAngle;
        this.size = size;
        this.color = color;
    }

    public static string EncodeToString(nav_marker input)
    {
        string result = "";

        result += "[";

        result += "{" + input.displayIndex.ToString() + "}";

        result += "{[" + "{" + input.position.x.ToString() + "}" + "{" + input.position.y.ToString() + "}" + "{" + input.position.z.ToString() + "}" + "]}";
        result += "{" + input.zAngle.ToString() + "}";
        result += "{" + input.size.ToString() + "}";

        result += "{" + "[" + "{" + input.color.r.ToString() + "}" + "{" + input.color.g.ToString() + "}" + "{" + input.color.b.ToString() + "}" + "]" + "}";

        result += "]";

        return "$" + result;
    }

    public static nav_marker DecodeFromString(string input)
    {
        nav_marker result = new nav_marker();

        input = input.Substring(1, input.Length - 2);

        result.displayIndex = ushort.Parse(input.Substring(Sys.FindFirstOccurance(input, '{') + 1, Sys.FindClosingBracket(input, Sys.FindFirstOccurance(input, '{')) - 1 - Sys.FindFirstOccurance(input, '{')));
        input = Sys.SubtractVariable(input);

        List<string> components = new List<string>();

        for (int i = 1; i < 4; i++)
        {
            int startIndex = Sys.FindOccurance(input, '{', i);
            int endIndex = Sys.FindClosingBracket(input, startIndex);

            components.Add(input.Substring(startIndex + 1, endIndex - startIndex - 1));
        }

        result.position = new DoubleVector3(double.Parse(components[0]), double.Parse(components[1]), double.Parse(components[2]));
        input = Sys.SubtractVariable(input);

        result.zAngle = double.Parse(input.Substring(Sys.FindFirstOccurance(input, '{') + 1, Sys.FindClosingBracket(input, Sys.FindFirstOccurance(input, '{')) - 1 - Sys.FindFirstOccurance(input, '{')));
        input = Sys.SubtractVariable(input);
        result.size = double.Parse(input.Substring(Sys.FindFirstOccurance(input, '{') + 1, Sys.FindClosingBracket(input, Sys.FindFirstOccurance(input, '{')) - 1 - Sys.FindFirstOccurance(input, '{')));
        input = Sys.SubtractVariable(input);

        float r = float.Parse(input.Substring(Sys.FindOccurance(input, '{', 1) + 1, Sys.FindClosingBracket(input, Sys.FindOccurance(input, '{', 1)) - 1 - Sys.FindOccurance(input, '{', 1)));
        float g = float.Parse(input.Substring(Sys.FindOccurance(input, '{', 2) + 1, Sys.FindClosingBracket(input, Sys.FindOccurance(input, '{', 2)) - 1 - Sys.FindOccurance(input, '{', 2)));
        float b = float.Parse(input.Substring(Sys.FindOccurance(input, '{', 3) + 1, Sys.FindClosingBracket(input, Sys.FindOccurance(input, '{', 3)) - 1 - Sys.FindOccurance(input, '{', 3)));

        result.color = new Color(r, g, b, 1);

        return result;
    }
}
