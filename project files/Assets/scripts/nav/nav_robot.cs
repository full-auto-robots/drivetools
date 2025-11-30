// might be a mistake, but I'm treating robots separatly to markers

// this is because markers display using images, and I'd like robots to be more mathematical
// point clouds are also not ideal for this use

// still, I might remove this class later

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class nav_robot
{
    public ushort teamNumber;

    public DoubleVector3 position;
    public double zAngle;

    public double width; // (m)
    public double length; // (m)

    public Color color;

    public nav_robot() { }

    public nav_robot(int teamNumber, DoubleVector3 position, double zAngle, double width, double length)
    {
        this.position = position;
        this.teamNumber = (ushort)teamNumber;

        this.zAngle = zAngle;
        this.width = width;
        this.length = length;

        this.color = Color.white;
    }
    public nav_robot(int teamNumber, DoubleVector3 position, double zAngle, double width, double length, Color color)
    {
        this.position = position;
        this.teamNumber = (ushort)teamNumber;

        this.zAngle = zAngle;
        this.width = width;
        this.length = length;

        this.color = color;
    }

    public static string EncodeToString(nav_robot input)
    {
        string result = "";

        result += "[";

        result += "{" + input.teamNumber.ToString() + "}";

        result += "{[" + "{" + input.position.x.ToString() + "}" + "{" + input.position.y.ToString() + "}" + "{" + input.position.z.ToString() + "}" + "]}";
        result += "{" + input.zAngle.ToString() + "}";

        result += "{" + input.width.ToString() + "}";
        result += "{" + input.length.ToString() + "}";

        result += "{" + "[" + "{" + input.color.r.ToString() + "}" + "{" + input.color.g.ToString() + "}" + "{" + input.color.b.ToString() + "}" + "]" + "}";

        result += "]";

        return result;
    }

    public static nav_robot DecodeFromString(string input)
    {
        nav_robot result = new nav_robot();

        input = input.Substring(1, input.Length - 2);

        result.teamNumber = ushort.Parse(input.Substring(Sys.FindFirstOccurance(input, '{') + 1, Sys.FindClosingBracket(input, Sys.FindFirstOccurance(input, '{')) - 1 - Sys.FindFirstOccurance(input, '{')));
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

        result.width = double.Parse(input.Substring(Sys.FindFirstOccurance(input, '{') + 1, Sys.FindClosingBracket(input, Sys.FindFirstOccurance(input, '{')) - 1 - Sys.FindFirstOccurance(input, '{')));
        input = Sys.SubtractVariable(input);

        result.length = double.Parse(input.Substring(Sys.FindFirstOccurance(input, '{') + 1, Sys.FindClosingBracket(input, Sys.FindFirstOccurance(input, '{')) - 1 - Sys.FindFirstOccurance(input, '{')));
        input = Sys.SubtractVariable(input);

        float r = float.Parse(input.Substring(Sys.FindOccurance(input, '{', 1) + 1, Sys.FindClosingBracket(input, Sys.FindOccurance(input, '{', 1)) - 1 - Sys.FindOccurance(input, '{', 1)));
        float g = float.Parse(input.Substring(Sys.FindOccurance(input, '{', 2) + 1, Sys.FindClosingBracket(input, Sys.FindOccurance(input, '{', 2)) - 1 - Sys.FindOccurance(input, '{', 2)));
        float b = float.Parse(input.Substring(Sys.FindOccurance(input, '{', 3) + 1, Sys.FindClosingBracket(input, Sys.FindOccurance(input, '{', 3)) - 1 - Sys.FindOccurance(input, '{', 3)));

        result.color = new Color(r, g, b, 1);

        return result;
    }
}
