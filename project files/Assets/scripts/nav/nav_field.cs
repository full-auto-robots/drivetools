// data class for field nodes, encoded into a string for sending over NT3

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class nav_field
{
    public nav_robot[] robots;
    public nav_path[] paths;
    public nav_marker[] markers;

    public string EncodeToString()
    {
        return nav_field.EncodeToString(this);
    }

    public static string EncodeToString(nav_field input)
    {
        // encoding looks like this:
        // "field(robots{[]}paths{[]}markers{[]})"

        string result = "";

        result += "field(";

        // *** robots ***
        result += "robots{";
        for (int i = 0; i < input.robots.Length; i++)
        {
            result += nav_robot.EncodeToString(input.robots[i]);
        }
        result += "}";

        // *** paths ***
        result += "paths{";
        for (int i = 0; i < input.paths.Length; i++)
        {
            result += nav_path.EncodeToString(input.paths[i]);
        }
        result += "}";

        // *** markers ***
        result += "markers{";
        for (int i = 0; i < input.markers.Length; i++)
        {
            result += nav_marker.EncodeToString(input.markers[i]);
        }
        result += "}";

        result += ")";

        return "$" + result;
    }

    public static nav_field DecodeFromString(string input)
    {
        // "field(robots{[][][][]}paths{[][][][]}markers{[][][][]})"

        // $field(robots{$[{2386}{[{1.963777125624874E-4}{0.0010931810664655415}{0.0}]}{0.0400085449218901}{0.8}{0.8}]}paths{                                                      }markers{})
        // $field(robots{$[{2386}{[{0}                   {0}                    {0}  ]}{0}                 {0.5}{0.5}]}paths{$[{3}{[{-1}{0}{0}][{0}{1}{0}][{1}{0}{0}][{0}{-1}{0}]}]}markers{})

        

        nav_field result = new nav_field();

        if (input == "nulldata")
        {
            result.robots = new nav_robot[0];
            result.paths = new nav_path[0];
            result.markers = new nav_marker[0];

            return result;
        }

        int dataStartIndex = Sys.FindFirstOccurance(input, '{');
        int dataEndIndex = Sys.FindClosingBracket(input, dataStartIndex);

        string isolatedData = Sys.SnipString(input, dataStartIndex, dataEndIndex);
        string[] splitData = Sys.SplitString(isolatedData, '[');
        List<nav_robot> decodedRobotData = new List<nav_robot>();

        for (int i = 0; i < splitData.Length; i++)
        {
            decodedRobotData.Add(nav_robot.DecodeFromString(splitData[i]));
        }
        input = input.Substring(dataEndIndex + 1, input.Length - dataEndIndex - 1);

        dataStartIndex = Sys.FindFirstOccurance(input, '{');
        dataEndIndex = Sys.FindClosingBracket(input, dataStartIndex);

        isolatedData = Sys.SnipString(input, dataStartIndex, dataEndIndex);
        splitData = Sys.SplitString(isolatedData, '[');
        List<nav_path> decodedPathData = new List<nav_path>();

        for (int i = 0; i < splitData.Length; i++)
        {
            decodedPathData.Add(nav_path.DecodeFromString(splitData[i]));
        }
        input = input.Substring(dataEndIndex + 1, input.Length - dataEndIndex - 1);

        dataStartIndex = Sys.FindFirstOccurance(input, '{');
        dataEndIndex = Sys.FindClosingBracket(input, dataStartIndex);

        isolatedData = Sys.SnipString(input, dataStartIndex, dataEndIndex);
        splitData = Sys.SplitString(isolatedData, '[');
        List<nav_marker> decodedMarkerData = new List<nav_marker>();

        for (int i = 0; i < splitData.Length; i++)
        {
            decodedMarkerData.Add(nav_marker.DecodeFromString(splitData[i]));
        }

        result.robots = decodedRobotData.ToArray();
        result.markers = decodedMarkerData.ToArray();
        result.paths = decodedPathData.ToArray();

        return result;
    }

    // testing whether the encoding and decoding (to and from a string var) works properly
    // returns true if success, false if not
    // public static bool TestEncodeDecode()
    // {
    //     nav_field testField = new nav_field();

    //     testField.robots = new nav_robot[0];
    //     testField.paths = new nav_path[0];
    //     testField.markers = new nav_marker[0];

    //     string encodedField = EncodeToString(testField);

    //     nav_field decodedField = DecodeFromString(encodedField);

    //     bool hasTestPassed = true;

    //     // basic tests to figure out any obvious issues
    //     if (decodedField.robots.Length != testField.robots.Length) { hasTestPassed = false; }
    //     if (decodedField.paths.Length != testField.paths.Length) { hasTestPassed = false; }
    //     if (decodedField.markers.Length != testField.markers.Length) { hasTestPassed = false; }

    //     if (hasTestPassed)
    //     {
    //         // now go and search the array specifically

    //     }

    //     return hasTestPassed;
    // }
}
